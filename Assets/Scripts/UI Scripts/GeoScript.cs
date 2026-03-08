using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class GeoScript : MonoBehaviour
{
    public static int geo;
    public TMP_Text geoText;
    int lastGeoCheck;


    void Start()
    {
        geoText.text = geo.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        GeoUpdate(lastGeoCheck);
        lastGeoCheck = geo;
    }

    void GeoUpdate(int geoAmount)
    {
        if(geoAmount == geo)
        return;

        geoText.text = geo.ToString();
    }
}
