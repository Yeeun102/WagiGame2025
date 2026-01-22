using UnityEngine;
using UnityEngine.UI;

public class NightResultUI : MonoBehaviour
{
    public Text perfectText;
    public Text sosoText;
    public Text burntText;
    public Text moneyText;

    void Start()
    {
        DisplayResults();
    }

    void DisplayResults()
    {
        // 금고(GameStateManager)에서 데이터를 가져와 텍스트로 변환
        perfectText.text = "완벽한 주문: " + GameStateManager.Instance.perfectOrders;
        sosoText.text = "부족한 주문: "+ GameStateManager.Instance.sosoOrders;
        burntText.text = "태운 주문: " + GameStateManager.Instance.burntOrders;
        moneyText.text = "수익: " + GameStateManager.Instance.totalEarnings + " G";
    }
}