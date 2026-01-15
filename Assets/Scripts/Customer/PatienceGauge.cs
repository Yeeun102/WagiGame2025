using UnityEngine;
using UnityEngine.UI;

public class PatienceGauge : MonoBehaviour
{
    public Image fillImage;
    public Color greenColor = Color.green;
    public Color yellowColor = Color.yellow;
    public Color redColor = Color.red;

    public void UpdateGauge(float fillAmount)
    {
        fillImage.fillAmount = fillAmount;

        // 시간에 따라 색상 변경 (초록 -> 노랑 -> 빨강)
        if (fillAmount > 0.5f)
        {
            fillImage.color = Color.Lerp(yellowColor, greenColor, (fillAmount - 0.5f) * 2f);
        }
        else
        {
            fillImage.color = Color.Lerp(redColor, yellowColor, fillAmount * 2f);
        }
    }
}