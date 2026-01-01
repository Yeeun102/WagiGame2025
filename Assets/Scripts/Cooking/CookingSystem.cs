using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookingSystem : MonoBehaviour
{
    public static CookingSystem Instance;

    [Header("멀티 팬 설정")]
    // 조리 씬의 Slider 2개를 순서대로 연결하세요.
    [SerializeField] private Slider[] timerBars;
    // 조리 씬의 팬 SpriteRenderer 2개를 순서대로 연결하세요.
    //[SerializeField] private SpriteRenderer[] panRenderers;

    private FoodState[] panStates = new FoodState[2];
    private Coroutine[] cookingCoroutines = new Coroutine[2];

    [Header("Dependencies")]
    public InventorySystem inventorySystem;

    [Header("Visual References")]
    public Sprite rawSprite;
    public Sprite undercookedSprite;
    public Sprite perfectSprite;
    public Sprite burntSprite;

    [Header("레시피 설정")]
    // 인스펙터에서 만든 RecipeData 파일들을 여기에 드래그해서 넣으세요.
    public List<RecipeData> recipeList = new List<RecipeData>();

    private Dictionary<string, RecipeData> recipeDictionary = new Dictionary<string, RecipeData>();

    [Header("Cooking Timers")]
    [SerializeField] private float undercookedTime = 2.0f; // 설익음 도달 시간 [cite: 49]
    [SerializeField] private float perfectTime = 2.0f;    // 설익음 이후 완성까지 시간 [cite: 50]
    [SerializeField] private float burntTime = 2.0f;      // 완성 이후 타기까지 시간 [cite: 50]

    private DragAndDropManager[] activeDoughs = new DragAndDropManager[2];

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

        // 팬 상태 초기화
        for (int i = 0; i < panStates.Length; i++)
        {
            panStates[i] = FoodState.Raw;
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
    /// <param name="panIndex">0 또는 1 (팬 번호)</param>
    /// <param name="recipeID">레시피 데이터 ID</param>
    public void StartCooking(int panIndex, string recipeID, DragAndDropManager dough)
    {
        if (panIndex < 0 || panIndex >= timerBars.Length) return;
        activeDoughs[panIndex] = dough;

        // 이미 해당 팬이 조리 중인지 체크
        if (cookingCoroutines[panIndex] != null)
        {
            Debug.Log($"{panIndex}번 팬은 이미 조리 중입니다.");
            return;
        }

        // 레시피 데이터 확인
        if (recipeDictionary.TryGetValue(recipeID, out RecipeData recipe))
        {
            timerBars[panIndex].gameObject.SetActive(true);
            // [수정] 이제 panRenderers 대신 activeDoughs의 이미지를 바꿉니다.
            UpdateDoughVisual(panIndex, FoodState.Raw);
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
        switch (newState)
        {
            case FoodState.Raw: sr.sprite = rawSprite; break;
            case FoodState.Undercooked: sr.sprite = undercookedSprite; break;
            case FoodState.Perfect: sr.sprite = perfectSprite; break;
            case FoodState.Burnt: sr.sprite = burntSprite; break;
        }
    }

    /// <summary>
    /// 조리 과정을 단계별로 관리하는 코루틴입니다.
    /// </summary>
    private IEnumerator CookFoodRoutine(int panIndex, RecipeData recipe)
    {
        float elapsedTime = 0f;
        float totalPerfectTime = undercookedTime + perfectTime;

        // 1단계: Raw -> Undercooked (설익음) [cite: 49]
        while (elapsedTime < undercookedTime)
        {
            elapsedTime += Time.deltaTime;
            timerBars[panIndex].value = elapsedTime / totalPerfectTime;
            yield return null;
        }
        UpdatePanState(panIndex, FoodState.Undercooked);

        // 2단계: Undercooked -> Perfect (잘 익음) [cite: 50]
        while (elapsedTime < totalPerfectTime)
        {
            elapsedTime += Time.deltaTime;
            timerBars[panIndex].value = elapsedTime / totalPerfectTime;
            yield return null;
        }
        UpdatePanState(panIndex, FoodState.Perfect);

        // 3단계: Perfect -> Burnt (탐) - 대기 시간 [cite: 50]
        float timeSpentBurning = 0f;
        while (timeSpentBurning < burntTime)
        {
            timeSpentBurning += Time.deltaTime;
            yield return null;
        }

// 4단계: 타버림 처리 [cite: 26, 50]
        UpdatePanState(panIndex, FoodState.Burnt);
        FailCooking(panIndex);
    }

    /// <summary>
    /// 팬의 상태 값을 변경하고 비주얼 업데이트를 호출합니다.
    /// </summary>
    private void UpdatePanState(int panIndex, FoodState newState)
    {
        panStates[panIndex] = newState;
        UpdatePanVisual(panIndex, newState);
    }

    /// <summary>
    /// 실제 팬 오브젝트의 SpriteRenderer를 변경합니다.
    /// </summary>
    public void UpdatePanVisual(int panIndex, FoodState newState)
    {
        if (activeDoughs[panIndex] == null) return;

        // 해당 도우의 SpriteRenderer를 가져와서 이미지 교체
        SpriteRenderer sr = activeDoughs[panIndex].GetComponent<SpriteRenderer>();

        switch (newState)
        {
            case FoodState.Raw: sr.sprite = rawSprite; break;
            case FoodState.Undercooked: sr.sprite = undercookedSprite; break;
            case FoodState.Perfect: sr.sprite = perfectSprite; break;
            case FoodState.Burnt: sr.sprite = burntSprite; break;
        }
    }

    /// <summary>
/// 요리가 타버렸을 때 호출됩니다. [cite: 26]
                /// </summary>
    public void FailCooking(int panIndex)
    {
        if (cookingCoroutines[panIndex] != null)
        {
            StopCoroutine(cookingCoroutines[panIndex]);
            cookingCoroutines[panIndex] = null;
        }
        //timerBars[panIndex].gameObject.SetActive(false);
        Debug.Log($"{panIndex + 1}번 팬의 요리가 타버렸습니다! [가격: 0원]");
    }

    /// <summary>
    /// 플레이어가 적절한 타이밍에 요리를 꺼냈을 때 호출해야 합니다.
    /// </summary>
    public void CompleteCooking(int panIndex)
    {
        if (cookingCoroutines[panIndex] != null)
        {
            StopCoroutine(cookingCoroutines[panIndex]);
            cookingCoroutines[panIndex] = null;
        }
        timerBars[panIndex].gameObject.SetActive(false);
        // 여기서 만족도 및 금액 정산 로직을 추가합니다. [cite: 29]
    }

    public void StopCookingManually(int panIndex)
    {
        if (cookingCoroutines[panIndex] != null)
        {
            StopCoroutine(cookingCoroutines[panIndex]);
            cookingCoroutines[panIndex] = null;
        }

        // 타이머 바 숨기기
        timerBars[panIndex].gameObject.SetActive(false);

        // 팬 상태를 다시 Raw나 초기 상태로 변경 (필요 시)
        panStates[panIndex] = FoodState.Raw;

        Debug.Log($"{panIndex + 1}번 팬에서 음식을 집어 올렸습니다. 조리 중단!");
    }
}