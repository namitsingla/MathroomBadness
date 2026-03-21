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
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
public class player_controller : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 11f;
    public float diagonalBoostMultiplier = 1f;

    [Header("Mouse")]
    public float mouseSensitivity = 300f;
    public Transform cameraTransform;

    [Header("Look Back")]
    public float lookBackAngle = 180f;

    private CharacterController controller;
    private float xRotation = 0f;

    void Start() 
    { 
        controller = GetComponent<CharacterController>(); 
        Cursor.lockState = CursorLockMode.Locked; 
    } 
    
    void Update()
    {
        // --- MOUSE (UNTOUCHED) ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * SettingsData.CameraSensitivity * Time.deltaTime * 5; 
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * SettingsData.CameraSensitivity * Time.deltaTime; 
        xRotation -= mouseY; 
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); 
        //limits how much u can rotate vertically 
        
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); 
        transform.Rotate(Vector3.up * mouseX); 

        // --- MOVEMENT (UNTOUCHED BESIDES GetAxisRaw) ---
        // 1. Get raw input (-1, 0, or 1)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // 2. Calculate direction and normalize it to establish a consistent base speed
        Vector3 move = (transform.right * horizontal + transform.forward * vertical).normalized; 

        // 3. Determine current speed
        float currentSpeed = moveSpeed;

        // 4. Check if moving diagonally (both axes have non-zero input)
        if (horizontal != 0 && vertical != 0)
        {
            currentSpeed *= diagonalBoostMultiplier;
        }


        controller.Move(move * currentSpeed * Time.deltaTime * 2);

        // --- NAVMESH SNAPPING ---
        NavMeshHit hit; 
        if (NavMesh.SamplePosition(transform.position, out hit, 2.0f, NavMesh.AllAreas)) 
        { 
            // We only override the Y axis to stay glued to the floor.
            // Adding controller.skinWidth ensures the collider doesn't embed into the floor,
            // completely stopping the physics engine from fighting your speed during lag!
            transform.position = new Vector3(transform.position.x, hit.position.y + controller.skinWidth, transform.position.z);
        } 
            
        // --- LOOK BACK (UNTOUCHED) ---
        if (Input.GetKey(KeyCode.Space)) 
        { 
            cameraTransform.Rotate(0, lookBackAngle, 0); 
        } 
    } 
}