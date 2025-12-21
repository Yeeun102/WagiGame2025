using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

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
        {
            Destroy(child.gameObject);
        }

        if (_ingredients == null)
            return;

        // 재료 슬롯 생성
        foreach (var pair in _ingredients)
        {
            string itemID = pair.Key;
            int amount = pair.Value;

            if (amount <= 0)
                continue;

            // ItemData 조회 (읽기 전용)
            ItemData itemData = Resources.Load<ItemData>($"ItemData/{itemID}");

            GameObject slot = Instantiate(ingredientSlotPrefab, ingredientPanel);

            // 슬롯 안에 TMP 텍스트가 하나 있다고 가정
            TextMeshProUGUI text = slot.GetComponentInChildren<TextMeshProUGUI>();

            if (text == null)
                continue;

            if (itemData != null)
            {
                text.text = $"{itemData.재료명} x {amount}";
            }
            else
            {
                // ItemData가 없을 경우 안전 fallback
                text.text = $"{itemID} x {amount}";
            }
        }
    }
}