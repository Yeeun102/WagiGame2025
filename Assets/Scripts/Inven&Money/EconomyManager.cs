using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance;

    [Header("경제 상태")]
    [SerializeField] private int currentMoney = 0;

    [Header("UI 연결")]
    public TMP_Text moneyText;

    private void Awake()
    {
        // 싱글톤 패턴 설정
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
        // [중요] 이제 여기서 이벤트를 구독(+=)하지 않습니다.
        // 그냥 시작하자마자 UI 한 번 갱신해줍니다.
        UpdateMoneyUI();

        Debug.Log("EconomyManager 준비 완료! 요리사가 직접 호출해주길 기다리는 중...");
    }

    // OnDestroy도 이제 구독 해지할 게 없으니 필요 없습니다. 지워도 됩니다.

    /// <summary>
    /// CookingSystem이 직접 호출하는 함수입니다. 반드시 public이어야 합니다.
    /// </summary>
    public void CalculateRevenue(int panIndex, FoodState state, string recipeID)
    {
        int earnedMoney = 0;

        switch (state)
        {
            case FoodState.Perfect:
                earnedMoney = 6000;
                Debug.Log($"정산: 완벽 (+{earnedMoney})");
                break;

            case FoodState.Undercooked:
                earnedMoney = 3000;
                Debug.Log($"정산: 설익음 (+{earnedMoney})");
                break;

            // Burnt, Raw 등은 0원이므로 처리 안 함
            default:
                Debug.Log($"정산: 판매 불가 상태 {state} (0)");
                break;
        }

        if (earnedMoney > 0)
        {
            AddMoney(earnedMoney);
        }
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyUI();
    }

    public bool TrySpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateMoneyUI();
            return true;
        }
        return false;
    }

    // MoneyLinker가 부를 수 있게 public으로 유지
    public void UpdateMoneyUI()
    {
        // 1. 만약 연결이 끊겼다면 이름으로 찾기 시도
        if (moneyText == null)
        {
            GameObject foundObj = GameObject.Find("Money"); // 아까 이름을 Money로 하셨다고 했죠?
            if (foundObj != null)
            {
                moneyText = foundObj.GetComponent<TMP_Text>();
            }
        }

        // 2. 텍스트 갱신
        if (moneyText != null)
        {
            moneyText.text = $"{currentMoney}";
        }
    }

    public int GetCurrentMoney()
    {
        return currentMoney;
    }
}