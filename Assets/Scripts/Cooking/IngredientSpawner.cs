using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    public GameObject ingredientPrefab; // 복사해서 꺼낼 토핑 프리팹

    private void OnMouseDown()
    {
        // 1. 마우스 위치에 토핑 생성
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        GameObject newIngredient = Instantiate(ingredientPrefab, mousePos, Quaternion.identity);

        // 2. 생성된 토핑의 드래그 스크립트를 찾아 즉시 드래그 상태로 만듦
        IngredientDrag dragScript = newIngredient.GetComponent<IngredientDrag>();
        if (dragScript != null)
        {
            dragScript.StartDraggingDirectly();
        }
    }
}
