using UnityEngine;

public class DoughSpawner : MonoBehaviour
{
    public GameObject doughPrefab; // 방금 만든 반죽 프리팹
    public Transform spawnPoint;   // 반죽이 생성될 위치 (반죽 통 위)

    private void OnMouseDown()
    {
        // 클릭 시 새로운 반죽 생성
        GameObject newDough = Instantiate(doughPrefab, transform.position, Quaternion.identity);

        // 생성되자마자 드래그 상태로 만들고 싶다면 추가 로직이 필요할 수 있습니다.
        Debug.Log("새 반죽을 꺼냈습니다.");
    }
}