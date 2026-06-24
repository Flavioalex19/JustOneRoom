using System.Collections.Generic;
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
    private bool isGamePaused = false;
    [Header("Upgrade Selection")]
    public bool isChoosingUpgrade = false;
    [Header("Game Start")]
    public bool gameHasStarted = false;
    [Header("Intro / Title Screen")]
    public bool isIntroActive = true;           
    public GameObject introPanel;

    [Header("Ui manager")]
    [SerializeField] UIManager uiManager;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (uiManager != null)
            uiManager.SetWaveText(currentLevel);
    }

    void Update()
    {
        HandleGameStartInput();

        // Timer entre ondas (só funciona depois que o jogo começou)
        if (gameHasStarted && !waveInProgress && enemiesAlive <= 0 && waveTimer > 0 && !isGamePaused)
        {
            if (uiManager != null)
            {
                uiManager.ActivateTimer();
                uiManager.SetTextTimer(Mathf.CeilToInt(waveTimer));
            }

            waveTimer -= Time.deltaTime;

            if (waveTimer <= 0)
            {
                if (uiManager != null) uiManager.DeactivateTimer();
                StartNextWave();
            }
        }
    }

    private void HandleGameStartInput()
    {
        if (!gameHasStarted && Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        gameHasStarted = true;
        isIntroActive = false;

        if (introPanel != null)
            introPanel.SetActive(false);

        Debug.Log("Jogo iniciado!");

        // Spawna a primeira onda
        if (PoolManager.Instance != null)
        {
            PoolManager.Instance.SpawnInitialEnemies();
        }
    }

    public void RegisterWaveSpawn(int amount)
    {
        enemiesAlive = amount;
        waveInProgress = true;
        Debug.Log($"Onda {currentLevel} iniciada com {enemiesAlive} inimigos.");
    }

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
        Debug.Log($"Onda {currentLevel} completada!");

        if (PoolManager.Instance != null)
            PoolManager.Instance.IncreaseSpawnMax(2);

        currentLevel++;

        isChoosingUpgrade = true;                    
        isGamePaused = true;

        if (uiManager != null)
            uiManager.ShowUpgradeScreen();
    }

    public void StartWaveTimer()
    {
        waveTimer = timeBetweenWaves;
        isGamePaused = false;
        isChoosingUpgrade = false;                   

        if (uiManager != null)
            uiManager.ActivateTimer();
    }

    private void StartNextWave()
    {
        Debug.Log($"Iniciando Onda {currentLevel}");

        // Recarrega munição do player
        PlayerShooting playerShooting = FindFirstObjectByType<PlayerShooting>();
        if (playerShooting != null)
            playerShooting.RefillAmmo();

        if (PoolManager.Instance != null)
            PoolManager.Instance.SpawnNewWave();
    }
}
