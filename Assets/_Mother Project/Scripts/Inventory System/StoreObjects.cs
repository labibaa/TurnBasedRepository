using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Store Objects", menuName = "ScriptableObjects/Store")]
public class StoreObjects : ScriptableObject
{
    public List<InventoryItem> storeObjects = new List<InventoryItem>();
}
