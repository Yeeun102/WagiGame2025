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

    [Header("�⺻ ����")]
    public string eventID;
    public EventType eventType;

    [TextArea]
    public string description;

    [Header("Ȯ��")]
    public float baseChance;

    [Header("ȿ��")]
    public int moneyChange;
    public float fameChange;

}
