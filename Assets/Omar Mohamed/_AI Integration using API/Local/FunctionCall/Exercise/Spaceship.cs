using System;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public GameObject shieldObject; // Assign in Unity
    public float moveSpeed = 5f;
    
    public int maxHealth = 100;
    private int currentHealth;
    private bool shieldActive = false;

    private float leftBoundary;
    private float rightBoundary;

        // Event to notify when health changes
    public event Action<int> OnHealthChanged;

    private void Start()
    {
        shieldObject.SetActive(false); // Start with shield off
        currentHealth = maxHealth; // Initialize health
        SetBoundaries(); // Calculate camera boundaries

                // Notify UI about initial health
        OnHealthChanged?.Invoke(currentHealth);
    }

    private void Update()
    {
        // Continuously update boundaries (useful if the camera moves or resizes dynamically)
        SetBoundaries();
    }

    private void SetBoundaries()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("No Main Camera found! Ensure your camera is tagged as 'MainCamera'.");
            return;
        }

        float halfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        leftBoundary = mainCamera.transform.position.x - halfWidth + 0.5f; // Add small padding
        rightBoundary = mainCamera.transform.position.x + halfWidth - 0.5f; // Add small padding
    }

    public void MoveRight()
    {
        float newX = transform.position.x + moveSpeed;
        if (newX < rightBoundary)
        {
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
    }

    public void MoveLeft()
    {
        float newX = transform.position.x - moveSpeed;
        if (newX > leftBoundary)
        {
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
    }

    public void Shoot()
    {
        Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
    }

    public void ActivateShield()
    {
        if (!shieldActive)
        {
            shieldActive = true;
            shieldObject.SetActive(true);
            Invoke(nameof(DeactivateShield), 3f); // ⏳ Auto disable after 3 seconds
        }
    }

    public void DeactivateShield()
    {
        shieldActive = false;
        shieldObject.SetActive(false);
    }

    // 🚀 Take Damage Function
    public void TakeDamage(int damage)
    {
        if (shieldActive)
        {
            Debug.Log("Shield is active! No damage taken.");
            return;
        }

        currentHealth -= damage;
        Debug.Log($"Took {damage} damage! Health: {currentHealth}/{maxHealth}");
        OnHealthChanged?.Invoke(currentHealth); // Notify UI

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 🚀 Heal Function (Without exceeding maxHealth)
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth; // Prevent overhealing
        }
        Debug.Log($"Healed {healAmount}! Health: {currentHealth}/{maxHealth}");
        OnHealthChanged?.Invoke(currentHealth); // Notify UI
    }

    // 🚀 Destroy Ship when Health Reaches 0
    private void Die()
    {
        Debug.Log("Spaceship destroyed!");
        Destroy(gameObject);
    }
}
