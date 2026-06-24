using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float currentSpeed;
    [Header("Dash Settings")]
    [SerializeField] float dashSpeed = 15f;         
    [SerializeField] float dashDuration = 0.2f;      
    [SerializeField] float dashCooldown = 0.8f;      
    [SerializeField] bool canDash = true;           

    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;

    // Dash variables
    private bool isDashing = false;
    private float dashTimeLeft = 0f;
    private float dashCooldownTimer = 0f;
    private Vector2 dashDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>(); // Se você for usar Animator depois

        rb.gravityScale = 0f;
        currentSpeed = moveSpeed;
    }

    void Update()
    {
        if (!GameManager.Instance.gameHasStarted) return;
        if (GameManager.Instance.isChoosingUpgrade) return;
        // Input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Normalize movement
        if (movement != Vector2.zero)
        {
            movement.Normalize();
        }
        // Dash input (Left Shift)
        if (Input.GetKeyDown(KeyCode.Space) && canDash && !isDashing && dashCooldownTimer <= 0f)
        {
            StartDash();
        }

        // Update dash timers
        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0f)
            {
                EndDash();
            }
        }

        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
        // TODO: Update Animator parameters here later
        // animator.SetFloat("Horizontal", movement.x);
        // animator.SetFloat("Vertical", movement.y);
        // animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            // Move fast in dash direction
            rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Normal movement
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }
    #region Dash methods
    private void StartDash()
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        dashCooldownTimer = dashCooldown;

        // Use current movement direction, or last direction if standing still
        if (movement != Vector2.zero)
        {
            dashDirection = movement;
        }
        else
        {
            // If not moving, dash in the last known direction (or default forward)
            dashDirection = dashDirection != Vector2.zero ? dashDirection : Vector2.up;
        }

        Debug.Log("Dash started!");
    }

    private void EndDash()
    {
        isDashing = false;
        Debug.Log("Dash ended. Cooldown started.");
    }

    public void SetDashEnabled(bool enabled)
    {
        canDash = enabled;
        Debug.Log("Dash enabled: " + enabled);
    }
    #endregion
    public void IncreaseMoveSpeed(float amount)
    {
        currentSpeed += amount;
        Debug.Log("Velocidade do jogador aumentada para: " + currentSpeed);
    }
}
