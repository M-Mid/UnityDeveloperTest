using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target; // Drag Exo Gray here

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0, 2f, -5f); // 2 units up, 5 units behind
    public float followSpeed = 15f;
    public float mouseSensitivity = 3f;

    private float pitch = 15f; // Up/down tilt

    void Start()
    {
        // Lock the mouse to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // 1. Get Mouse Input
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            // 2. TURN THE PLAYER: Mouse X rotates the character's body left and right.
            // This is the secret to keeping the camera locked behind them!
            target.Rotate(0, mouseX, 0, Space.Self);

            // 3. TILT THE CAMERA: Mouse Y tilts the camera view up and down.
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, -15f, 60f); // Stops the camera from flipping upside down

            // 4. Find the perfect spot directly behind the player's back
            Vector3 idealPosition = target.TransformPoint(offset);

            // 5. CAMERA COLLISION: Shoot a laser to prevent looking through walls
            Vector3 rayStart = target.position + target.up * 1.5f;
            Vector3 rayDir = idealPosition - rayStart;

            if (Physics.Raycast(rayStart, rayDir.normalized, out RaycastHit hit, rayDir.magnitude))
            {
                // Snap camera slightly in front of the wall
                idealPosition = hit.point - (rayDir.normalized * 0.2f);
            }

            // 6. Move the camera to the ideal position smoothly
            transform.position = Vector3.Lerp(transform.position, idealPosition, Time.deltaTime * followSpeed);

            // 7. Lock the camera's rotation to exactly match the player's back, plus the up/down tilt
            transform.rotation = target.rotation * Quaternion.Euler(pitch, 0, 0);
        }
    }
}