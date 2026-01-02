using UnityEngine;
using System.Collections.Generic;

public class CuttingBoard : MonoBehaviour
{
    public Transform anchorPoint; // 도우가 붙을 중심점
    public DragAndDropManager currentDough;

    [Header("스프레드 설정")]
    public SpriteRenderer spreadRenderer; // 도우 위에 겹쳐질 SpriteRenderer
    public Sprite[] spreadSprites; // 0: 생크림, 1: 치즈크림, 2: 초콜릿

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
        dough.transform.localPosition = (anchorPoint != null) ? anchorPoint.localPosition : Vector3.zero;

        Debug.Log("고정 완료.");
    }



    // 1. 스프레드 바르기
    public void ApplySpread(SpreadType type)
    {
        if (currentDough == null) return; // 도우가 없으면 못 바름

        currentSpread = type;
        // Enum 순서에 맞춰 스프라이트 변경 (예: 1번이 생크림)
        int index = (int)type - 1;
        if (index >= 0 && index < spreadSprites.Length)
        {
            spreadRenderer.sprite = spreadSprites[index];
            spreadRenderer.gameObject.SetActive(true);
        }
        Debug.Log($"스프레드 적용: {type}");
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
            GameObject visualTopping = Instantiate(toppingPrefabs[index], this.transform, false);

            Vector3 spawnPos = (anchorPoint != null) ? anchorPoint.localPosition : Vector3.zero;
            visualTopping.transform.localPosition = new Vector3(
                spawnPos.x + Random.Range(-0.2f, 0.2f),
                spawnPos.y + Random.Range(-0.2f, 0.2f),
                -0.2f
            );

            // 스크립트 및 콜라이더 비활성화
            IngredientDrag drag = visualTopping.GetComponent<IngredientDrag>();
            if (drag != null) Destroy(drag);

            Collider2D col = visualTopping.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            // 5. 레이어를 도우(5)보다 높게 설정 (코드로 강제 설정 가능)
            //visualTopping.GetComponent<SpriteRenderer>().sortingOrder = 10;
        }
        Debug.Log($"토핑 추가: {type}");
    }
}
