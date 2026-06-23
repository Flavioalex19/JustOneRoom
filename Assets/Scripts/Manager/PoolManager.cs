using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    [Header("Enemy Pool")]
    public Transform enemyPoolParent;          

    [Header("Spawn Points")]
    public Transform spawnPointsParent;

    [Header("Spawn Control")]
    public int spawnMaxRestart = 5;    
    public int spawnMax = 5;

    private List<GameObject> enemyPool = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        InitializeEnemyPool();
        SpawnInitialEnemies();
    }

    private void InitializeEnemyPool()
    {
        enemyPool.Clear();

        foreach (Transform child in enemyPoolParent)
        {
            GameObject enemy = child.gameObject;
            enemy.SetActive(false);
            enemyPool.Add(enemy);
        }

        Debug.Log("Pool inicializado com " + enemyPool.Count + " inimigos.");
    }

    private void SpawnInitialEnemies()
    {
        int spawnCount = Mathf.Min(spawnMax, enemyPool.Count, spawnPointsParent.childCount);
        int spawned = 0;

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject enemy = GetEnemyFromPool();

            if (enemy != null)
            {
                Transform spawnPoint = spawnPointsParent.GetChild(i);
                enemy.transform.position = spawnPoint.position;
                enemy.SetActive(true);

                EnemyController enemyScript = enemy.GetComponent<EnemyController>();
                if (enemyScript != null)
                    enemyScript.ResetEnemy();

                spawned++;
            }
        }

        // === NOVO: Registra a primeira onda no GameManager ===
        if (GameManager.Instance != null && spawned > 0)
        {
            GameManager.Instance.RegisterWaveSpawn(spawned);
        }

        Debug.Log("Primeira onda spawnada com " + spawned + " inimigos.");
    }


    public void IncreaseSpawnMax(int amount)
    {
        spawnMax += amount;
        Debug.Log("SpawnMax aumentado! Novo valor: " + spawnMax);
    }


    public void ResetSpawnMax()
    {
        spawnMax = spawnMaxRestart;
        Debug.Log("SpawnMax resetado para: " + spawnMax);
    }


    public GameObject GetEnemyFromPool()
    {
        foreach (GameObject enemy in enemyPool)
        {
            if (!enemy.activeInHierarchy)
                return enemy;
        }
        return null;
    }

    public void ReturnEnemyToPool(GameObject enemy)
    {
        enemy.SetActive(false);
        enemy.transform.SetParent(enemyPoolParent);
    }
    public void SpawnNewWave()
    {
        int spawnCount = Mathf.Min(spawnMax, enemyPool.Count, spawnPointsParent.childCount);

        int spawned = 0;

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject enemy = GetEnemyFromPool();

            if (enemy != null)
            {
                Transform spawnPoint = spawnPointsParent.GetChild(i);
                enemy.transform.position = spawnPoint.position;
                enemy.SetActive(true);

                EnemyController enemyScript = enemy.GetComponent<EnemyController>();
                if (enemyScript != null)
                    enemyScript.ResetEnemy();

                spawned++;
            }
        }

        // Avisa o GameManager quantos inimigos foram spawnados
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterWaveSpawn(spawned);
        }

        Debug.Log("Nova onda spawnada com " + spawned + " inimigos.");
    }
}
