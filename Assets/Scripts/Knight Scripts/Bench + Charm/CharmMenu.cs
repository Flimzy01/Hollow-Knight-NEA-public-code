using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CharmsMenu : MonoBehaviour
{
    //uses game objects, tranforms and text to for what the actual objects used in the menu are
    public GameObject charmsMenuObject;
    public GameObject charmSlotPrefab;
    public Transform charmsGrid;
    public TMP_Text descriptionText;
    public TMP_Text notchText;

    //array of all of the charms the player has
    public Charm[] allCharms;

    //max number of slots the player can use as each charm has an assigned number of notches
    public int maxNotches = 4;

    //checks if the player was on the bench to know when to open the menu
    private bool wasOnBench = false;
    private bool menuOpen = false;

    void Update()
    {
        //Opens the menu if the player sits down and closes it when they stand up
        if (BenchCheck.satOnBench && !wasOnBench)
            OpenMenu();

        if (!BenchCheck.satOnBench && wasOnBench)
            CloseMenu();

        wasOnBench = BenchCheck.satOnBench;
    }

    void OpenMenu()
    {
        //if the menu is already open, will not run again
        if (menuOpen) 
        return;

        //runs all processes to add the charms, info about them and the player notches
        menuOpen = true;
        charmsMenuObject.SetActive(true);
        PopulateCharms();
        UpdateNotchDisplay();

        //changes the text stores in the TMP_text object
        descriptionText.text = "Select a charm.";
    }

    void CloseMenu()
    {

        //sets eveything in the menu to be inactive
        if (!menuOpen) return;
        menuOpen = false;
        charmsMenuObject.SetActive(false);

        //destroys the slots generated for the charms in the grid which are remade when the menu is next opened
        foreach (Transform child in charmsGrid)
            Destroy(child.gameObject);
    }

    void PopulateCharms()
    {
        //clear any slots left from the last open
        foreach (Transform child in charmsGrid)
            Destroy(child.gameObject);


        foreach (Charm charm in allCharms)
        {
            GameObject slot = Instantiate(charmSlotPrefab, charmsGrid);
            CharmSlot slotScript = slot.GetComponent<CharmSlot>();

            slotScript.Init(charm, this);

            //takes capturedSlot as a perameter at the given point of this execution
            CharmSlot capturedSlot = slotScript;

            //adds the ability to click the charms to equip them
            Button btn = slot.GetComponent<Button>();
            if (btn == null) btn = slot.AddComponent<Button>();
            
            //when the button is clicked, will update the notch count
            btn.onClick.AddListener(() =>
            {
                capturedSlot.OnClick();
                UpdateNotchDisplay();
            });

            //when they hover over the button this will allow the description to show
            EventTrigger trigger = slot.GetComponent<EventTrigger>();
            if (trigger == null) trigger = slot.AddComponent<EventTrigger>();

            EventTrigger.Entry hoverEntry = new EventTrigger.Entry();
            hoverEntry.eventID = EventTriggerType.PointerEnter;
            hoverEntry.callback.AddListener((_) => capturedSlot.OnHover());
            trigger.triggers.Add(hoverEntry);
        }
    }

    public void ShowDescription(Charm charm, bool tooExpensive)
    {
        //shows the charm name and info and will show red text if the player does not have enough notches to use it
        if (tooExpensive)
            descriptionText.text = $"{charm.charmName}\n{charm.description}\n\n<color=red>Not enough notches!</color>";
        else
            descriptionText.text = $"{charm.charmName}\n{charm.description}\n\nNotch Cost: {charm.notchCost}";
    }

    public void UpdateNotchDisplay()
    {
        //shows the current player notches and their max notches
        notchText.text = $"Notches: {GetUsedNotches()} / {maxNotches}";
    }

    public int GetUsedNotches()
    {
        //shows the notchcost of every charm currently equipped
        int used = 0;
        foreach (Charm charm in allCharms)
            if (PlayerStats.charmInventory.Contains(charm.charmName))
                used += charm.notchCost;
        return used;
    }
}