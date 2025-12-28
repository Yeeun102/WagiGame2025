using UnityEngine;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance;

    public Dictionary<string, int> 재고 = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("InventorySystem Awake 실행됨"); // 나중에 삭제
        }
        else Destroy(gameObject);
    }


    public bool HasItem(string itemID)
    {
        return 재고.ContainsKey(itemID) && 재고[itemID] > 0;
    }

    public void UseItem(string itemID)
    {
        if (HasItem(itemID))
            재고[itemID]--;
    }
}
