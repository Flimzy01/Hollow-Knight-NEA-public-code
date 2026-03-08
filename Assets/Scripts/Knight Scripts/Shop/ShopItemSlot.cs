using UnityEngine;
using UnityEngine.UI;

public class ShopItemSlot : MonoBehaviour
{
    private ShopItem item;
    private ShopMenu menu;
    private Image itemImage;
    private bool purchased = false;

    //called by shopmenu to add all of the item data into the shop
    public void Init(ShopItem shopItem, ShopMenu shopMenu)
    {
        item = shopItem;
        menu = shopMenu;

        itemImage = GetComponentInChildren<Image>();
        if (itemImage != null)
            itemImage.sprite = item.icon;

        //used to check if the player has already bought the item
        purchased = PlayerStats.inventory.Contains(item.itemName);
        UpdateVisual();
    }

    public void OnClick()
    {
        //only shows the item description if it is already bought
        if (purchased)
        {
            menu.ShowDescription(item, cantAfford: false);
            return;
        }

        //if the player cant afford the item the game wont let them buy it showing a warning
        if (GeoScript.geo < item.cost)
        {
            menu.ShowDescription(item, cantAfford: true);
            return;
        }

        //shows the player they can purchase the item
        menu.ShowDescription(item, cantAfford: false);
        menu.UpdateGeoDisplay();

        //otherwise the geo is taken and the item is set as purchased
        GeoScript.geo -= item.cost;
        purchased = true;
        UpdateVisual();

        //adds the item to the players inventory for the save file
        PlayerStats.inventory.Add(item.itemName);
    }

    //previews the item description
    public void OnHover()
    {
        menu.ShowDescription(item, cantAfford: GeoScript.geo < item.cost);
    }

    //greys the icon to show the item has been bought
    void UpdateVisual()
    {
        if (itemImage == null) return;
        itemImage.color = purchased ? Color.gray : Color.white;
    }
}