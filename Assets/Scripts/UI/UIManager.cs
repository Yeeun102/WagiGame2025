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

        // [수정] 팝업 패널이 있을 때만 끄기 (없으면 무시)
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }

        // [수정] 닫기 버튼이 있을 때만 연결 (없으면 무시)
        if (popupCloseButton != null)
        {
            popupCloseButton.onClick.AddListener(HidePopup);
        }
    }

    //HUD 업데이트
    public void UpdateHUD()
    {
        if (hud == null) return;

        var economy = GameStateManager.Instance.economyManager;
        var inventory = GameStateManager.Instance.inventorySystem;

        int money = economy.GetCurrentMoney();
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