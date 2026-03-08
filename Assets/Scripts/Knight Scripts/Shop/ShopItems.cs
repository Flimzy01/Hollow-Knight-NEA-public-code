using UnityEngine;

[CreateAssetMenu(fileName = "ShopItems", menuName = "Scriptable Objects/ShopItems")]
public class ShopItem : ScriptableObject
{
    //identifier for each item
    public string itemName;

    //description of the item
    public string description;

    //sprite which accompanies the item
    public Sprite icon;

    //geo cost of the item
    public int cost;
}
