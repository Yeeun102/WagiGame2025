using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public string ID;
    public string 재료명;

    [Header("구매 정보")]
    public int 구매가격;

    [Header("보관")]
    public int 최대보관수량;

    [Header("설명")]
    public string 설명;
}
