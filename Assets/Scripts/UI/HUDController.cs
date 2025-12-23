using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class HUDController : MonoBehaviour
{
    [Header("HUD UI Elements")]
    public TMPro.TextMeshProUGUI money;
    public TMPro.TextMeshProUGUI fame;
    public Transform ingredientPanel;
    public GameObject ingredientSlotPrefab;
    public TMPro.TextMeshProUGUI cookingState;

    private int _money;
    private int _fame;
    private Dictionary<string, int> _ingredients;
    private string _cookingState;

    // HUD UI 갱신
    public void Refresh(int getMoney, int getFame, Dictionary<string, int> ingredientDict, string getCookingState)
    {
        _money = getMoney;
        _fame = getFame;
        _ingredients = ingredientDict;
        _cookingState = getCookingState;

        if (money != null) money.text = _money.ToString();
        if (fame != null) fame.text = _fame.ToString();
        if (cookingState != null) cookingState.text = _cookingState;

        // 재료 슬롯 초기화
        foreach (Transform child in ingredientPanel)
            Destroy(child.gameObject);

        // 재료 슬롯 생성
        foreach (var pair in _ingredients)
        {
            string id = pair.Key;
            int amount = pair.Value;

            // IngredientDB에서 이름/아이콘 가져오기
            //var data = IngredientDatabase.Instance.Get(id);

            var slot = Instantiate(ingredientSlotPrefab, ingredientPanel);

            /*
            slot.GetComponent<IngredientSlot>().SetData(
                data.이름,
                amount,
                data.아이콘
            );
            */
        }
    }
}
