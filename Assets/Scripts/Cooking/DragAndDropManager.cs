using UnityEngine;

public class FryingPan : MonoBehaviour 
{
    //코드 작성 예정 
}
public class DragAndDropManager : MonoBehaviour
{
    private Vector3 originalPosition;
    private Vector3 offset;

    private bool isDragging = false;

    private string currentRecipe;
    private CookingSystem cookingSystem;
    private GameObject currentObject;
    private Collider2D foodCollider;

    void Awake()
    {
        // CookingSystem 싱글톤 참조
        if (CookingSystem.Instance != null)
        {
            cookingSystem = CookingSystem.Instance;
        }
        // 이 오브젝트가 어떤 레시피의 재료인지 초기화하는 로직이 필요
        // 현재는 하드코딩하거나 외부에서 설정해야 함
        currentRecipe = "Recipe"; // 실제 구현 시 외부에서 설정 필요

        currentObject = gameObject;
        foodCollider = GetComponent<Collider2D>();
    }

    void OnMouseDown()
    {
        originalPosition = transform.position;

        offset = transform.position - GetMouseWorldPosition();

        isDragging = true;

        transform.position = new Vector3(transform.position.x, transform.position.y, -0.1f);
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
            // 1. 팬 오브젝트를 찾습니다. (FryingPan이 MonoBehaviour를 상속받았다고 가정)
            FryingPan fryingPan = hit.GetComponent<FryingPan>();

            // FryingPan이 빈 클래스이므로, 임시로 태그로 검사할 수도 있습니다.
            // if (hit.CompareTag("FryingPan")) 

            if (fryingPan != null)
            {
                // 이 컴포넌트가 붙은 오브젝트(음식)가 콜라이더를 가지고 있는지 확인
                if (foodCollider != null)
                {
                    // 2. 조리 시작 요청 (이 로직은 현재 CookingSystem 안에 팬 로직이 통합되었기에 CookingSystem을 직접 호출합니다.)
                    if (cookingSystem != null)
                    {
                        // CookingSystem이 팬 오브젝트에 붙어있는 경우:
                        if (cookingSystem.gameObject == hit.gameObject)
                        {
                            cookingSystem.InitializePan(null); // 레시피 데이터 전달 (null 대신 실제 RecipeData 필요)
                            cookingSystem.StartCooking(currentRecipe); // 조리 시작

                            // 음식 오브젝트를 팬의 자식으로 만들고 위치를 팬 중앙으로 고정
                            transform.SetParent(hit.transform);
                            transform.position = hit.transform.position;

                            // 드래그 컴포넌트 비활성화 (조리 중에는 드래그 불가)
                            enabled = false;
                            return true;
                        }
                    }
                }
            }

            // TODO: 도마에 드롭, 손님에게 드롭 등의 상호작용 로직을 여기에 추가
        }
        return false;
    }
}