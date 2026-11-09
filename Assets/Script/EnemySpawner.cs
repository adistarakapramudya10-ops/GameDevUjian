using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab; 
    [SerializeField] private float spawnInterval = 3f; 
    [SerializeField] private float spawnRadius = 3f;  
    [SerializeField] private int maxEnemies = 3; // Batas maksimal musuh di arena

    void Start()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab belum di-assign di Inspector!");
            return;
        }
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        // Beri jeda 1 detik di awal game agar scene siap dulu
        yield return new WaitForSeconds(1f);

        while (true)
        {
            // Menghitung jumlah musuh aktif langsung berdasarkan script Enemy di scene
            Enemy[] activeEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            int currentEnemyCount = activeEnemies.Length;

            // Jika jumlah musuh kurang dari batas maksimal, lakukan spawn
            if (currentEnemyCount < maxEnemies)
            {
                SpawnEnemy();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy()
    {
        Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}