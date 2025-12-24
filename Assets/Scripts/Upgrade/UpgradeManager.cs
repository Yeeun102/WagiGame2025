using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public List<string> OwnedUpgrades = new();

    public int 추가조리대수 = 0;
    public float 조리속도배율 = 1f;
    public int 추가최대재고 = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ApplyUpgrade(string upgradeID)
    {
        if (OwnedUpgrades.Contains(upgradeID))
        {
            Debug.Log("이미 해당 업그레이드 단계에 도달해 있습니다.");
            return;
        }

        // 존재 여부만 확인
        UpgradeData data = Resources.Load<UpgradeData>($"UpgradeData/{upgradeID}");

        if (data == null)
        {
            Debug.LogError($"UpgradeData를 찾을 수 없습니다: {upgradeID}");
            return;
        }

        OwnedUpgrades.Add(upgradeID);

        추가조리대수 += data.조리대증가;
        추가최대재고 += data.최대재고증가;
        조리속도배율 *= (1f + data.조리속도증가퍼센트);

        Debug.Log($"업그레이드 적용 완료: {data.업그레이드명}");
    }
}