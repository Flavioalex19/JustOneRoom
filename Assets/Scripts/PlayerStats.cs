using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] int maxHP = 100;              
    [SerializeField]private int currentHP = 0;
    [Header("XP System")]
    public int currentXP = 0;
    public int xpToNextLevel = 100;
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
    public void AddXP(int amount)
    {
        if (amount <= 0) return;

        currentXP += amount;
        Debug.Log("Ganhou " + amount + " XP! Total: " + currentXP);

        // TODO: Verificar se subiu de nível (podemos fazer depois)
        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        // Por enquanto só reseta o XP (depois podemos melhorar)
        currentXP -= xpToNextLevel;
        Debug.Log("LEVEL UP! Novo nível!");
        // TODO: Aumentar stats do player aqui no futuro
    }

    public int GetCurrentXP() => currentXP;
    private void Die()
    {
        Debug.Log("Player has died!");
        // TODO: Add death logic later (disable movement, play animation, restart, etc.)
    }
}
