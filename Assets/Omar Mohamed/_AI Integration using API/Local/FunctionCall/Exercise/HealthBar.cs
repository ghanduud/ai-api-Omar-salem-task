using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Spaceship playerSpaceship;

    private void Start()
    {
        if (playerSpaceship != null)
        {
            playerSpaceship.OnHealthChanged += UpdateHealthBar; // Subscribe to health updates
            SetMaxHealth(playerSpaceship.maxHealth); // Initialize health bar
        }
        else
        {
            Debug.LogError("No Spaceship found! Make sure the player has the Spaceship script.");
        }
    }

    private void OnDestroy()
    {
        if (playerSpaceship != null)
        {
            playerSpaceship.OnHealthChanged -= UpdateHealthBar; // Unsubscribe to prevent memory leaks
        }
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    private void UpdateHealthBar(int health)
    {
        slider.value = health;
    }
}
