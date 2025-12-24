using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class HUDController : MonoBehaviour
{
    [Header("HUD UI Elements")]
    public TextMeshProUGUI money;
    public TextMeshProUGUI fame;
    public Transform ingredientPanel;
    public GameObject ingredientSlotPrefab;
    public TextMeshProUGUI cookingState;

    [Header("Item Data")]
    [Tooltip("모든 ItemData 에셋을 여기에 등록하세요")]
    public List<ItemData> itemDataList = new();

    private Dictionary<string, ItemData> itemDataDict = new();

    private void Awake()
    {
        // ID → ItemData 매핑 생성
        itemDataDict.Clear();
        foreach (var data in itemDataList)
        {
            if (data != null && !itemDataDict.ContainsKey(data.ID))
            {
                itemDataDict.Add(data.ID, data);
            }
        }
    }

    // HUD UI 갱신
    public void Refresh(
        int getMoney,
        int getFame,
        Dictionary<string, int> ingredientDict,
        string getCookingState)
    {

        if (money != null) money.text = getMoney.ToString();
        if (fame != null) fame.text = getFame.ToString();
        if (cookingState != null) cookingState.text = getCookingState;

        // 재료 슬롯 생성
        foreach (var pair in ingredientDict)
        {
            string itemID = pair.Key;
            int amount = pair.Value;

            if (!itemDataDict.TryGetValue(itemID, out ItemData itemData))
                continue;

            GameObject slot = Instantiate(ingredientSlotPrefab, ingredientPanel);

            IngredientSlot ingredientSlot = slot.GetComponent<IngredientSlot>();
            if (ingredientSlot != null)
            {
                ingredientSlot.SetData(
                    itemData.재료명,
                    amount,
                    itemData.최대보관수량
                );
            }
        }
    }
}
