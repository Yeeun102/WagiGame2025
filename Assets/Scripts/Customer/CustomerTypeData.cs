using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Data/CustomerType")]
public class CustomerTypeData : ScriptableObject
{
    [Header("기본 정보")]
    public string customerTypeID;
    public string displayName;

    [Header("성향")]
    public float patience;        // 대기 인내도
    public float payMultiplier;   // 지불 성향
    public float fameWeight;      // 평판 영향도

    [Header("선호 메뉴")]
    public List<string> favoriteRecipeIDs;
}
