using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void TryPoliceEvent()
    {
        // TODO: 경찰 단속 발생 확률 체크
    }

    public void TriggerRivalEvent()
    {
        // TODO: 라이벌 셰프 방해
    }
}
