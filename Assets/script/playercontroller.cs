using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;

    [Header("Gravity Settings")]
    public float gravityStrength = 40f;

    [Header("Ground Check")]
    public float raycastLength = 0.5f; // Made the laser shorter and more accurate!

    private Rigidbody rb;
    private Animator anim;
    private Vector3 currentGravity = Vector3.down;
    private bool isGrounded;

    private float lastJumpTime; // NEW: Tracks the exact time you jumped

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void Update()
    {
        HandleMovement();
        HandleGravityInput();

        // Tells the Animator to switch between Falling and Idle/Run
        anim.SetBool("isGrounded", isGrounded);
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 moveInput = (transform.right * h + transform.forward * v).normalized;
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.deltaTime);

        anim.SetFloat("Speed", moveInput.magnitude);

        // NEW: The Cooldown! Time.time must be greater than your last jump + 0.2 seconds.
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && Time.time > lastJumpTime + 0.2f)
        {
            lastJumpTime = Time.time; // Record the time you jumped
            rb.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
            anim.SetTrigger("Jump");
        }
    }

    void HandleGravityInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) ChangeGravity(transform.right);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) ChangeGravity(-transform.right);
        if (Input.GetKeyDown(KeyCode.UpArrow)) ChangeGravity(transform.forward);
        if (Input.GetKeyDown(KeyCode.DownArrow)) ChangeGravity(transform.up);
    }

    void ChangeGravity(Vector3 newDirection)
    {
        transform.position += transform.up * 0.5f;

        Quaternion rotationOffset = Quaternion.FromToRotation(transform.up, -newDirection);
        transform.rotation = rotationOffset * transform.rotation;

        Vector3 euler = transform.eulerAngles;
        euler.x = Mathf.Round(euler.x / 90f) * 90f;
        euler.y = Mathf.Round(euler.y / 90f) * 90f;
        euler.z = Mathf.Round(euler.z / 90f) * 90f;
        transform.eulerAngles = euler;

        currentGravity = newDirection;
    }

    void FixedUpdate()
    {
        rb.AddForce(currentGravity * gravityStrength, ForceMode.Acceleration);

        // Start the laser right at the feet
        Vector3 rayStart = transform.position + (transform.up * 0.1f);

        // Shoot the laser
        isGrounded = Physics.Raycast(rayStart, -transform.up, raycastLength);

        Debug.DrawRay(rayStart, -transform.up * raycastLength, Color.red);
    }
}