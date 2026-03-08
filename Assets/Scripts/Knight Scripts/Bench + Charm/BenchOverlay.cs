using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BenchOverlay : MonoBehaviour
{
    private Image overlay;

    //tracks if the player was sat on a bench previously
    private bool wasOnBench = false;

    void Update()
    {
        //when the player sits down the overlay will fade in
        if (BenchCheck.satOnBench && !wasOnBench)
        {
            overlay = GetComponent<Image>();
            StartCoroutine(FadeIn());
        }

        //when the player stands up the overlay will fade out
        if (!BenchCheck.satOnBench && wasOnBench)
            StartCoroutine(FadeOut());

        wasOnBench = BenchCheck.satOnBench;
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < 1f)
        {   
            //uses time to make the alpha channel slowly change to be stronger
            t += Time.deltaTime * 1.5f;
            overlay.color = new Color(0, 0, 0, Mathf.Lerp(0, 0.85f, t));

            //waits one frame before returning
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < 1f)
        {
            //uses time to make the alpha channel slowly change to be weaker
            t += Time.deltaTime * 1.5f;
            overlay.color = new Color(0, 0, 0, Mathf.Lerp(0.85f, 0, t));
            
            yield return null;
        }
    }
}