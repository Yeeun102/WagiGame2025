using UnityEngine;

public class IngredientDrag : MonoBehaviour
{
    public bool isTopping;
    public ToppingType toppingType;
    public SpreadType spreadType;

    private bool isDragging = false;
    private Vector3 offset;

    // 상자에서 생성되자마자 호출될 함수
    public void StartDraggingDirectly()
    {
        isDragging = true;
        // 생성 시점의 마우스 오프셋 계산
        offset = transform.position - GetMouseWorldPos();
    }

    void Update()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPos() + offset;

            // 마우스를 떼면 드롭 판정 후 파괴
            if (Input.GetMouseButtonUp(0))
            {
                CheckDrop();
                Destroy(gameObject); // 도마에 정보를 넘겼으니 이 오브젝트는 삭제
            }
        }
    }

    private void CheckDrop()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.5f);
        if (hit != null)
        {
            Debug.Log($"토핑이 닿은 오브젝트: {hit.name}");
            CuttingBoard board = hit.GetComponent<CuttingBoard>();
            if (board == null)
            {
                board = hit.GetComponentInParent<CuttingBoard>();
            }
            if (board != null && board.currentDough != null)
            {
                if (isTopping) board.AddTopping(toppingType);
                else board.ApplySpread(spreadType);
                Debug.Log("도마(또는 도우 위)에 성공적으로 재료를 전달했습니다.");
            }
            else
            {
                Debug.Log($"감지된 {hit.name}은 도마와 연결되어 있지 않습니다.");
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