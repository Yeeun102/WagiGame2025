using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateHUD()
    {
        // TODO: 돈, 재료, 평판 등 HUD 갱신
    }

    public void ShowPopup(string msg)
    {
        // TODO: 팝업 표시
    }
}
