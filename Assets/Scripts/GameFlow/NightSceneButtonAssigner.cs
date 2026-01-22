using UnityEngine;
using UnityEngine.UI; // Button 컴포넌트 사용을 위해 필수

public class NightSceneButtonAssigner : MonoBehaviour
{
    void Start()
    {
        // 1. 버튼 찾기 (이름이나 태그로)
        Button btn = GetComponent<Button>();

        if (btn != null)
        {
            // 2. 기존 연결 청소 후 새 연결 추가
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => {
                // GameStateManager의 Instance를 통해 함수 호출
                GameStateManager.Instance.EndDay();
            });

            Debug.Log("버튼 클릭 이벤트가 코드로 연결되었습니다.");
        }
    }
}