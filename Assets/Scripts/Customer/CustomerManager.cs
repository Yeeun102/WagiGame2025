using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SpawnCustomer()
    {
        // TODO: 손님 프리팹 스폰
    }

    public void StartCustomerFlow()
    {
        // TODO: 손님 생성 루프 시작
    }
}
