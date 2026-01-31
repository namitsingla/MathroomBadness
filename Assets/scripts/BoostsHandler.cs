using UnityEngine;

public class BoostsHandler : MonoBehaviour
{
    public player_controller player_Controller;
    public collectedisplay collectedisplay;
    public void SpeedIncrease()
    {
        player_Controller.moveSpeed *= 1.25f;
    }

    public void MultiplierIncrease()
    {
        collectedisplay.mult += 0.15f;
    }

    public void VisionIncrease()
    {
        if (RenderSettings.fogDensity <= 0.01f) RenderSettings.fogDensity = 0f;
         else RenderSettings.fogDensity -= 0.01f;
    }
}
