using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance;

    [Header("설정")]
    public GameObject[] customerPrefabs; // 여러 종류의 손님 프리팹
    public Transform spawnPoint;         // 손님이 나타날 위치
    public Transform orderPoint;         // 손님이 주문하러 서는 위치 (카운터 앞)
    public Transform exitPoint;
    public float spawnInterval = 20f;    // 손님 생성 간격

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartCustomerFlow();
    }

    public void StartCustomerFlow()
    {
        // TODO: 손님 프리팹 스폰
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnCustomer();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void SpawnCustomer()
    {
        if (customerPrefabs.Length == 0) return;

        // 랜덤하게 손님 선택 및 생성
        int randomIndex = Random.Range(0, customerPrefabs.Length);
        GameObject customerObj = Instantiate(customerPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);

        // 생성된 손님에게 목표 지점 전달 및 이동 명령
        CustomerController controller = customerObj.GetComponent<CustomerController>();
        if (controller != null)
        {
            controller.Enter(orderPoint.position);
        }
    }
}