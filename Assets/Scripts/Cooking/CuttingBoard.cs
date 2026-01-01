using UnityEngine;
using System.Collections.Generic;

public class CuttingBoard : MonoBehaviour
{
    public Transform anchorPoint; // 도우가 붙을 중심점
    public DragAndDropManager currentDough;
    public List<string> addedToppings = new List<string>();

    public void PlaceDough(DragAndDropManager dough)
    {
        currentDough = dough;

        // 1. 드래그 스크립트를 즉시 비활성화 (가장 중요!)
        // 업데이트 루프가 위치를 다시 계산하지 못하게 막습니다.
        dough.enabled = false;

        // 2. 부모 설정 및 좌표 강제 고정
        dough.transform.SetParent(this.transform, false);
        dough.transform.localPosition = (anchorPoint != null) ? anchorPoint.localPosition : Vector3.zero;

        // Z축은 도마보다 앞(-0.5 정도)으로 확실히 뺍니다.
        Vector3 pos = dough.transform.localPosition;
        pos.z = -0.5f;
        dough.transform.localPosition = pos;

        Debug.Log("고정 완료. 이제 움직이지 않아야 합니다.");
    }

    public void AddTopping(string toppingName)
    {
        if (currentDough == null) return;

        addedToppings.Add(toppingName);
        Debug.Log($"토핑 추가됨: {toppingName}");
        // 여기에 토핑 시각적 효과(Instantiate)를 추가할 수 있습니다.
    }
}
