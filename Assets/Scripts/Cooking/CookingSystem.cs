using System; // Action 이벤트를 사용하기 위해 필수
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookingSystem : MonoBehaviour
{
    public static CookingSystem Instance;

    // 정의: 외부로 보낼 이벤트 (매개변수: 팬 번호, 음식 상태, 레시피ID)
    // EconomyManager 등 다른 시스템이 이 신호를 듣고 돈을 계산합니다.
    public event Action<int, FoodState, string> OnCookingCompleted;

    [Header("멀티 팬 설정")]
    // 조리 씬의 Slider들을 순서대로 연결하세요.
    [SerializeField] private Slider[] timerBars;

    // 팬 상태 관리를 위한 배열들
    private FoodState[] panStates;
    private Coroutine[] cookingCoroutines;
    private RecipeData[] activeRecipes; // 현재 팬에서 요리 중인 레시피 정보 저장
    private DragAndDropManager[] activeDoughs; // 현재 팬 위에 있는 도우 객체

    [Header("Dependencies")]
    public InventorySystem inventorySystem;

    [Header("Visual References")]
    public Sprite onPanSprite;
    public Sprite rawSprite;
    public Sprite undercookedSprite;
    public Sprite perfectSprite;
    public Sprite burntSprite;

    [Header("레시피 설정")]
    // 인스펙터에서 만든 RecipeData 파일들을 여기에 드래그해서 넣으세요.
    public List<RecipeData> recipeList = new List<RecipeData>();

    private Dictionary<string, RecipeData> recipeDictionary = new Dictionary<string, RecipeData>();

    [Header("Cooking Timers")]
    [SerializeField] private float onPanTime = 1.0f;
    [SerializeField] private float undercookedTime = 2.0f; // 설익음 도달 시간
    [SerializeField] private float perfectTime = 2.0f;    // 설익음 이후 완성까지 시간
    [SerializeField] private float burntTime = 2.0f;      // 완성 이후 타기까지 시간

    private void Awake()
    {
        // 싱글톤 설정: Managers 프리팹이 씬 전환 시에도 유지되도록 함
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 배열 크기 초기화 (인스펙터에 연결된 TimerBar 개수에 맞춰서 동적으로 할당)
        // 이렇게 하면 팬이 1개든 3개든 코드를 수정할 필요가 없습니다.
        int panCount = timerBars != null ? timerBars.Length : 0;

        panStates = new FoodState[panCount];
        cookingCoroutines = new Coroutine[panCount];
        activeRecipes = new RecipeData[panCount];
        activeDoughs = new DragAndDropManager[panCount];

        for (int i = 0; i < panCount; i++)
        {
            panStates[i] = FoodState.OnPan;
        }

        foreach (RecipeData recipe in recipeList)
        {
            if (recipe != null && !recipeDictionary.ContainsKey(recipe.ID))
            {
                recipeDictionary.Add(recipe.ID, recipe);
            }
        }
    }

    /// <summary>
    /// 외부(DragAndDropManager 등)에서 조리를 시작할 때 호출합니다.
    /// </summary>
    /// <param name="panIndex">팬 번호 (0부터 시작)</param>
    /// <param name="recipeID">레시피 데이터 ID</param>
    public void StartCooking(int panIndex, string recipeID, DragAndDropManager dough)
    {
        if (panIndex < 0 || panIndex >= timerBars.Length) return;

        activeDoughs[panIndex] = dough;

        // 이미 해당 팬이 조리 중인지 체크
        if (cookingCoroutines[panIndex] != null)
        {
            StopCoroutine(cookingCoroutines[panIndex]);
            Debug.Log($"{panIndex}번 팬은 이미 조리 중입니다. 기존 조리를 중단하고 새로 시작합니다.");
        }

        // 레시피 데이터 확인
        if (recipeDictionary.TryGetValue(recipeID, out RecipeData recipe))
        {
            // [중요] 현재 팬에서 어떤 레시피를 요리하는지 저장
            activeRecipes[panIndex] = recipe;

            timerBars[panIndex].gameObject.SetActive(true);

            UpdateDoughVisual(panIndex, FoodState.OnPan);
            cookingCoroutines[panIndex] = StartCoroutine(CookFoodRoutine(panIndex, recipe));
        }
        else
        {
            Debug.LogWarning($"레시피 ID '{recipeID}'를 찾을 수 없습니다.");
        }
    }

    private void UpdateDoughVisual(int panIndex, FoodState newState)
    {
        if (activeDoughs[panIndex] == null) return;

        activeDoughs[panIndex].currentFoodState = newState;

        SpriteRenderer sr = activeDoughs[panIndex].GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            switch (newState)
            {
                case FoodState.OnPan: sr.sprite = onPanSprite; break;
                case FoodState.Raw: sr.sprite = rawSprite; break;
                case FoodState.Undercooked: sr.sprite = undercookedSprite; break;
                case FoodState.Perfect: sr.sprite = perfectSprite; break;
                case FoodState.Burnt: sr.sprite = burntSprite; break;
            }
        }
        // 디버그 로그가 너무 많으면 주석 처리 하셔도 됩니다.
        // Debug.Log($"{panIndex}번 팬 음식 상태 변경: {newState}");
    }

    /// <summary>
    /// 조리 과정을 단계별로 관리하는 코루틴입니다.
    /// </summary>
    private IEnumerator CookFoodRoutine(int panIndex, RecipeData recipe)
    {
        float elapsedTime = 0f;
        float totalPerfectTime = onPanTime + undercookedTime + perfectTime;

        // 0단계: OnPan -> Raw (초기 가열)
        while (elapsedTime < onPanTime)
        {
            elapsedTime += Time.deltaTime;
            timerBars[panIndex].value = elapsedTime / totalPerfectTime;
            yield return null;
        }
        UpdatePanState(panIndex, FoodState.Raw);

        // 1단계: Raw -> Undercooked (설익음)
        while (elapsedTime < onPanTime + undercookedTime)
        {
            elapsedTime += Time.deltaTime;
            timerBars[panIndex].value = elapsedTime / totalPerfectTime;
            yield return null;
        }
        UpdatePanState(panIndex, FoodState.Undercooked);

        // 2단계: Undercooked -> Perfect (잘 익음)
        while (elapsedTime < totalPerfectTime)
        {
            elapsedTime += Time.deltaTime;
            timerBars[panIndex].value = elapsedTime / totalPerfectTime;
            yield return null;
        }
        UpdatePanState(panIndex, FoodState.Perfect);

        // 3단계: Perfect -> Burnt (탐) - 대기 시간
        // 이 구간에서는 게이지가 꽉 찬 상태 유지 혹은 별도 처리가 가능
        float timeSpentBurning = 0f;
        while (timeSpentBurning < burntTime)
        {
            timeSpentBurning += Time.deltaTime;
            yield return null;
        }

        // 4단계: 타버림 처리
        UpdatePanState(panIndex, FoodState.Burnt);
        FailCooking(panIndex);
    }

    /// <summary>
    /// 팬의 상태 값을 변경하고 비주얼 업데이트를 호출합니다.
    /// </summary>
    private void UpdatePanState(int panIndex, FoodState newState)
    {
        panStates[panIndex] = newState;
        UpdateDoughVisual(panIndex, newState); // activeDoughs[panIndex]는 내부에서 사용
    }

    public void StopCookingVisual(int panIndex)
    {
        if (cookingCoroutines[panIndex] != null)
        {
            StopCoroutine(cookingCoroutines[panIndex]);
            cookingCoroutines[panIndex] = null;
            Debug.Log($"{panIndex}번 팬 조리 중단 (게이지 멈춤)");
        }
    }

    /// <summary>
    /// 요리가 타버렸을 때 호출됩니다.
    /// </summary>
    public void FailCooking(int panIndex)
    {
        if (cookingCoroutines[panIndex] != null)
        {
            StopCoroutine(cookingCoroutines[panIndex]);
            cookingCoroutines[panIndex] = null;
        }

        // [이벤트 발송] Burnt 상태를 EconomyManager 등에 알림 (돈 0원 처리 등을 위해)
        if (activeRecipes[panIndex] != null)
        {
            OnCookingCompleted?.Invoke(panIndex, FoodState.Burnt, activeRecipes[panIndex].ID);
        }

        Debug.Log($"{panIndex + 1}번 팬의 요리가 타버렸습니다! [가격: 0원]");
    }

    /// <summary>
    /// 플레이어가 적절한 타이밍에 요리를 꺼냈을 때 호출해야 합니다. (클릭 등)
    /// </summary>
    public void CompleteCooking(int panIndex)
    {
        if (cookingCoroutines[panIndex] != null)
        {
            StopCoroutine(cookingCoroutines[panIndex]);
            cookingCoroutines[panIndex] = null;
        }

        // 타이머 바 숨기기
        timerBars[panIndex].gameObject.SetActive(false);

        // [핵심 로직 연결] 현재 상태와 레시피 정보를 EconomyManager에 방송합니다.
        if (activeRecipes[panIndex] != null)
        {
            // 현재 팬의 상태(Perfect, Undercooked 등)를 보냅니다.
            OnCookingCompleted?.Invoke(panIndex, panStates[panIndex], activeRecipes[panIndex].ID);
            Debug.Log($"[CookingSystem] 조리 완료 신호 보냄: {panStates[panIndex]}");
        }
        else
        {
            Debug.LogError("조리 중인 레시피 정보가 없습니다!");
        }
    }

    public void StopCookingManually(int panIndex)
    {
        if (cookingCoroutines[panIndex] != null)
        {
            StopCoroutine(cookingCoroutines[panIndex]);
            cookingCoroutines[panIndex] = null;
        }

        // 팬 상태를 다시 초기 상태로 변경 (필요 시)
        panStates[panIndex] = FoodState.OnPan;

        Debug.Log($"{panIndex + 1}번 팬에서 음식을 집어 올렸습니다. 조리 중단!");
    }
}
