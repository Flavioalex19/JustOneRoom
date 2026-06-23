using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Wave Settings")]
    public int currentLevel = 1;
    public int enemiesAlive = 0;
    public float timeBetweenWaves = 7f;

    private bool waveInProgress = false;
    private float waveTimer = 0f;

    [Header("Ui manager")]
    [SerializeField] UIManager uiManager;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        
    }
    void Update()
    {
        // Timer between waves
        if (!waveInProgress && enemiesAlive <= 0 && waveTimer > 0)
        {
            uiManager.ActivateTimer();
            uiManager.SetTextTimer((int)waveTimer);
            waveTimer -= Time.deltaTime;

            if (waveTimer <= 0)
            {
                uiManager.DeactivateTimer();
                StartNextWave();
            }
        }
    }

    /// <summary>
    /// Chamado pelo PoolManager quando spawna os inimigos da onda
    /// </summary>
    public void RegisterWaveSpawn(int amount)
    {
        enemiesAlive = amount;
        waveInProgress = true;
        Debug.Log("Onda " + currentLevel + " iniciada com " + enemiesAlive + " inimigos.");
    }

    /// <summary>
    /// Chamado quando um inimigo é derrotado
    /// </summary>
    public void EnemyDefeated()
    {
        enemiesAlive--;

        if (enemiesAlive <= 0 && waveInProgress)
        {
            waveInProgress = false;
            WaveCompleted();
        }
    }

    private void WaveCompleted()
    {
        Debug.Log("Onda " + currentLevel + " completada!");

        // Aumenta a quantidade de inimigos da próxima onda
        if (PoolManager.Instance != null)
        {
            PoolManager.Instance.IncreaseSpawnMax(2); // Aumenta 2 por onda (você pode mudar)
        }

        currentLevel++;
        waveTimer = timeBetweenWaves;

        Debug.Log("Próxima onda começa em " + timeBetweenWaves + " segundos...");
    }

    private void StartNextWave()
    {
        Debug.Log("Iniciando Onda " + currentLevel);

        if (PoolManager.Instance != null)
        {
            PoolManager.Instance.SpawnNewWave(); // Vamos criar esse método no PoolManager
        }
    }
}
