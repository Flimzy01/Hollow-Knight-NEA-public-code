using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GenerateHealthSprite : MonoBehaviour
{
    //hearts and masks are the same thing in this case
    //heart UI prefab
    public GameObject healthPrefab;

    //list used for activating the heart sprites
    List<InitiateHealthSprite> health = new List<InitiateHealthSprite>();

    //heart scale size
    private Vector2 size = new Vector2(1, 1);
    

    private void OnEnable()
    {
        //subscribes to the damage event
        PlayerHealth.OnPlayerDamaged += DrawHearts;
    }

    private void OnDisable()
    {
        //unsubscribes to prevent calling a destoyed object
        PlayerHealth.OnPlayerDamaged -= DrawHearts;
    }

    private void Start()
    {
        //draws the initial state
        DrawHearts();
    }


    public void DrawHearts()
    {
        ClearMasks();

        float maxPlayerHealth = PlayerHealth.maxHealth;
        int heartsToMake = (int)(PlayerHealth.maxHealth);

        //creates one empty heart for the max health-points
        for (int i = 0; i < heartsToMake; i++)  
            CreateEmptyMasks();

        //sets the hearts to be full
        for (int i = 0; i < PlayerHealth.health; i++)
            health[i].SetHeartImg(HeartStatus.Full);
    }

    public void CreateEmptyMasks()
    {
        //makes an instance of an empty health sprite and stores it in a list
        GameObject newMask = Instantiate(healthPrefab);
        newMask.transform.SetParent(transform);
        newMask.transform.localScale = size;

        InitiateHealthSprite heartComponent = newMask.GetComponent<InitiateHealthSprite>();
        heartComponent.SetHeartImg(HeartStatus.Empty);
        health.Add(heartComponent);

    }
    public void ClearMasks()
    {
        //destroys all child heart objects and clears the tracked list
        foreach (Transform t in transform)
            Destroy(t.gameObject);

        health = new List<InitiateHealthSprite>();


    }
}
