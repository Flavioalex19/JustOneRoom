using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public List<UpgradeType> availableUpgrades = new List<UpgradeType>();
    private bool reloadEnabled = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        InitializeUpgradePool();
    }

    private void InitializeUpgradePool()
    {
        availableUpgrades = new List<UpgradeType>
        {
            UpgradeType.MoreAmmo,
            UpgradeType.FasterFireRate,
            UpgradeType.MoreDamage,
            UpgradeType.MoreMaxHP,
            UpgradeType.EnableReload
        };
    }

    public List<UpgradeType> GetRandomUpgrades(int count = 3)
    {
        List<UpgradeType> pool = new List<UpgradeType>(availableUpgrades);

        if (reloadEnabled)
            pool.Remove(UpgradeType.EnableReload);

        // Embaralha
        for (int i = 0; i < pool.Count; i++)
        {
            int rand = Random.Range(i, pool.Count);
            (pool[i], pool[rand]) = (pool[rand], pool[i]);
        }

        List<UpgradeType> result = new List<UpgradeType>();
        for (int i = 0; i < count && i < pool.Count; i++)
            result.Add(pool[i]);

        return result;
    }

    public void ApplyUpgrade(UpgradeType upgrade)
    {
        PlayerShooting shooting = FindFirstObjectByType<PlayerShooting>();
        PlayerStats stats = FindFirstObjectByType<PlayerStats>();
        PlayerMovement movement = FindFirstObjectByType<PlayerMovement>();

        switch (upgrade)
        {
            case UpgradeType.MoreAmmo:
                if (shooting) shooting.IncreaseMaxAmmo(10);
                break;

            case UpgradeType.FasterFireRate:
                if (shooting) shooting.IncreaseFireRate(0.05f);
                break;

            case UpgradeType.MoreDamage:
                if (shooting) shooting.IncreaseDamage(5);
                break;

            case UpgradeType.MoreMaxHP:
                if (stats) stats.IncreaseMaxHP(20);
                break;

            case UpgradeType.EnableReload:
                if (shooting) shooting.EnableReload();
                reloadEnabled = true;
                availableUpgrades.Remove(UpgradeType.EnableReload);
                break;
            case UpgradeType.IncreaseMaxAmmo:
                if (shooting) shooting.IncreaseMaxAmmo(15);
                break;

            case UpgradeType.IncreasePlayerSpeed:
                if (movement) movement.IncreaseMoveSpeed(1.5f);
                break;
        }

        Debug.Log("Upgrade aplicado: " + upgrade);
    }
}
