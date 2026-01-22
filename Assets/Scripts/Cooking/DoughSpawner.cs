using UnityEngine;
using UnityEngine.EventSystems;

public class DoughSpawner : MonoBehaviour
{
    public GameObject doughPrefab;
    private Vector3 originalPosition;
    private bool isDragging = false;
    private Vector3 offset;

    [Header("기울기 및 감지 설정")]
    public float pourAngle = -45f;
    public float rotationSpeed = 15f; // 조금 더 빠르게 반응하도록 수정
    public float detectionRadius = 1.0f; // 범위를 더 넓게 잡으세요
    public LayerMask panLayer; // 인스펙터에서 'Cooking' 레이어를 선택하세요

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // [중요] 마우스가 UI 요소(버튼, 패널 등) 위에 있다면 아래 로직을 아예 실행 안 함
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log($"[클릭 성공] 마우스 아래에 있는 것: {hit.collider.gameObject.name}");
                // 여기서 도우 생성 로직 실행...
            }
            else
            {
                // UI가 아닌 허공을 클릭했을 때만 이 로그가 찍힘
                Debug.LogWarning("[클릭 실패] 아무것도 감지되지 않음.");
            }
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
        offset = transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;
        transform.position = GetMouseWorldPos() + offset;

        // [핵심] 팬 레이어를 가진 오브젝트가 반경 내에 있는지 검사
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, panLayer);

        if (hit != null)
        {
            // 팬 발견! 기울이기
            Quaternion target = Quaternion.Euler(0, 0, pourAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * rotationSpeed);
        }
        else
        {
            // 팬 없음! 세우기
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.deltaTime * rotationSpeed);
        }
    }

    void OnMouseUp()
    {
        isDragging = false;

        // 마우스를 뗄 때 레이어 기반으로 다시 확인
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, panLayer);

        if (hit != null)
        {
            FryingPan pan = hit.GetComponent<FryingPan>();
            if (pan != null)
            {
                SpawnDoughOnPan(pan);
            }
        }

        transform.position = originalPosition;
        transform.rotation = Quaternion.identity;
    }

    private void SpawnDoughOnPan(FryingPan pan)
    {
        if (doughPrefab == null) return;

        GameObject newDough = Instantiate(doughPrefab, pan.transform.position, Quaternion.identity);
        DragAndDropManager doughManager = newDough.GetComponent<DragAndDropManager>();

        if (doughManager != null)
        {
            doughManager.isOnPan = true;
            doughManager.currentPanIndex = pan.panIndex;

            newDough.transform.SetParent(pan.transform);
            newDough.transform.localPosition = new Vector3(0, 0, -0.1f);
            if (CookingSystem.Instance != null)
            {
                CookingSystem.Instance.StartCooking(pan.panIndex, doughManager.currentRecipe, doughManager);
            }
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = 10;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    // 감지 범위를 씬 뷰에서 시각적으로 확인 (흰색 원)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}