using UnityEngine;
public enum EnemyType
{
    Melee,
    Shooter
}
public class EnemyController : MonoBehaviour
{
    [Header("Enemy Type")]
    public EnemyType enemyType = EnemyType.Melee;

    [Header("Stats")]
    public int maxHP = 50;
    [SerializeField]private int currentHP;

    public float moveSpeed = 3f;
    public int damage = 10;

    [Header("Melee Settings")]
    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;

    [Header("Shooter Settings")]
    public float shootRange = 8f;
    public float minDistanceFromPlayer = 4f;   // Shooter tries to keep this distance
    public float fireRate = 1.2f;
    public GameObject bulletPrefab;            // Can be the same as player or a different one
    public float bulletSpeed = 10f;

    [Header("References")]
    public Transform player;                   // Assign the Player here (or find by tag)

    private float nextAttackTime = 0f;
    private float nextShootTime = 0f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHP = maxHP;

        // Auto find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (enemyType == EnemyType.Melee)
        {
            MeleeBehavior(distanceToPlayer);
        }
        else if (enemyType == EnemyType.Shooter)
        {
            ShooterBehavior(distanceToPlayer);
        }
    }

    // ==================== MELEE BEHAVIOR ====================
    private void MeleeBehavior(float distance)
    {
        if (distance > attackRange)
        {
            // Move towards player
            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // In attack range - Attack
            if (Time.time >= nextAttackTime)
            {
                AttackPlayer();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    // ==================== SHOOTER BEHAVIOR ====================
    private void ShooterBehavior(float distance)
    {
        Vector2 direction = (player.position - transform.position).normalized;

        // Keep distance from player
        if (distance > shootRange)
        {
            // Too far  move closer
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
        else if (distance < minDistanceFromPlayer)
        {
            // Too close  move away
            rb.MovePosition(rb.position - direction * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Good distance  shoot
            if (Time.time >= nextShootTime && bulletPrefab != null)
            {
                ShootAtPlayer(direction);
                nextShootTime = Time.time + fireRate;
            }
        }
    }

    // ==================== ATTACK / SHOOT ====================
    private void AttackPlayer()
    {
        Debug.Log(gameObject.name + " attacked the player for " + damage + " damage!");

        // Call damage on player (we will connect this later with PlayerStats)
        if (player != null)
        {
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }
        }
    }

    private void ShootAtPlayer(Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.owner = this.gameObject;   
            bulletScript.damage = damage;
        }
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = direction * bulletSpeed;
        }

        
        if (bulletScript != null)
        {
            bulletScript.damage = damage;
        }

        Debug.Log(gameObject.name + " shot at the player!");
    }

    // ==================== DAMAGE SYSTEM ====================
    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP < 0) currentHP = 0;

        Debug.Log(gameObject.name + " took " + amount + " damage. HP: " + currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " died!");
        // TODO: Add death animation, drop items, etc.
        Destroy(gameObject);
    }

    // Optional: visualize ranges in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (enemyType == EnemyType.Melee)
            Gizmos.DrawWireSphere(transform.position, attackRange);
        else
            Gizmos.DrawWireSphere(transform.position, shootRange);
    }
}
