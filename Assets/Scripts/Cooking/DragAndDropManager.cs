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

    public bool isOnPan = false;
    public int currentPanIndex = -1; 

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
            isOnPan = false;

            CookingSystem.Instance.StopCookingVisual(currentPanIndex);
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

        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    /*private Vector3 GetMouseWorldPosition()
    {

        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = Camera.main.nearClipPlane; 
        return Camera.main.ScreenToWorldPoint(mouseScreenPosition);
    }*/
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        // UI 환경에서는 카메라와의 거리를 명시적으로 주는 것이 안전합니다.
        // 카메라가 -10, 오브젝트가 0이라면 거리는 10입니다.
        mouseScreenPosition.z = Mathf.Abs(Camera.main.transform.position.z);
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
                board.PlaceDough(this);
                isOnPan = false; // 팬에서 벗어남
                return true;
            }

            // 2. 손님에게 전달하기 (태그 사용)
            if (hit.CompareTag("Customer"))
            {
                DeliverToCustomer(hit.gameObject);
                return true;
            }

            if (hit.CompareTag("TrashCan"))
            {
                DiscardDish();
                return true;
            }

        }
        return false;
    }


    private void DeliverToCustomer(GameObject customer)
    {
        CustomerController cc = customer.GetComponent<CustomerController>();
        if (cc != null)
        {
            CuttingBoard board = Object.FindAnyObjectByType<CuttingBoard>();
            if (board != null)
            {
                // 수정된 매개변수: (토핑 리스트, 스프레드 타입, 조리 상태)
                bool success = cc.ReceiveFood(board.addedToppings, board.currentSpread, this.currentFoodState);

                DiscardDish();

                // 결과에 따른 피드백 (성공/실패 로그는 CustomerController에서 출력됨)
                // 퇴장 처리 (CustomerManager에 exitPoint가 있는지 확인하세요!)
                Vector3 exitPos = CustomerManager.Instance.exitPoint.position;
                cc.Leave(exitPos);
            }
        }

    }

    private void DiscardDish()
    {
        CuttingBoard board = GetComponentInParent<CuttingBoard>();
        if (board != null)
        {
            board.ClearBoard(); // 도마 비우기 (이 안에서 Destroy(gameObject)가 호출됨)
        }
        else
        {
            // 도마 위에 있지 않은 상태(예: 팬에서 바로 버릴 때)라면 자기 자신만 파괴
            Destroy(gameObject);
        }
    }
}