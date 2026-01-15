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

    private FoodState[] panStates = new FoodState[1];
    private Coroutine[] cookingCoroutines = new Coroutine[1];

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
    [SerializeField] private float undercookedTime = 2.0f; // ������ ���� �ð� [cite: 49]
    [SerializeField] private float perfectTime = 2.0f;    // ������ ���� �ϼ����� �ð� [cite: 50]
    [SerializeField] private float burntTime = 2.0f;      // �ϼ� ���� Ÿ����� �ð� [cite: 50]


    private DragAndDropManager[] activeDoughs = new DragAndDropManager[1];

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
    /// <param name="panIndex">0 또는 1 (팬 번호)</param>
    /// <param name="recipeID">레시피 데이터 ID</param>
    public void StartCooking(int panIndex, string recipeID, DragAndDropManager dough)
    {
        if (panIndex < 0 || panIndex >= timerBars.Length) return;
        activeDoughs[panIndex] = dough;

        // 이미 해당 팬이 조리 중인지 체크
        if (cookingCoroutines[panIndex] != null)
        {

            StopCoroutine(cookingCoroutines[panIndex]);
            Debug.Log($"{panIndex}�� ���� �̹� ���� ���Դϴ�.");

            return;
        }

        // 레시피 데이터 확인
        if (recipeDictionary.TryGetValue(recipeID, out RecipeData recipe))
        {
            timerBars[panIndex].gameObject.SetActive(true);

            // [����] ���� panRenderers ��� activeDoughs�� �̹����� �ٲߴϴ�.

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
            case FoodState.OnPan: sr.sprite = onPanSprite; break;
            case FoodState.Raw: sr.sprite = rawSprite; break;
            case FoodState.Undercooked: sr.sprite = undercookedSprite; break;
            case FoodState.Perfect: sr.sprite = perfectSprite; break;
            case FoodState.Burnt: sr.sprite = burntSprite; break;
        }
        Debug.Log($"{panIndex}�� �� ���� ���� ����: {newState}");
    }

    /// <summary>
    /// 조리 과정을 단계별로 관리하는 코루틴입니다.
    /// </summary>
    private IEnumerator CookFoodRoutine(int panIndex, RecipeData recipe)
    {
        float elapsedTime = 0f;
        float totalPerfectTime = onPanTime + undercookedTime + perfectTime;

        while (elapsedTime < onPanTime)
        {
            elapsedTime += Time.deltaTime;
            timerBars[panIndex].value = elapsedTime / totalPerfectTime;

            yield return null;
        }
        UpdatePanState(panIndex, FoodState.Raw);


        // 1�ܰ�: Raw -> Undercooked (������) [cite: 49]
        while (elapsedTime < onPanTime + undercookedTime)

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
        cookingCoroutines[panIndex] = null;
    }

    /// <summary>
    /// 팬의 상태 값을 변경하고 비주얼 업데이트를 호출합니다.
    /// </summary>
    private void UpdatePanState(int panIndex, FoodState newState)
    {
        panStates[panIndex] = newState;
        UpdateFoodState(panIndex, newState, activeDoughs[panIndex]);
    }

    public void StopCookingVisual(int panIndex)
    {
        if (cookingCoroutines[panIndex] != null)
        {
            StopCoroutine(cookingCoroutines[panIndex]);
            cookingCoroutines[panIndex] = null;
            Debug.Log($"{panIndex}�� �� ���� �ߴ� (������ ����)");
        }
    }


    /// <summary>
    /// 실제 팬 오브젝트의 SpriteRenderer를 변경합니다.
    /// </summary>
    private void UpdateFoodState(int panIndex, FoodState newState, DragAndDropManager dough)
    {
        if (dough == null) dough = activeDoughs[panIndex];
        if (activeDoughs[panIndex] == null) return;


        dough.currentFoodState = newState;

        // �ش� ������ SpriteRenderer�� �����ͼ� �̹��� ��ü

        SpriteRenderer sr = activeDoughs[panIndex].GetComponent<SpriteRenderer>();

        switch (newState)
        {
            case FoodState.OnPan: sr.sprite = onPanSprite; break;
            case FoodState.Raw: sr.sprite = rawSprite; break;
            case FoodState.Undercooked: sr.sprite = undercookedSprite; break;
            case FoodState.Perfect: sr.sprite = perfectSprite; break;
            case FoodState.Burnt: sr.sprite = burntSprite; break;
        }
        Debug.Log($"�� {panIndex} ���� ������ ����: {newState}");
    }


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


        // Ÿ�̸� �� �����
        //timerBars[panIndex].gameObject.SetActive(false);

        // �� ���¸� �ٽ� Raw�� �ʱ� ���·� ���� (�ʿ� ��)
        panStates[panIndex] = FoodState.OnPan;

        Debug.Log($"{panIndex + 1}번 팬에서 음식을 집어 올렸습니다. 조리 중단!");
    }
}