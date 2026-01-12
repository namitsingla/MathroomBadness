using UnityEngine;

public class CamRotateMenu : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float angleRange = 30f;   // max rotation angle (degrees, left/right)
    public float speed = 2f;         // how fast it swings

    private float initialY;

    void Start()
    {
        // store starting yaw (Y rotation)
        initialY = transform.localEulerAngles.y;
    }

    void Update()
    {
        // oscillate smoothly with a sine wave
        float angle = Mathf.Sin(Time.time * speed) * angleRange;

        // apply rotation only on Y axis (sideways)
        transform.localEulerAngles = new Vector3(
            transform.localEulerAngles.x,
            initialY + angle,
            transform.localEulerAngles.z
        );
    }
}
