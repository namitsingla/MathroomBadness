using UnityEngine;

public class camerabob : MonoBehaviour
{
    [Header("Bob Settings")]
    public float bobSpeed = 14f;      
    public float bobAmount = 0.05f;   

    private float defaultYPos;
    private float timer = 0;

    void Start()
    {
        // Remember where the camera started
        defaultYPos = transform.localPosition.y;
    }

    void Update()
    {
        if (Time.deltaTime == 0) return;

        // 1. Check for raw input instead of physics velocity
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // 2. If the player is pressing movement keys...
        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
            timer += Time.deltaTime * bobSpeed;
            float newY = defaultYPos + Mathf.Sin(timer) * bobAmount;
            transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
        }
        else
        {
            // 3. Reset and smoothly glide back to the center when stopped
            timer = 0;
            float returnY = Mathf.Lerp(transform.localPosition.y, defaultYPos, Time.deltaTime * bobSpeed);
            transform.localPosition = new Vector3(transform.localPosition.x, returnY, transform.localPosition.z);
        }
    }
}