using UnityEngine;
using System.Collections.Generic;

public class CuttingBoard : MonoBehaviour
{
    public Transform anchorPoint; // 도우가 붙을 중심점
    public DragAndDropManager currentDough;

    [Header("스프레드 설정")]
    public GameObject[] spreadPrefabs;
    private GameObject currentSpreadObject; // 0: 생크림, 1: 치즈크림, 2: 초콜릿

    [Header("토핑 설정")]
    public Transform toppingParent; // 토핑들이 생성될 부모 오브젝트
    public GameObject[] toppingPrefabs;

    //public List<string> addedToppings = new List<string>();

    public List<ToppingType> addedToppings = new List<ToppingType>();
    public SpreadType currentSpread = SpreadType.None;

    public void PlaceDough(DragAndDropManager dough)
    {
        currentDough = dough;

        // 2. 부모 설정 및 좌표 강제 고정
        dough.transform.SetParent(this.transform, false);
        dough.transform.localPosition = new Vector3(0, 0, -0.1f);

        Debug.Log("고정 완료.");
    }



    // 1. 스프레드 바르기
    public void ApplySpread(SpreadType type)
    {
        if (currentDough == null) return; // 도우가 없으면 못 바름

        // 이미 스프레드가 발라져 있다면 기존 것은 삭제 (중복 방지)
        if (currentSpreadObject != null)
        {
            Destroy(currentSpreadObject);
        }

        currentSpread = type;

        // Enum 순서에 맞춰 프리팹 생성 (None이 0이므로 type-1)
        int index = (int)type - 1;

        if (index >= 0 && index < spreadPrefabs.Length)
        {
            // 1. 스프레드 프리팹 생성 (도우의 자식으로 넣으면 도우와 함께 움직입니다)
            currentSpreadObject = Instantiate(spreadPrefabs[index], currentDough.transform);

            // 2. 위치 및 크기 초기화
            currentSpreadObject.transform.localPosition = new Vector3(0, 0, -0.01f); // 도우보다 살짝 앞
            currentSpreadObject.transform.localScale = Vector3.one;

            // 3. 레이어 설정 (도우보다 높고 토핑보다 낮게)
            SpriteRenderer sr = currentSpreadObject.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = 5;
            }

            // 4. 스패츌라로 바른 것이므로 클릭 방해 안 되게 콜라이더 제거
            Collider2D col = currentSpreadObject.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            Debug.Log($"스프레드 프리팹 적용 완료: {type}");
        }
    }

    // 2. 토핑 얹기
    public void AddTopping(ToppingType type)
    {
        if (currentDough == null) return;
        addedToppings.Add(type);

        int index = (int)type - 1;
        if (index >= 0 && index < toppingPrefabs.Length)
        {
            // [수정] 부모를 도마(toppingParent)가 아닌 도우(currentDough)로 설정합니다.
            GameObject visualTopping = Instantiate(toppingPrefabs[index], currentDough.transform);

            // 부모가 도우이므로 도우의 중심(0,0,0)을 기준으로 랜덤 위치 설정
            visualTopping.transform.localPosition = new Vector3(
                Random.Range(-0.4f, 0.4f),
                Random.Range(-0.4f, 0.4f),
                -0.1f // 도우보다 앞, 스프레드(-0.01)보다도 앞
            );

            visualTopping.transform.localScale = new Vector3(1.4f,1.4f,1.4f);

            // 드래그 방해 요소 제거
            Destroy(visualTopping.GetComponent<Collider2D>());
            Destroy(visualTopping.GetComponent<IngredientDrag>());

            // 레이어 설정
            SpriteRenderer sr = visualTopping.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sortingOrder = 10;

            Debug.Log($"토핑 {type}이 도우에 귀속되어 생성되었습니다.");
        }
    }
}
