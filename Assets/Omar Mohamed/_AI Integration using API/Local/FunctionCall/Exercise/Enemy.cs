using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject enemyBulletPrefab; // Assign the EnemyBullet prefab in Unity
    public Transform bulletSpawnPoint; // Assign a bullet spawn point in Unity
    public float minShootInterval = 3f; // Minimum time between shots
    public float maxShootInterval = 7f; // Maximum time between shots

    private void Start()
    {
        ScheduleNextShot(); // Start shooting at a random interval
    }

    private void Shoot()
    {
        if (enemyBulletPrefab != null && bulletSpawnPoint != null)
        {
            GameObject bullet = Instantiate(enemyBulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            Destroy(bullet, 3f); // Destroy bullet after 3 seconds
        }
        else
        {
            Debug.LogError("EnemyBulletPrefab or BulletSpawnPoint is not assigned!");
        }

        // Schedule the next shot at a random interval
        ScheduleNextShot();
    }

    private void ScheduleNextShot()
    {
        float randomTime = Random.Range(minShootInterval, maxShootInterval); // Get random time between 3-7 seconds
        Invoke(nameof(Shoot), randomTime); // Call Shoot after a random delay
    }

    private void OnDestroy()
    {
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.EnemyKilled();
        }
    }
}
