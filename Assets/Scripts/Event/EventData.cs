using UnityEngine;

public enum EventType
{
    Police,
    Rival,
    Special
}

[CreateAssetMenu(menuName = "Data/Event")]
public class EventData : ScriptableObject
{
    public string ID;
    public EventType 타입;

    [Header("발생 확률(지역 보정 전에)")]
    public float 기본확률;

    [Header("이벤트 설명")]
    public string 설명;

    [Header("효과 값")]
    public int 벌금액;
    public float 평판변화;
    public int 보너스매출;
}
