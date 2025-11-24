using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Data/Region")]
public class RegionData : ScriptableObject
{
    [Header("지역 기본 정보")]
    public string 지역ID;
    public string 지역명칭;

    [Header("손님 관련")]
    public int baseCustomerRate;
    public List<string> 인기메뉴IDs;

    [Header("이벤트 관련")]
    public float policeBaseChance;
    public bool HasRival;

    [Header("해금 조건")]
    public int requiredBrandLevel;
}
