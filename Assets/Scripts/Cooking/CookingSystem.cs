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

        if (!recipeDictionary.ContainsKey(recipeID))
        {
            Debug.LogError($"레시피 ID '{recipeID}'를 찾을 수 없습니다.");
            return;
        }
        currentRecipe = recipeDictionary[recipeID];
        // 상태 초기화
        ChangeState(FoodState.Raw);
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
        if (cookingCoroutine != null)
        {
            StopCoroutine(cookingCoroutine);
        }
        timerBar.gameObject.SetActive(false);

        // 실패 사운드 및 이펙트 (구현 예정)
        // SoundManager.Instance.PlaySFX("FailCook");

        // 크레페/음식 오브젝트 파괴 (currentFood 변수가 음식 오브젝트를 참조해야 함)
        if (currentFood != null)
        {
            Destroy(currentFood);
            currentFood = null;
        }

        // 현재 조리대 오브젝트(이 스크립트가 붙어있는 오브젝트) 파괴 
        Destroy(gameObject);
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
        float timeToPerfect = currentRecipe.조리시간;
        float elapsedTime = 0f;

        while (elapsedTime < timeToPerfect)
        {
            elapsedTime += Time.deltaTime;
            timerBar.value = elapsedTime / timeToPerfect;

            yield return null;
        }

        ChangeState(FoodState.Perfect);

        float timeToBurnt = timeToPerfect * 0.5f;
        elapsedTime = 0f;

        while (elapsedTime < timeToBurnt)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ChangeState(FoodState.Burnt);
        CookingSystem.Instance.FailCooking();

        // 팬 역할 종료
        Destroy(gameObject);
    }
}
