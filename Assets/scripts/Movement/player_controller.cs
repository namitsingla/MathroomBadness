// using UnityEngine;
// using UnityEngine.AI;

// [RequireComponent(typeof(CharacterController))]
// public class player_controller : MonoBehaviour
// {
//     [Header("Movement")]
//     public float moveSpeed = 11f;

//     [Header("Mouse")]
//     public float mouseSensitivity = 300f;
//     public Transform cameraTransform;

//     [Header("Look Back")]
//     public float lookBackAngle = 180f;

//     private CharacterController controller;
//     private float xRotation = 0f;

//     void Start() 
//     { 
//         controller = GetComponent<CharacterController>(); 
//         Cursor.lockState = CursorLockMode.Locked; 
//     } 
//     void Update()
//     {
//         float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * SettingsData.CameraSensitivity * Time.deltaTime * 5; 
//         float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * SettingsData.CameraSensitivity * Time.deltaTime; 
//         xRotation -= mouseY; 
//         xRotation = Mathf.Clamp(xRotation, -90f, 90f); 
//         //limits how much u can rotate vertically 
        
//         cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); 
//         transform.Rotate(Vector3.up * mouseX); 

//         float moveX = Input.GetAxis("Horizontal"); 
//         float moveZ = Input.GetAxis("Vertical"); 
//         Vector3 move = transform.right * moveX + transform.forward * moveZ; 
//         controller.Move(move * moveSpeed * Time.deltaTime * 2);

//         NavMeshHit hit; 
//         if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas)) 
//         { 
//             transform.position = hit.position; 
//         } 
            
//             //for looking back 
//             if (Input.GetKey(KeyCode.Space)) 
//             { 
//                 cameraTransform.Rotate(0, lookBackAngle, 0); 
//             } 
//         } 
//     }


using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class player_controller : MonoBehaviour
{
    public enum TargetDevice { PC, Mobile }

    [Header("Device Selection")]
    [Tooltip("Change this to Mobile before making an Android build.")]
    public TargetDevice currentDevice = TargetDevice.PC;

    [Header("Mobile Controls")]
    public Joystick movementJoystick; 

    [Header("Movement")]
    public float moveSpeed = 11f;
    public float diagonalBoostMultiplier = 1f;
    [Range(0.05f, 0.5f)] 
    public float diagonalTolerance = 0.2f; 

    private Vector3 verticalVelocity;
    private float gravity = -15.0f;

    [Header("Look / Camera")]
    public float mouseSensitivity = 300f;
    public Transform cameraTransform;

    [Header("Look Back")]
    public float lookBackAngle = 180f;
    private bool isLookingBack = false; // Tracks if the player is holding the button

    private CharacterController controller;
    private float xRotation = 0f;
    public RarityManager rarityManager;

    void Start() 
    { 
        controller = GetComponent<CharacterController>(); 
        
        if (currentDevice == TargetDevice.PC)
        {
            Cursor.lockState = CursorLockMode.Locked; 
        }
    } 
    
    void Update()
    {
        if(PauseMenu.GameIsPaused) return;
        if (rarityManager.isRewardScreenUp) return;
        
        // --- CHECK LOOK BACK STATE (PC ONLY) ---
        // Mobile state is handled by the SetMobileLookBack function below
        if (currentDevice == TargetDevice.PC)
        {
            isLookingBack = Input.GetKey(KeyCode.Space);
        }

        // --- CAMERA ROTATION ---
        float lookX = 0f;
        float lookY = 0f;

        if (currentDevice == TargetDevice.Mobile)
        {
            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    if (touch.position.x > Screen.width / 2f && touch.phase == TouchPhase.Moved)
                    {
                        lookX = touch.deltaPosition.x * mouseSensitivity * SettingsData.CameraSensitivity * 5f * 0.01f;
                        lookY = touch.deltaPosition.y * mouseSensitivity * SettingsData.CameraSensitivity * 0.01f;
                    }
                }
            }
        }
        else 
        {
            lookX = Input.GetAxis("Mouse X") * mouseSensitivity * SettingsData.CameraSensitivity * 5f; 
            lookY = Input.GetAxis("Mouse Y") * mouseSensitivity * SettingsData.CameraSensitivity; 
        }

        xRotation -= lookY; 
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); 
        
        // APPLY LOOK BACK OFFSET HERE
        float currentYRotation = isLookingBack ? lookBackAngle : 0f;
        cameraTransform.localRotation = Quaternion.Euler(xRotation, currentYRotation, 0f); 
        
        // Horizontal turning is still applied to the player body
        transform.Rotate(Vector3.up * lookX); 

        // --- HORIZONTAL MOVEMENT ---
        float horizontal = 0f;
        float vertical = 0f;

        if (currentDevice == TargetDevice.Mobile && movementJoystick != null)
        {
            horizontal = movementJoystick.Horizontal;
            vertical = movementJoystick.Vertical;
        }
        else
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }

        Vector3 move = (transform.right * horizontal + transform.forward * vertical).normalized; 
        float currentSpeed = moveSpeed;

        // --- PROTECTED DIAGONAL BOOST ---
        float absX = Mathf.Abs(horizontal);
        float absY = Mathf.Abs(vertical);

        bool isTightDiagonal = Mathf.Abs(absX - absY) <= diagonalTolerance;
        bool isPushingHard = new Vector2(horizontal, vertical).magnitude > 0.7f;

        if (horizontal != 0 && vertical != 0 && isTightDiagonal && isPushingHard)
        {
            currentSpeed *= diagonalBoostMultiplier;
        }

        controller.Move(move * currentSpeed * Time.deltaTime * 2);

        // --- VERTICAL MOVEMENT (GRAVITY) ---
        if (controller.isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f;
        }

        verticalVelocity.y += gravity * Time.deltaTime;

        if (verticalVelocity.y < -20f) 
        {
            verticalVelocity.y = -20f; 
        }

        controller.Move(verticalVelocity * Time.deltaTime);
    }
    
    // --- MOBILE UI BUTTON FUNCTION ---
    // The UI button will call this to change the look back state
    public void SetMobileLookBack(bool lookingBack)
    {
        if (currentDevice == TargetDevice.Mobile)
        {
            isLookingBack = lookingBack;
        }
    }
}