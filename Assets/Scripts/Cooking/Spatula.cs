using UnityEngine;

public class Spatula : MonoBehaviour
{
    public SpreadType spatulaType; // 이 스패츌라가 담당하는 스프레드 종류
    private Vector3 originalPosition;
    private bool isDragging = false;
    private Vector3 offset;

    [Header("시각 효과")]
    public GameObject spreadEffectPrefab; // 칠할 때 나올 이펙트 (선택사항)

    void Start() => originalPosition = transform.position;

    void OnMouseDown()
    {
        isDragging = true;
        offset = transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        if (isDragging) transform.position = GetMouseWorldPos() + offset;
    }

    void OnMouseUp()
    {
        isDragging = false;
        CheckSpreadApply();
        transform.position = originalPosition; // 사용 후 제자리로
    }

    private void CheckSpreadApply()
    {
        // 반지름 0.5 이내에 무엇이 있는지 확인
        Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.5f);

        if (hit != null)
        {
            // 도우 또는 도마 확인
            CuttingBoard board = hit.GetComponent<CuttingBoard>();
            if (board == null) board = hit.GetComponentInParent<CuttingBoard>();

            if (board != null && board.currentDough != null)
            {
                // 도마에 스프레드 적용 명령!
                board.ApplySpread(spatulaType);
                Debug.Log($"{spatulaType} 스프레드를 발랐습니다.");
            }
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = 10;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}