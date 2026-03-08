using UnityEngine;
using UnityEngine.UI;

public class CharmSlot : MonoBehaviour
{
    //the charm that a specific charm represents
    public Charm charm;
    private CharmsMenu menu;
    private bool isEquipped = false;

    //icon for each charm
    private Image charmImage;

    //called by charmmenu
    public void Init(Charm charm, CharmsMenu menu)
    {
        this.charm = charm;
        this.menu = menu;

        charmImage = GetComponentInChildren<Image>();

        //if the charm is already equipped the charm menu will already show what is equipped
        isEquipped = PlayerStats.charmInventory.Contains(charm.charmName);
        UpdateVisual();
    }

    public void OnClick()
    {
        //toggles equip state when the slot is clicked
        if (isEquipped)
        {
            //removes the charm from the list stored in playerstats
            isEquipped = false;
            PlayerStats.charmInventory.Remove(charm.charmName);
        }
        else
        {
            //will not add the charm if they dont have enough notches
            int usedNotches = menu.GetUsedNotches();
            if (usedNotches + charm.notchCost > menu.maxNotches)
            {
                menu.ShowDescription(charm, tooExpensive: true);
                return;
            }

            // if equipped will add it to the list in playerstats
            isEquipped = true;
            PlayerStats.charmInventory.Add(charm.charmName);
        }

        UpdateVisual();
        menu.ShowDescription(charm, tooExpensive: false);
        menu.UpdateNotchDisplay();
    }

    public void OnHover()
    {
        //shows the charm description when the cursor hovers over the slot
        menu.ShowDescription(charm, tooExpensive: false);
    }

    void UpdateVisual()
    {
        //swaps sprites based on current equip state
        if (charmImage == null)
            return;

        if (charm.equippedSprite != null && charm.unequippedSprite != null)
            charmImage.sprite = isEquipped ? charm.equippedSprite : charm.unequippedSprite;

    }
}