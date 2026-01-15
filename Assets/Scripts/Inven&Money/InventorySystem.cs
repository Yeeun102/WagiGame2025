using UnityEngine;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance;


    public Dictionary<string, int> inventory = new();

    public int maxInventory = 10; // ���׷��̵�� ����

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool HasItem(string itemID, int amount = 1)
    {

        return inventory.ContainsKey(itemID) && inventory[itemID] >= amount;

    }

    public bool AddItem(string itemID, int amount)
    {

        if (!inventory.ContainsKey(itemID))
            inventory[itemID] = 0;

        if (inventory[itemID] + amount > maxInventory)
            return false;

        inventory[itemID] += amount;
        return true;
    }

    public bool UseItem(string itemID, int amount = 1)
    {
        if (!HasItem(itemID, amount)) return false;

        inventory[itemID] -= amount;
        return true;
    }
}