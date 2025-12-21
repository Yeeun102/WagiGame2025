using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingSystem : MonoBehaviour
{
    public static CookingSystem Instance;

    [Header("Dependencies")]
    // 필요한 의존성 (Inspector에서 연결)
    public InventorySystem inventorySystem;
    public Transform panSpawnPoint; // 팬이 생성될 위치

    [Header("Visual References")]
    public Sprite rawSprite;
    public Sprite undercookedSprite;
    public Sprite perfectSprite;
    public Sprite burntSprite;

    private SpriteRenderer spriteRenderer;

    // 현재 사용 가능한 레시피 데이터 목록
    private Dictionary<string, RecipeData> recipeDictionary = new Dictionary<string, RecipeData>();

    [Header("조리 상태")]
    public FoodState currentState = FoodState.Raw;

    private List<ToppingType> toppings = new List<ToppingType>();

    [Header("Cooking Timers")]
    [SerializeField] private float undercookedTime = 2.0f; // 덜익는 시간 (수정예정)
    [SerializeField] private float perfectTime = 2.0f; // 완벽하게 익기까지의 시간(덜익+완벽)
    [SerializeField] private float burntTime = 2.0f; // 타게 되는 시간 (덜익+완벽+태움)

    [SerializeField] public UnityEngine.UI.Slider timerBar;

    private Coroutine cookingCoroutine;
    private RecipeData currentRecipe;
    private GameObject currentFood;

    public void InitializePan(RecipeData recipe)
    {
        currentRecipe = recipe;
    }

    public void ChangeState(FoodState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        UpdateVisualState();

    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisualState();
    }

    public void StartCooking(string recipeID)
    {
        // TODO: 조리 시작 (타이밍 바 생성 등)
        timerBar.gameObject.SetActive(true);

        cookingCoroutine = StartCoroutine(CookFoodRoutine());
    }

    public void CompleteCooking()
    {
        // TODO: 요리 완성 처리
        if (cookingCoroutine != null)
        {
            StopCoroutine(cookingCoroutine);
        }
        timerBar.gameObject.SetActive(false);
    }

    public void FailCooking()
    {
        // TODO: 태움/버림 처리
    }


    public void UpdateVisualState()
    {
        switch (currentState)
        {
            case FoodState.Raw:
                spriteRenderer.sprite = rawSprite;
                break;
            case FoodState.Undercooked:
                spriteRenderer.sprite = undercookedSprite;
                break;
            case FoodState.Perfect:
                spriteRenderer.sprite = perfectSprite;
                // 잘 익었을 때 특별한 효과/사운드 발생하는 코드 추가 예정
                // SoundManager.Instance.PlaySFX("PerfectCook");
                break;
            case FoodState.Burnt:
                spriteRenderer.sprite = burntSprite;
                // 탔을 때 손님 표정 연동 이벤트 발생 
                // CustomerManager.Instance.NotifyBurntFood();
                break;
        }
    }

    public void AddTopping(ToppingType topping)
    {
        toppings.Add(topping);
    }

    private IEnumerator CookFoodRoutine()
    {
        float elapsedTime = 0f;
        // 전체 조리 시간 계산 (UI 슬라이더의 최대값 기준)
        float totalPerfectTime = undercookedTime + perfectTime;

        // 1단계: Raw -> Undercooked (설익음)
        while (elapsedTime < undercookedTime)
        {
            elapsedTime += Time.deltaTime;
            timerBar.value = elapsedTime / totalPerfectTime; // 슬라이더 업데이트
            yield return null;
        }
        ChangeState(FoodState.Undercooked); // 

        // 2단계: Undercooked -> Perfect (잘 익음)
        while (elapsedTime < totalPerfectTime)
        {
            elapsedTime += Time.deltaTime;
            timerBar.value = elapsedTime / totalPerfectTime;
            yield return null;
        }
        ChangeState(FoodState.Perfect); // 

        // 3단계: Perfect -> Burnt (탐) - 대기 시간
        float timeSpentBurning = 0f;
        while (timeSpentBurning < burntTime)
        {
            timeSpentBurning += Time.deltaTime;
            // 슬라이더가 꽉 찬 상태에서 빨간색으로 깜빡이는 효과 등을 추가할 수 있음
            yield return null;
        }

        // 4단계: 상태 변경 및 실패 처리
        ChangeState(FoodState.Burnt); // 
        FailCooking(); // [cite: 26]

        // 팬 역할 종료 및 파괴
        Destroy(gameObject);
    }
}
