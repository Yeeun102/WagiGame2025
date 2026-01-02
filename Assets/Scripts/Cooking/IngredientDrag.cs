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
        Collider2D hit = Physics2D.OverlapPoint(transform.position);
        if (hit != null)
        {
            CuttingBoard board = hit.GetComponent<CuttingBoard>();
            if (board != null && board.currentDough != null)
            {
                if (isTopping) board.AddTopping(toppingType);
                else board.ApplySpread(spreadType);
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