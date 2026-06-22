using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] int maxHP = 100;              
    [SerializeField]private int currentHP = 0;

    [Header("Invulnerability")]
    public float invulnerabilityDuration = 3f;
    private bool isInvulnerable = false;

    void Start()
    {
        currentHP = maxHP;
    }

    //Player takes damage
    public void TakeDamage(int damageAmount)
    {
        if (isInvulnerable || damageAmount <= 0) return;

        currentHP -= damageAmount;
        if (currentHP < 0) currentHP = 0;

        Debug.Log("Player took " + damageAmount + " damage! Current HP: " + currentHP);

        // Ativa invulnerabilidade
        ActivateInvulnerability();

        if (currentHP <= 0)
        {
            Die();
        }
    }
    private void ActivateInvulnerability()
    {
        isInvulnerable = true;
        Debug.Log("Player is now INVULNERABLE for " + invulnerabilityDuration + " seconds");

        // Desativa depois de X segundos
        Invoke(nameof(DeactivateInvulnerability), invulnerabilityDuration);
    }
    
    private void DeactivateInvulnerability()
    {
        isInvulnerable = false;
        Debug.Log("Player is no longer invulnerable");
    }
    public int GetCurrentHP() => currentHP;
    public int GetMaxHP() => maxHP;

    private void Die()
    {
        Debug.Log("Player has died!");
        // TODO: Add death logic later (disable movement, play animation, restart, etc.)
    }
}
