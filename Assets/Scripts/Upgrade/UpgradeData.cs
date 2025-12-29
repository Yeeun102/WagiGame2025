using UnityEngine;

public enum UpgradeCategory
{
    Truck,
    Kitchen,
    Staff,
    Marketing
}

[CreateAssetMenu(menuName = "Data/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string ID;
    public string 업그레이드명;
    public UpgradeCategory 카테고리;

    [TextArea]
    public string 설명;

    [Header("비용")]
    public int 비용;
    public int 필요평판; // 없으면 0

    [Header("효과")]
    public int 조리대증가;
    public float 조리속도증가퍼센트;
    public int 최대재고증가;
    public int 직원수증가;
    public int 기본만족도보정;

    [Header("선행 업그레이드")]
    public string 선행업그레이드ID; // 없으면 ""
}
