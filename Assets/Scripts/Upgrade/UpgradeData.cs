using UnityEngine;

[CreateAssetMenu(menuName = "Data/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string ID;
    public string 업그레이드명;
    public string 설명;

    [Header("가격")]
    public int 비용;

    [Header("효과")]
    public int 조리대증가;
    public float 조리속도증가퍼센트;
    public int 최대재고증가;
}
