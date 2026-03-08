using UnityEngine;

public class ShopCheck : MonoBehaviour
{
    //read by shopmenu to know when to open and close
    public static bool inShop;

    //shoptrigger causes the overlay to open
    public GameObject shopTrigger, shopOverlay;
    private GameObject player;

    void Start()
    {
        player = this.gameObject;
        shopOverlay.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == shopTrigger.name)
            inShop = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == shopTrigger.name)
            inShop = false;
    }

    void FixedUpdate()
    {
        if (inShop)
            shopOverlay.SetActive(true);
        else
            shopOverlay.SetActive(false);
    }
}
