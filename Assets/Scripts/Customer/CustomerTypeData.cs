using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/CustomerType")]
public class CustomerTypeData : ScriptableObject
{
    public string ID;
    public string 타입명;  // 예: "학생", "직장인", "관광객"

    [Header("성향")]
    public float 대기인내도;       // 얼마나 오래 기다려주는가 (기본 대기 시간에 곱해줄 계수 등)
    public float 지갑두께지수;     // 기본 지불 가격(+α) - 추후 팁/보너스용
    public float 평판가중치;       // 만족/불만족이 평판에 주는 영향도

    [Header("추가 선호도")]
    public List<string> 선호메뉴IDs;   // 좋아하는 메뉴 ID들 (RecipeData.ID 기준)
}
