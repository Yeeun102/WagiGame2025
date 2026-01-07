using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Data/Region")]
public class RegionData : ScriptableObject
{
    [Header("기본 정보")]
    public string regionID;          // 내부 식별용 ID
    public string regionName;        // UI 표시 이름

    [Header("손님 관련")]
    public int baseCustomerRate;     // 하루 기본 손님 수
    public List<string> allowedCustomerTypeIDs; // 등장 가능한 손님 타입 ID

    [Header("이벤트 관련")]
    public float policeBaseChance;   // 경찰 단속 기본 확률
    public bool hasRival;            // 라이벌 존재 여부

    [Header("해금 조건")]
    public int requiredBrandLevel;   // 브랜드 레벨 제한
}
