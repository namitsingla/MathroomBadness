using UnityEngine;

public class camerabob : MonoBehaviour
{
 public float bobSpeed = 6f;      // How fast the bob oscillates
    public float bobAmount = 0.3f;  // How much the camera moves
    public CharacterController controller;

    private float defaultYPos;
    private float timer = 0;

    void Start()
    {
        defaultYPos = transform.localPosition.y;
    }

    void Update()
    {
            // Move in a sine wave pattern
           timer += Time.deltaTime * bobSpeed * controller.velocity.magnitude / 5;
            float newY = defaultYPos + Mathf.Sin(timer) * bobAmount;
            transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
        }
}
