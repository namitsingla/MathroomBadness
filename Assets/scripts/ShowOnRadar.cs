using UnityEngine;

public class ShowOnRadar : MonoBehaviour
{
    BoostsHandler boostsHandler;
    private GameObject marker;
    Transform player;
    void Awake()
    {
        boostsHandler = ReferencesManager.instance.boostsHandler;
        player = ReferencesManager.instance.player;

        Transform markerTransform =  transform.Find("Marker");
        if (markerTransform != null)
            marker = markerTransform.gameObject;
    }

    void Update()
    {
        if (!boostsHandler.isRadarOn) return;

        if (Vector3.Distance(transform.position, player.position) <= 60f)
        {
            marker.SetActive(true);
        }
        else
        {
            marker.SetActive(false);
        }
    }
}
