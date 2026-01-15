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
            // 1. �ش� ���� ������ �Ͻ� �ߴ��ϰų� �ʱ�ȭ�ؾ� ��
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

            // 1. ������ �ø���
            CuttingBoard board = hit.GetComponent<CuttingBoard>();
            if (board != null)
            {
                transform.position = hit.transform.position;
                transform.SetParent(hit.transform);
                board.PlaceDough(this);
                isOnPan = false; // �ҿ��� ���
                return true;
            }

            // 2. �մԿ��� �����ϱ� (�±� ���)
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
                // ������ �Ű�����: (���� ����Ʈ, �������� Ÿ��, ���� ����)
                bool success = cc.ReceiveFood(board.addedToppings, board.currentSpread, this.currentFoodState);

                DiscardDish();

                // ����� ���� �ǵ�� (����/���� �α״� CustomerController���� ��µ�)
                // ���� ó�� (CustomerManager�� exitPoint�� �ִ��� Ȯ���ϼ���!)
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
            board.ClearBoard(); // ���� ���� (�� �ȿ��� Destroy(gameObject)�� ȣ���)
        }
        else
        {
            // ���� ���� ���� ���� ����(��: �ҿ��� �ٷ� ���� ��)��� �ڱ� �ڽŸ� �ı�
            Destroy(gameObject);
        }
    }
}