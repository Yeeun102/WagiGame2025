using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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

    public List<ToppingType> addedToppings = new List<ToppingType>();
    public SpreadType currentSpread = SpreadType.None;

    public Sprite[] finishedCrepeSprites;
    public int completionToppingCount = 3;

    [Header("토핑 배치 설정")]
    // 토핑 4개가 놓일 고정 좌표 (도우 중심 기준)
    private Vector3[] toppingPositions = new Vector3[]
    {
        new Vector3(-0.6f,  0.6f, -0.1f), // 1번째: 왼쪽 위
        new Vector3( 0f, 0.6f, -0.1f),
        new Vector3( 0.6f,  0.6f, -0.1f) // 3번째: 오른쪽 위
    };

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
        if (IsFinished()) return;

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
        CheckCompletion();
    }

    // 2. 토핑 얹기
    public void AddTopping(ToppingType type)
    {
        if (currentDough == null) return;

        if (IsFinished()) return;

        if (addedToppings.Count >= completionToppingCount) return;

        addedToppings.Add(type);

        int index = (int)type - 1;
        if (index >= 0 && index < toppingPrefabs.Length)
        {
            // [수정] 부모를 도마(toppingParent)가 아닌 도우(currentDough)로 설정합니다.
            GameObject visualTopping = Instantiate(toppingPrefabs[index], currentDough.transform);

            int posIndex = addedToppings.Count - 1;
            if (posIndex < toppingPositions.Length)
            {
                visualTopping.transform.localPosition = toppingPositions[posIndex];
            }

            visualTopping.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);

            // 드래그 방해 요소 제거
            Destroy(visualTopping.GetComponent<Collider2D>());
            Destroy(visualTopping.GetComponent<IngredientDrag>());

            // 레이어 설정
            SpriteRenderer sr = visualTopping.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sortingOrder = 10;

            CheckCompletion();
        }
    }

    private void CheckCompletion()
    {
        // 조건: 스프레드가 발려 있고 && 토핑이 3개이며 && 모든 토핑이 같은 종류일 때
        if (currentSpread != SpreadType.None &&
            addedToppings.Count == completionToppingCount &&
            CheckIfAllToppingsSame())
        {
            FinishCrepe(addedToppings[0]);
        }
    }

    // 이미 완성되었는지 확인하는 도우미 함수
    private bool IsFinished()
    {
        // 토핑 3개와 스프레드가 모두 있으면 이미 완성된 것으로 간주
        return currentSpread != SpreadType.None && addedToppings.Count >= completionToppingCount;
    }

    private bool CheckIfAllToppingsSame()
    {
        if (addedToppings.Count == 0) return false;

        ToppingType firstTopping = addedToppings[0];
        foreach (ToppingType t in addedToppings)
        {
            if (t != firstTopping) return false;
        }
        return true;
    }

    private void FinishCrepe(ToppingType finalToppingType)
    {
        // 1. 도우의 SpriteRenderer를 가져와서 완성된 이미지로 교체
        SpriteRenderer doughSR = currentDough.GetComponent<SpriteRenderer>();
        int spriteIndex = (int)finalToppingType - 1;
        Vector3 finishedScale = new Vector3(1.2f, 1.2f, 1.2f);

        if (doughSR != null && spriteIndex >= 0 && spriteIndex < finishedCrepeSprites.Length)
        {
            // 1. 이미지 교체
            currentDough.transform.localScale = finishedScale;
            doughSR.sprite = finishedCrepeSprites[spriteIndex];

            // 2. 기존 개별 토핑/스프레드 숨기기
            foreach (Transform child in currentDough.transform)
            {
                child.gameObject.SetActive(false);
            }

            // 3. [핵심 수정] 여기서 돈을 지급합니다! (6000원)
            if (EconomyManager.Instance != null)
            {
                //EconomyManager.Instance.AddMoney(6000);
                Debug.Log($"[정산 완료] {finalToppingType} 크레페 완성!");
            }
            else
            {
                Debug.LogWarning("EconomyManager가 없어서 돈을 줄 수 없습니다!");
            }
        }
    }

    public void ClearBoard()
    {
        // 1. 도마 위에 등록된 도우 데이터 제거
        if (currentDough != null)
        {
            // 도우 오브젝트 자체를 파괴 (자식인 토핑, 스프레드도 함께 삭제됨)
            Destroy(currentDough.gameObject);
            currentDough = null;
        }

        // 2. 기록된 레시피 정보 초기화
        currentSpread = SpreadType.None;
        addedToppings.Clear();

        Debug.Log("도마가 완전히 비워졌습니다. 다음 요리 준비 완료!");
    }
}