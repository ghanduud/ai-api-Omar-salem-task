using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance; // Singleton pattern
    private int totalEnemies;
    private int enemiesKilled = 0;

    public delegate void OnWin();
    public event OnWin OnPlayerWin;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CountEnemiesInScene();
    }

    private void CountEnemiesInScene()
    {
        totalEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length; // Count enemies in the scene
        Debug.Log($"Total Enemies: {totalEnemies}");
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
        Debug.Log($"Enemy killed! {enemiesKilled}/{totalEnemies}");

        if (enemiesKilled >= totalEnemies)
        {
            Debug.Log("🎉 All enemies defeated! You win!");
            OnPlayerWin?.Invoke(); // Notify win event
        }
    }
}
