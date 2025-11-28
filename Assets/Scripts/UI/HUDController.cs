using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("HUD UI Elements")]
    public TMPro.TextMeshProUGUI money;
    public TMPro.TextMeshProUGUI fame;
    public TMPro.TextMeshProUGUI ingredient;
    public TMPro.TextMeshProUGUI cookingState;

    private int _money;
    private int _fame;
    private int _ingredient;
    private string _cookingState;

    // HUD UI °»½Å
    public void Refresh(int getMoney, int getFame, int getIngredient, string getCookingState)
    {
        _money = getMoney;
        _fame = getFame;
        _ingredient = getIngredient;
        _cookingState = getCookingState;

        if (money != null) money.text = _money.ToString();
        if (fame != null) fame.text = _fame.ToString();
        if (ingredient != null) ingredient.text = _ingredient.ToString();
        if (cookingState != null) cookingState.text = _cookingState;
    }
}
