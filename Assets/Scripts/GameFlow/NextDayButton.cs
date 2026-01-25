using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환을 위해 필요

public class NextDayButton : MonoBehaviour
{
    // 버튼을 눌렀을 때 실행될 함수
    public void StartNextDay()
    {
        if (GameStateManager.Instance != null)
        {
            // 1. 오늘 치 실적 데이터 리셋 및 날짜 증가
            GameStateManager.Instance.ResetResultsForNextDay();

            // 2. 다시 영업 씬(RealDayScene)으로 이동
            SceneManager.LoadScene("RealDayScene");
        }
        else
        {
            Debug.LogError("GameStateManager 인스턴스를 찾을 수 없습니다!");
            // 만약 매니저가 없다면 그냥 씬이라도 이동하게 안전장치
            SceneManager.LoadScene("RealDayScene");
        }
    }
}
