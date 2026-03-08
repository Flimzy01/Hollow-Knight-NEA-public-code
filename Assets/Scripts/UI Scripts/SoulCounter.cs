using TMPro;
using UnityEngine;

public class SoulCounter : MonoBehaviour
{
    public TMP_Text soulMeter;
    void Start()
    {
        soulMeter.text = PlayerStats.soul.ToString(); 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(PlayerStats.soul > PlayerStats.maxSoul)
        {
            PlayerStats.soul = PlayerStats.maxSoul;
        }
        soulMeter.text = PlayerStats.soul.ToString();
    }

}
