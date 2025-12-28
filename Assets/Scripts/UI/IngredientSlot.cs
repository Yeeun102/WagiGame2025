using TMPro;
using UnityEngine;

public class IngredientSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI amountText;

    public void SetData(string ingredientName, int current, int max)
    {
        nameText.text = ingredientName;
        amountText.text = $"{current} / {max}";
    }
}