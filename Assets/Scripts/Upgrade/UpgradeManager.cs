using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [Header("보유 업그레이드")]
    public List<string> ownedUpgradeIDs = new();

    [Header("전체 업그레이드 DB")]
    public List<UpgradeData> allUpgrades;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 구매 가능 여부 체크
    public bool CanBuy(UpgradeData data)
    {
        if (ownedUpgradeIDs.Contains(data.ID))
            return false;

        if (!string.IsNullOrEmpty(data.선행업그레이드ID) &&
            !ownedUpgradeIDs.Contains(data.선행업그레이드ID))
            return false;

        if (GameStateManager.Instance.economyManager.GetCurrentMoney() < data.비용)
            return false;

        // 평판 조건 (나중에 FameManager 붙이면 됨)
        // if (GameStateManager.Instance.fame < data.필요평판) return false;

        return true;
    }

    // 업그레이드 구매
    public void BuyUpgrade(UpgradeData data)
    {
        if (!CanBuy(data))
        {
            Debug.Log("업그레이드 구매 불가: " + data.ID);
            return;
        }

        // 돈 차감
        GameStateManager.Instance.economyManager.TrySpendMoney(data.비용);

        // 보유 처리
        ownedUpgradeIDs.Add(data.ID);

        // 효과 적용
        ApplyUpgradeEffect(data);

        Debug.Log("업그레이드 구매: " + data.업그레이드명);
    }

    // 실제 효과 적용 (여기서만 GameState 건드림)
    void ApplyUpgradeEffect(UpgradeData data)
    {
        // 재고
        if (data.최대재고증가 > 0)
        {
            // InventorySystem에 나중에 MaxCapacity 변수 붙이면 됨
            Debug.Log("재고 증가 +" + data.최대재고증가);
        }

        // 조리 관련
        if (data.조리대증가 > 0)
        {
            Debug.Log("조리대 +" + data.조리대증가);
        }

        if (data.조리속도증가퍼센트 > 0)
        {
            Debug.Log("조리 속도 +" + data.조리속도증가퍼센트);
        }

        // 직원
        if (data.직원수증가 > 0)
        {
            Debug.Log("직원 수 +" + data.직원수증가);
        }

        // 만족도
        if (data.기본만족도보정 > 0)
        {
            Debug.Log("기본 만족도 +" + data.기본만족도보정);
        }
    }
}
