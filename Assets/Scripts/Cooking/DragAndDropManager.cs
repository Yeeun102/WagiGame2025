using UnityEngine;

public class DragAndDropManager : MonoBehaviour
{
    private Vector3 originalPosition;
    private Vector3 offset;

    private bool isDragging = false;

    public string currentRecipe = "Crepe_Basic";
    private CookingSystem cookingSystem;
    private GameObject currentObject;
    private Collider2D foodCollider;

    private bool isOnPan = false;
    private int currentPanIndex = -1;

    public FoodState currentFoodState;

    void OnMouseDown()
    {
        isDragging = true;
        offset = (Vector2)transform.position - (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 만약 팬 위에서 다시 집어 올리는 거라면
        if (isOnPan)
        {
            // 1. 해당 팬의 조리를 일시 중단하거나 초기화해야 함
            //CookingSystem.Instance.StopCookingManually(currentPanIndex);

            // 2. 팬과의 부모 관계 해제 (다시 자유로운 몸)
            transform.SetParent(null);
            //isOnPan = false;

            CookingSystem.Instance.StopCookingVisual(currentPanIndex);
            isOnPan = false;
        }
        //GetComponent<SpriteRenderer>().sortingOrder = 10;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 newPosition = GetMouseWorldPosition() + offset;

            newPosition.z = transform.position.z;

            transform.position = newPosition;
        }
    }

    void OnMouseUp()
    {
        if (!isDragging) return;

        isDragging = false;

        bool actionTaken = HandleDropInteraction();

        if (!actionTaken)
        {
            transform.position = originalPosition;
        }

        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }

    private Vector3 GetMouseWorldPosition()
    {

        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = Camera.main.nearClipPlane; 
        return Camera.main.ScreenToWorldPoint(mouseScreenPosition);
    }
    

    private bool HandleDropInteraction()
    {
        Vector2 dropPoint = transform.position;
        Collider2D[] hitColliders = Physics2D.OverlapPointAll(dropPoint);

        foreach (Collider2D hit in hitColliders)
        {
            
            FryingPan fryingPan = hit.GetComponent<FryingPan>();
            if (fryingPan != null)
            {
                transform.position = hit.transform.position;
                transform.SetParent(hit.transform);

                // [수정] 세 번째 인자로 'this'를 보냅니다.
                CookingSystem.Instance.StartCooking(fryingPan.panIndex, currentRecipe, this);

                isOnPan = true;
                currentPanIndex = fryingPan.panIndex;
                return true;
            }
           
            // 1. 도마에 올리기
            CuttingBoard board = hit.GetComponent<CuttingBoard>();
            if (board != null)
            {
                transform.position = hit.transform.position;
                transform.SetParent(hit.transform);
                //board.PlaceDough(this);
                isOnPan = false; // 팬에서 벗어남
                return true;
            }

            // 2. 손님에게 전달하기 (태그 사용)
            if (hit.CompareTag("Customer"))
            {
                DeliverToCustomer(hit.gameObject);
                return true;
            }
            
        }
        return false;
    }


    private void DeliverToCustomer(GameObject customer)
    {
        // 도마에 있는 토핑 리스트 등을 가져와서 최종 점수 계산 가능
        Debug.Log("손님에게 배달! 상태: " + this.currentFoodState);

        // 돈 계산 로직 (예시)
        // EconomyManager.Instance.AddMoney(currentFoodState, addedToppings);

        Destroy(gameObject); // 배달 완료 후 파괴
    }
}