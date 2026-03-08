using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class InitiateHealthSprite : MonoBehaviour
{
    //sprites for full and empty hearts
    public Sprite healthFull, healthEmpty;

    //the image the sprite is put on
    Image MaskContainer;

    private void Awake()
    {
        MaskContainer = GetComponent<Image>();
    }

    public void SetHeartImg(HeartStatus status)
    {
        //sets the heart state to the provided state
        switch (status)
        {
            case HeartStatus.Empty:
                MaskContainer.sprite = healthEmpty;
                break;

            case HeartStatus.Full:
                MaskContainer.sprite = healthFull;
                break;
            }
        }
    }

//represents a full or empty heart
public enum HeartStatus
    {
        Empty = 0,
        Full = 1
    }

