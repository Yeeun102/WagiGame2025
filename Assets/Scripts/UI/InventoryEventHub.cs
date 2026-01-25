using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryEventHub : MonoBehaviour
{
    public static InventoryEventHub Instance;

    public event Action<Dictionary<string, int>> OnInventoryChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void NotifyInventoryChanged()
    {
        if (InventorySystem.Instance == null) return;
        OnInventoryChanged?.Invoke(InventorySystem.Instance.inventory);
    }
}
