using UnityEngine;
using System.Collections;
public enum EnemyType
{
    Melee,
    Shooter,
    Lunge
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
    [Header("XP Reward")]
    public int sendXp = 10;
    [Header("Melee Settings")]
    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;

    [Header("Shooter Settings")]
    public float shootRange = 8f;
    public float minDistanceFromPlayer = 4f;   
    public float fireRate = 1.2f;
    public GameObject bulletPrefab;            
    public float bulletSpeed = 10f;

    [Header("Lunge Settings")]
    public float lungeDetectionRange = 6f;     
    public float lungeStopDistance = 3f;       
    public float lungeSpeed = 18f;             
    public float lungePrepareTime = 0.6f;      
    public float lungeCooldown = 2.5f;

    private bool isLunging = false;
    private float lungeCooldownTimer = 0f;

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

        float distance = Vector2.Distance(transform.position, player.position);

        if (enemyType == EnemyType.Lunge)
        {
            LungeBehavior(distance);
        }
        else if (enemyType == EnemyType.Melee)
        {
            MeleeBehavior(distance);
        }
        else if (enemyType == EnemyType.Shooter)
        {
            ShooterBehavior(distance);
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
        // Dar XP pro player...
        if (player != null)
        {
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
                playerStats.AddXP(sendXp);
        }

        // Avisar o GameManager que um inimigo foi derrotado
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EnemyDefeated();
        }

        // Devolver para o pool
        if (PoolManager.Instance != null)
        {
            PoolManager.Instance.ReturnEnemyToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void ResetEnemy()
    {
        currentHP = maxHP;
        // Aqui você pode resetar outras coisas depois (velocidade, estado, etc)
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
    private void LungeBehavior(float distance)
    {
        if (lungeCooldownTimer > 0)
            lungeCooldownTimer -= Time.fixedDeltaTime;

        if (isLunging) return;

        if (distance > lungeStopDistance && distance < lungeDetectionRange)
        {
            // Anda até a distância de ataque
            Vector2 dir = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
        }
        else if (distance <= lungeStopDistance && lungeCooldownTimer <= 0)
        {
            // Para e prepara o dash
            StartCoroutine(PerformLunge());
        }
    }

    private IEnumerator PerformLunge()
    {
        isLunging = true;

        // Olha para o player
        Vector2 direction = (player.position - transform.position).normalized;

        // Tempo de preparação (pode adicionar animação aqui depois)
        yield return new WaitForSeconds(lungePrepareTime);

        // Dá o dash
        float dashDuration = 0.25f;
        float timer = 0f;

        while (timer < dashDuration)
        {
            rb.MovePosition(rb.position + direction * lungeSpeed * Time.fixedDeltaTime);
            timer += Time.fixedDeltaTime;
            yield return null;
        }

        // Causa dano se estiver perto do player após o dash
        if (Vector2.Distance(transform.position, player.position) < 2f)
        {
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
                playerStats.TakeDamage(damage);
        }

        lungeCooldownTimer = lungeCooldown;
        isLunging = false;
    }
}
