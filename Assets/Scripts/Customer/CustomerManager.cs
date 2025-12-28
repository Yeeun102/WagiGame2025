using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance;

    [Header("Spawn")]
    public GameObject customerPrefab;
    public Transform spawnPoint;
    public float spawnInterval = 3f;

    [Header("Day")]
    [Range(1, 5)] public int currentDay = 1;     // DAY1~5
    public int customersPerDay = 5;              // 하루 5명

    [Header("Customer Types")]
    public List<CustomerTypeData> possibleCustomerTypes = new List<CustomerTypeData>();

    [Header("Menus by Day (누적 풀)")]
    public List<RecipeData> day1Menus = new List<RecipeData>();
    public List<RecipeData> day2Menus = new List<RecipeData>();
    public List<RecipeData> day3Menus = new List<RecipeData>();
    public List<RecipeData> day4Menus = new List<RecipeData>();
    public List<RecipeData> day5Menus = new List<RecipeData>();

    // 내부 카운트
    private int spawnedCount = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SpawnCustomer()
    {
        // TODO: 손님 프리팹 스폰
        if (customerPrefab == null || spawnPoint == null)
        {
            Debug.LogWarning("[CustomerManager] customerPrefab 또는 spawnPoint 미설정");
            return;
        }

        GameObject obj = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);
        CustomerController controller = obj.GetComponent<CustomerController>();

        if (controller == null)
        {
            Debug.LogWarning("[CustomerManager] CustomerPrefab에 CustomerController가 없습니다.");
            Destroy(obj);
            return;
        }

        // 1) 손님 타입 랜덤
        CustomerTypeData randomType = null;
        if (possibleCustomerTypes != null && possibleCustomerTypes.Count > 0)
        {
            int tIdx = Random.Range(0, possibleCustomerTypes.Count);
            randomType = possibleCustomerTypes[tIdx];
        }

        // 2) DAY 누적 메뉴 풀 구성
        List<RecipeData> pool = new List<RecipeData>();
        if (currentDay >= 1) pool.AddRange(day1Menus);
        if (currentDay >= 2) pool.AddRange(day2Menus);
        if (currentDay >= 3) pool.AddRange(day3Menus);
        if (currentDay >= 4) pool.AddRange(day4Menus);
        if (currentDay >= 5) pool.AddRange(day5Menus);

        // 3) 오늘 주문 레시피 랜덤
        RecipeData randomRecipe = null;
        if (pool.Count > 0)
        {
            int rIdx = Random.Range(0, pool.Count);
            randomRecipe = pool[rIdx];
        }

        // 4) CustomerController 필드 세팅 (함수 추가 없이 필드로만 연결)
        controller.주문레시피ID = (randomRecipe != null) ? randomRecipe.ID : "";
        controller.대기인내도 = (randomType != null) ? randomType.대기인내도 : controller.대기인내도;

        // 토핑은 CustomerController 쪽에서 CheckOrderMatch가 필드로 비교하니까
        // 여기서는 "레시피ID에서 토핑 추정"을 아주 간단히 세팅(원하면 나중에 바꿔도 됨)
        controller.주문토핑.Clear();
        string id = controller.주문레시피ID.ToLowerInvariant();
        if (id.Contains("strawberry") || id.Contains("딸기")) controller.주문토핑.Add(ToppingType.Strawberry);
        if (id.Contains("blueberry") || id.Contains("블루베리")) controller.주문토핑.Add(ToppingType.Blueberry);
        if (id.Contains("choco") || id.Contains("chocolate") || id.Contains("초코")) controller.주문토핑.Add(ToppingType.Chocolate);
        if (id.Contains("creamcheese") || id.Contains("크림치즈")) controller.주문토핑.Add(ToppingType.CreamCheese);
        if (id.Contains("cream") || id.Contains("생크림")) controller.주문토핑.Add(ToppingType.Cream);

        controller.Enter();
    }

    public void StartCustomerFlow()
    {
        // TODO: 손님 생성 루프 시작
        StopAllCoroutines();
        spawnedCount = 0;

        // 함수 추가 없이: 로컬 코루틴 사용
        IEnumerator SpawnLoop()
        {
            while (spawnedCount < customersPerDay)
            {
                SpawnCustomer();
                spawnedCount++;
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        StartCoroutine(SpawnLoop());
    }
}
