using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public List<string> OwnedUpgrades = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ApplyUpgrade(string upgradeID)
    {
        OwnedUpgrades.Add(upgradeID);
        // TODO: 업그레이드 효과 적용
    }
}
