using UnityEngine;
using System.Collections.Generic;

public class InventoryUIController : MonoBehaviour
{
    [Header("Inventory Window")]
    public GameObject inventoryPanel;

    [Header("Slot Settings")]
    public Transform slotParent;
    public GameObject ingredientSlotPrefab;

    [Header("Item Data")]
    public List<ItemData> itemDataList = new();

    private Dictionary<string, ItemData> itemDataDict = new();

    private void Awake()
    {
        foreach (var data in itemDataList)
        {
            if (data != null && !itemDataDict.ContainsKey(data.ID))
                itemDataDict.Add(data.ID, data);
        }
    }

    private void Start()
    {
        InventorySystem.Instance.AddItem("test", 1);
    }


    private void OnEnable()
    {
        if (InventoryEventHub.Instance != null)
            InventoryEventHub.Instance.OnInventoryChanged += RefreshUI;
    }

    private void OnDisable()
    {
        if (InventoryEventHub.Instance != null)
            InventoryEventHub.Instance.OnInventoryChanged -= RefreshUI;
    }

    // 🔹 인벤토리 버튼에서 호출
    public void ToggleInventory()
    {
        Debug.Log("ToggleInventory 호출됨");

        if (inventoryPanel == null)
        {
            Debug.LogError("Inventory Panel is not assigned.");
            return;
        }

        inventoryPanel.SetActive(!inventoryPanel.activeSelf);

        // 열릴 때 한 번 강제 동기화
        if (inventoryPanel.activeSelf)
        {
            RefreshUI(InventorySystem.Instance.inventory);
        }
    }

    private void RefreshUI(Dictionary<string, int> inventory)
    {
        foreach (Transform child in slotParent)
            Destroy(child.gameObject);

        foreach (var pair in inventory)
        {
            if (!itemDataDict.TryGetValue(pair.Key, out ItemData data))
                continue;

            GameObject slot = Instantiate(ingredientSlotPrefab, slotParent);
            slot.GetComponent<IngredientSlot>()
                .SetData(data.재료명, pair.Value, data.최대보관수량);
        }
    }
}