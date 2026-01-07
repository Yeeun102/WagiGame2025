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
    [Header("기본 정보")]
    public string eventID;
    public EventType eventType;

    [TextArea]
    public string description;

    [Header("확률")]
    public float baseChance;

    [Header("효과")]
    public int moneyChange;
    public float fameChange;
}
