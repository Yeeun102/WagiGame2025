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
        if (currentDough == null) return; // 도우가 없으면 못 얹음

        addedToppings.Add(type);

        // 시각적 연출: 도우 위치에 토핑 프리팹 생성
        int index = (int)type - 1;
        if (index >= 0 && index < toppingPrefabs.Length)
        {
            GameObject visualTopping = Instantiate(toppingPrefabs[index], toppingParent);
            visualTopping.transform.localScale = new Vector3(0.35f, 0.7f, 0.6f);
            //Vector3 spawnPos = Vector3.zero;
            visualTopping.transform.localPosition = new Vector3(
                Random.Range(-0.2f, 0.2f),
                Random.Range(-0.2f, 0.2f),
                -1f
            );

            // 스크립트 및 콜라이더 비활성화
            IngredientDrag drag = visualTopping.GetComponent<IngredientDrag>();
            if (drag != null) Destroy(drag);

            Collider2D col = visualTopping.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            // 5. 레이어를 도우(5)보다 높게 설정 (코드로 강제 설정 가능)
            visualTopping.GetComponent<SpriteRenderer>().sortingOrder = 10;
        }
        Debug.Log($"토핑 추가: {type}");
    }
}
