using UnityEngine;

public class RegionManager : MonoBehaviour
{
    public static RegionManager Instance;

    public RegionData CurrentRegion;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void SetRegion(RegionData data)
    {
        CurrentRegion = data;
        // TODO: 지역 변경 시 초기화 처리
    }

    public bool CanUnlockRegion(RegionData data, int brandLevel)
    {
        return brandLevel >= data.requiredBrandLevel;
    }
}
