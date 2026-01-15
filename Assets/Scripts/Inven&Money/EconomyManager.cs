using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance;

    [Header("경제 상태")]
    [SerializeField] private int currentMoney = 0;

    private void Awake()
    {
        // 싱글톤 패턴: 어디서든 EconomyManager.Instance로 접근 가능
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // CookingSystem이 존재한다면 조리 완료 이벤트를 구독(연결)합니다.
        if (CookingSystem.Instance != null)
        {
            CookingSystem.Instance.OnCookingCompleted += CalculateRevenue;
        }
        else
        {
            Debug.LogWarning("EconomyManager: CookingSystem을 찾을 수 없습니다.");
        }
    }

    private void OnDestroy()
    {
        // 매니저가 파괴될 때 구독을 해지해야 에러를 방지할 수 있습니다.
        if (CookingSystem.Instance != null)
        {
            CookingSystem.Instance.OnCookingCompleted -= CalculateRevenue;
        }
    }

    /// <summary>
    /// CookingSystem에서 조리가 끝날 때마다 자동으로 호출되는 함수입니다.
    /// </summary>
    private void CalculateRevenue(int panIndex, FoodState state, string recipeID)
    {
        int earnedMoney = 0;

        // TODO: 나중에는 recipeID를 통해 RecipeData에서 기본 가격을 가져오세요.
        int basePrice = 1000;

        switch (state)
        {
            case FoodState.Perfect:
                // 완벽함: 1.5배 가격
                earnedMoney = (int)(basePrice * 1.5f);
                Debug.Log($"[정산] 완벽한 조리! (+{earnedMoney}원)");
                break;

            case FoodState.Undercooked:
                // 설익음: 50% 가격
                earnedMoney = (int)(basePrice * 0.5f);
                Debug.Log($"[정산] 설익었습니다. (+{earnedMoney}원)");
                break;

            case FoodState.Burnt:
                // 탔음: 0원
                earnedMoney = 0;
                Debug.Log($"[정산] 음식이 탔습니다. (0원)");
                break;

            case FoodState.Raw:
                // 생재료: 0원
                earnedMoney = 0;
                Debug.Log($"[정산] 생재료 상태입니다. (0원)");
                break;

            default:
                Debug.Log("[정산] 알 수 없는 상태입니다.");
                break;
        }

        if (earnedMoney > 0)
        {
            AddMoney(earnedMoney);
        }
    }

    /// <summary>
    /// 돈을 추가하고 로그를 출력합니다.
    /// </summary>
    public void AddMoney(int amount)
    {
        currentMoney += amount;

        // TODO: 여기서 UI 업데이트 함수 호출 (예: UIManager.Instance.UpdateMoneyUI)
        Debug.Log($"[System] 정산 완료! 현재 소지금: {currentMoney}원");
    }

    /// <summary>
    /// 돈을 사용하려고 시도합니다. 잔액이 충분하면 차감하고 true를 반환합니다.
    /// </summary>
    /// <param name="amount">사용할 금액</param>
    /// <returns>성공 시 true, 실패 시 false</returns>
    public bool TrySpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            Debug.Log($"[지출] {amount}원 사용했습니다. (남은 돈: {currentMoney}원)");

            // TODO: 여기서도 UI 갱신 함수를 호출해야 합니다.
            // UIManager.Instance.UpdateMoneyUI(currentMoney);

            return true; // 구매 성공
        }
        else
        {
            Debug.Log($"[지출 실패] 잔액이 부족합니다. (필요: {amount}원, 보유: {currentMoney}원)");
            return false; // 구매 실패
        }
    }

    /// <summary>
    /// 현재 소지금을 반환합니다.
    /// </summary>
    public int GetCurrentMoney()
    {
        return currentMoney;
    }
}