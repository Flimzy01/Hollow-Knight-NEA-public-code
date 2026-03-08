using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    //overlay image for when in the menu
    private Image optionsOverlay;

    //bool to store if the player is actually in the menu
    private bool inGameMenu = false;

    void Start()
    {
        //hides the children of the component the main component has an alpha channel of zero
        ShowChildren(false);
        optionsOverlay = GetComponent<Image>();

        //doesent block clicks while not open
        optionsOverlay.raycastTarget = false;
    }
    void Update()
    {
        //toggles the menu on the escape click
        if (Keyboard.current.escapeKey.wasPressedThisFrame && !inGameMenu)
        { 
            StartCoroutine(FadeIn());
            ShowChildren(true);
            inGameMenu = true;
            return;
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame && inGameMenu)
        {
            ShowChildren(false);
            StartCoroutine(FadeOut());
            inGameMenu = false;
        }

    }

    IEnumerator FadeIn()
    {
        //allows the player to click the menu
        optionsOverlay.raycastTarget = true;

        //allows the menu to fade in by increasing the alpha value
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * 1.5f;
            optionsOverlay.color = new Color(0, 0, 0, Mathf.Lerp(0, 0.85f, time));
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        //menu fades out by decreasing the alpha value
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * 1.5f;
            optionsOverlay.color = new Color(0, 0, 0, Mathf.Lerp(0.85f, 0, time));
            yield return null;
        }

        //options menu will no longer block clicks
        optionsOverlay.raycastTarget = false;
    }

    //hides all the children of the options menu on exit
    private void ShowChildren(bool visibile)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(visibile);
        }
    }

    public void Resume()
    {
        ShowChildren(false);
        StartCoroutine(FadeOut());
        inGameMenu = false;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
