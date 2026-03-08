using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour
{
    //what is hidden or revealed while in the trigger of the shop
    public GameObject shopMenuObject;

    //prefab for items in the shop
    public GameObject shopSlotPrefab;
    
    //parent transform used to organise items into the shop
    public Transform shopGrid;

    //text boxes used to present information on the side
    public TMP_Text descriptionText;
    public TMP_Text geoText;

    //all items avaialable in the shop
    public ShopItem[] allItems;

    private bool wasInShop = false;
    private bool menuOpen = false;

    void Update()
    {
        if (ShopCheck.inShop && !wasInShop)
            OpenMenu();
        if (!ShopCheck.inShop && wasInShop)
            CloseMenu();

        wasInShop = ShopCheck.inShop;
    }


    void OpenMenu()
    {
        //wont run if the menu is already open
        if (menuOpen)
        return;

        menuOpen = true;
        shopMenuObject.SetActive(true);
        PopulateItems();
        UpdateGeoDisplay();
    }

    void CloseMenu()
    {
        //wont run if the menu is already closed
        if (!menuOpen)
        return;

        menuOpen = false;
        shopMenuObject.SetActive(false);
        foreach (Transform child in shopGrid)
            Destroy(child.gameObject);
    }

    void PopulateItems()
    {
        //clears slots made the last time the shop was opened
        foreach (Transform child in shopGrid)
            Destroy(child.gameObject);

        foreach (ShopItem item in allItems)
        {
            GameObject slot = Instantiate(shopSlotPrefab, shopGrid);
            ShopItemSlot slotScript = slot.GetComponent<ShopItemSlot>();
            slotScript.Init(item, this);

            ShopItemSlot captured = slotScript;

            //attempts to buy the item when the player clicks it
            Button btn = slot.GetComponent<Button>() ?? slot.AddComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                captured.OnClick();
                UpdateGeoDisplay();
            });

            //shows the description on hover
            EventTrigger trigger = slot.GetComponent<EventTrigger>() ?? slot.AddComponent<EventTrigger>();
            EventTrigger.Entry hover = new EventTrigger.Entry();
            hover.eventID = EventTriggerType.PointerEnter;
            hover.callback.AddListener((_) => captured.OnHover());
            trigger.triggers.Add(hover);
        }
    }

    //updates the description pannel and shows not enough geo in red if they cannot afford it
    public void ShowDescription(ShopItem item, bool cantAfford)
    {
        if (cantAfford)
            descriptionText.text = $"{item.itemName}\n{item.description}\n\n<color=red>Not enough Geo!</color>";
        else
            descriptionText.text = $"{item.itemName}\n{item.description}\n\nCost: {item.cost}";
    }

    //refreshes the geo
    public void UpdateGeoDisplay()
    {
        geoText.text = $"Geo: {GeoScript.geo}";
    }
}