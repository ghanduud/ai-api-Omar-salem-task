using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f; // Bullet lifetime before getting destroyed

    void Start()
    {
        Destroy(gameObject, lifetime); // Destroy bullet after 5 seconds
    }

    void Update()
    {
        transform.position += Vector3.down * speed * Time.deltaTime; // Move downward
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Check if it hits the player
        {
            Spaceship player = collision.GetComponent<Spaceship>(); // Get player script
            if (player != null)
            {
                int damage = Random.Range(5, 11); // Random damage between 10 and 20
                player.TakeDamage(damage); // Apply damage to player
                Debug.Log($"Enemy bullet hit player! Damage: {damage}");
            }

            Destroy(gameObject); // Destroy bullet after hitting player
        }
    }
}
