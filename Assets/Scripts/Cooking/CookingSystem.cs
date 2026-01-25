using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookingSystem : MonoBehaviour
{
    public static CookingSystem Instance;

    // [삭제함] 더 이상 쓰지 않는 이벤트라 지웠습니다. (경고 해결)
    // public event Action<int, FoodState, string> OnCookingCompleted;

    [Header("멀티 팬 설정")]
    [SerializeField] private Slider[] timerBars;

    // 팬 상태 관리를 위한 배열들
    private FoodState[] panStates;
    private Coroutine[] cookingCoroutines;
    private RecipeData[] activeRecipes;
    private DragAndDropManager[] activeDoughs;

    [Header("Dependencies")]
    public InventorySystem inventorySystem;

    [Header("Visual References")]
    public Sprite onPanSprite;
    public Sprite rawSprite;
    public Sprite undercookedSprite;
    public Sprite perfectSprite;
    public Sprite burntSprite;

    [Header("레시피 설정")]
    public List<RecipeData> recipeList = new List<RecipeData>();
    private Dictionary<string, RecipeData> recipeDictionary = new Dictionary<string, RecipeData>();

    [Header("Cooking Timers")]
    [SerializeField] private float onPanTime = 1.0f;
    [SerializeField] private float undercookedTime = 2.0f;
    [SerializeField] private float perfectTime = 2.0f;
    [SerializeField] private float burntTime = 2.0f;

    private void Awake()
    {
        Instance = this;

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

    public void StartCooking(int panIndex, string recipeID, DragAndDropManager dough)
    {
        if (panIndex < 0 || panIndex >= timerBars.Length) return;

        activeDoughs[panIndex] = dough;

        if (cookingCoroutines[panIndex] != null)
        {
            StopCoroutine(cookingCoroutines[panIndex]);
            Debug.Log($"{panIndex}번 팬은 이미 조리 중입니다. 기존 조리를 중단하고 새로 시작합니다.");
        }

        if (recipeDictionary.TryGetValue(recipeID, out RecipeData recipe))
        {
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
    }

    private IEnumerator CookFoodRoutine(int panIndex, RecipeData recipe)
    {
        float elapsedTime = 0f;
        float totalPerfectTime = onPanTime + undercookedTime + perfectTime;

        // 0단계: OnPan -> Raw 
        while (elapsedTime < onPanTime)
        {
            elapsedTime += Time.deltaTime;
            timerBars[panIndex].value = elapsedTime / totalPerfectTime;
            yield return null;
        }
        UpdatePanState(panIndex, FoodState.Raw);

        // 1단계: Raw -> Undercooked 
        while (elapsedTime < onPanTime + undercookedTime)
        {
            elapsedTime += Time.deltaTime;
            timerBars[panIndex].value = elapsedTime / totalPerfectTime;
            yield return null;
        }
        UpdatePanState(panIndex, FoodState.Undercooked);

        // 2단계: Undercooked -> Perfect 
        while (elapsedTime < totalPerfectTime)
        {
            elapsedTime += Time.deltaTime;
            timerBars[panIndex].value = elapsedTime / totalPerfectTime;
            yield return null;
        }
        UpdatePanState(panIndex, FoodState.Perfect);

        // 3단계: Perfect -> Burnt 
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

    private void UpdatePanState(int panIndex, FoodState newState)
    {
        panStates[panIndex] = newState;
        UpdateDoughVisual(panIndex, newState);
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

        // EconomyManager를 직접 호출하여 처리 (Burnt 상태 전달 -> 0원 처리)
        if (activeRecipes[panIndex] != null)
        {
            if (EconomyManager.Instance != null)
            {
                EconomyManager.Instance.CalculateRevenue(panIndex, FoodState.Burnt, activeRecipes[panIndex].ID);
            }
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

        // [핵심 로직 연결] EconomyManager 직접 호출
        if (activeRecipes[panIndex] != null)
        {
            // 1. 매니저가 있는지 확인하고 직접 호출
            if (EconomyManager.Instance != null)
            {
                EconomyManager.Instance.CalculateRevenue(panIndex, panStates[panIndex], activeRecipes[panIndex].ID);
                Debug.Log($"[CookingSystem] 매니저에게 정산 요청 보냄 (상태: {panStates[panIndex]})");
            }
            else
            {
                // 2. 만약 Instance가 없다면 비상 연락망으로 찾아서 호출
                var ecoManager = FindAnyObjectByType<EconomyManager>();
                if (ecoManager != null)
                {
                    ecoManager.CalculateRevenue(panIndex, panStates[panIndex], activeRecipes[panIndex].ID);
                    Debug.Log($"[CookingSystem] 매니저(Find)에게 정산 요청 보냄 (상태: {panStates[panIndex]})");
                }
                else
                {
                    Debug.LogError("[CookingSystem] EconomyManager가 씬에 없습니다! 정산 불가!");
                }
            }
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

        panStates[panIndex] = FoodState.OnPan;
        Debug.Log($"{panIndex + 1}번 팬에서 음식을 집어 올렸습니다. 조리 중단!");
    }
}