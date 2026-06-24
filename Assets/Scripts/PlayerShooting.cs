using UnityEngine;
public enum FireMode
{
    Single,
    Auto,
    ThreeRound
}
public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;              // Crie um Empty Child no player para spawnar o tiro
    public float bulletSpeed = 12f;

    [Header("Stats")]
    public int currentAmmo = 30;
    public int maxAmmo = 30;                 // CurrentMax ammo (pode aumentar durante a run)
    public int damage = 10;
    public bool CanReload = false;

    [Header("Fire Rate")]
    public float fireRate = 0.25f;           // Tempo entre tiros (menor = mais r�pido)
    public FireMode currentFireMode = FireMode.Single;

    private float nextFireTime = 0f;
    private bool isBursting = false;
    private int burstShotsFired = 0;

    [Header("Others")]
    [SerializeField] UIManager uiManager;
    private void Start()
    {
        uiManager.playerGunAmmoStats(currentAmmo);
        uiManager.PlayerGunFireType(currentFireMode.ToString());
    }
    void Update()
    {
        if (!GameManager.Instance.gameHasStarted) return;
        if (GameManager.Instance.isChoosingUpgrade) return;
        // Shooting input depending on fire mode
        if (Time.time >= nextFireTime && currentAmmo > 0)
        {
            switch (currentFireMode)
            {
                case FireMode.Single:
                    if (Input.GetMouseButtonDown(0))
                        Shoot();
                    break;

                case FireMode.Auto:
                    if (Input.GetMouseButton(0))
                        Shoot();
                    break;

                case FireMode.ThreeRound:
                    if (Input.GetMouseButtonDown(0) && !isBursting)
                        StartBurst();
                    break;
            }
        }

        // Handle burst firing
        if (isBursting)
        {
            HandleBurstFire();
        }
        if(CanReload == true)
        {
            if (Input.GetKeyDown(KeyCode.R)) RefillAmmo();
        }
        
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        Vector2 shootDirection = (mousePosition - transform.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Define owner
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.owner = this.gameObject;  
            bulletScript.damage = damage;
        }

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = shootDirection * bulletSpeed;
        }

        currentAmmo--;
        uiManager.playerGunAmmoStats(currentAmmo);
        nextFireTime = Time.time + fireRate;

        Debug.Log("Shot fired! Ammo left: " + currentAmmo);
    }

    private void StartBurst()
    {
        isBursting = true;
        burstShotsFired = 0;
    }

    private void HandleBurstFire()
    {
        if (burstShotsFired < 3 && Time.time >= nextFireTime)
        {
            Shoot();
            burstShotsFired++;

            if (burstShotsFired >= 3)
            {
                isBursting = false;
                nextFireTime = Time.time + fireRate; // Apply normal cooldown after burst
            }
            else
            {
                nextFireTime = Time.time + 0.08f; // Small delay between burst shots
            }
        }
    }

    // ==================== HELPER FUNCTIONS ====================

    /// <summary>
    /// Changes the current fire mode.
    /// </summary>
    public void SetFireMode(FireMode newMode)
    {
        currentFireMode = newMode;
        isBursting = false;
        Debug.Log("Fire mode changed to: " + newMode);
    }

    /// <summary>
    /// Increases the maximum ammo (used during the run).
    /// </summary>
    public void AddMaxAmmo(int amount)
    {
        maxAmmo += amount;
        currentAmmo = Mathf.Min(currentAmmo + amount, maxAmmo);
        Debug.Log("Max ammo increased! New max: " + maxAmmo);
    }

    /// <summary>
    /// Resets max ammo back to a specific value.
    /// </summary>
    public void ResetMaxAmmo(int newMax)
    {
        maxAmmo = newMax;
        currentAmmo = Mathf.Min(currentAmmo, maxAmmo);
        Debug.Log("Max ammo reset to: " + maxAmmo);
    }

    /// <summary>
    /// Resets fire rate to a new value.
    /// </summary>
    public void ResetFireRate(float newFireRate)
    {
        fireRate = newFireRate;
        Debug.Log("Fire rate reset to: " + fireRate);
    }

    /// <summary>
    /// Refills current ammo to maximum.
    /// </summary>
    public void RefillAmmo()
    {
        currentAmmo = maxAmmo;
        Debug.Log("Ammo refilled!");
    }
    public void IncreaseMaxAmmo(int amount)
    {
        maxAmmo += amount;
        currentAmmo = Mathf.Min(currentAmmo + amount, maxAmmo);
        Debug.Log("Max Ammo aumentado para: " + maxAmmo);
    }

    // Método para aumentar Fire Rate
    public void IncreaseFireRate(float amount)
    {
        fireRate = Mathf.Max(0.05f, fireRate - amount); // quanto menor, mais rápido
        Debug.Log("Fire Rate melhorado para: " + fireRate);
    }

    // Método para aumentar Dano
    public void IncreaseDamage(int amount)
    {
        damage += amount;
        Debug.Log("Damage aumentado para: " + damage);
    }

    // Método para habilitar Reload
    public void EnableReload()
    {
        CanReload = true;
        Debug.Log("Reload HABILITADO!");
    }
    


}
