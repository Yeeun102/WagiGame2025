using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD")]
    public HUDController hud;

    [Header("Popup")]
    public GameObject popupPanel;
    public Text popupMessage;
    public Button popupCloseButton;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 팝업 기본 비활성화
        popupPanel.SetActive(false);

        // 닫기 버튼 연결
        popupCloseButton.onClick.AddListener(HidePopup);
    }

    //HUD 업데이트
    public void UpdateHUD()
    {
        if (hud == null) return;

        var economy = GameStateManager.Instance.economyManager;
        var inventory = GameStateManager.Instance.inventorySystem;

        int money = economy.Money;
        int fame = 0; // 아직 FameManager 없으면 0
        Dictionary<string, int> ingredients = inventory.inventory;
        string cookingState = "";

        hud.Refresh(money, fame, ingredients, cookingState);
    }


    public void ShowPopup(string popupMassage)
    {
        // TODO: 팝업 표시
        popupMessage.text = popupMassage;
        popupPanel.SetActive(true);
    }

    // 팝업 숨기기
    public void HidePopup()
    {
        popupPanel.SetActive(false);
    }
}
