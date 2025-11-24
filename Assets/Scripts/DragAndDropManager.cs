using UnityEngine;

public class DragAndDropManager : MonoBehaviour
{
    private Vector3 originalPosition;
    private Vector3 offset;

    private bool isDragging = false;

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
            FryingPan fryingPan = hit.GetComponent<FryingPan>();
            if (fryingPan != null)
            {
                CrepeItem crepeItem = GetComponent<CrepeItem>();
                if (crepeItem != null)
                {
                    fryingPan.StartCooking(crepeItem);
                    transform.position = fryingPan.transform.position; 
                    return true;
                }
            }
        }
        return false;
    }
}