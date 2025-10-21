using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    //Enemies variables
    [SerializeField] private Slider healthBar;
    [SerializeField] private float speed;

    [SerializeField] private float maxHealth;
    [SerializeField] private float health;
    [SerializeField] private float dracuPalleteDropProbability;
    private Rigidbody2D rb;

    //Player position
    private Transform player;

    //DracuPlallete prefab
    [SerializeField] private GameObject dracuPallete;

    void Awake()
    {
        //Organize values to the health bar slider
        health = maxHealth;
        healthBar.value = 1;

        player = GameObject.Find("Player").GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FollowPlayer();

        if (health <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        if (Random.value <= dracuPalleteDropProbability)
            Instantiate(dracuPallete, transform.position, dracuPallete.transform.rotation);
        Destroy(gameObject);
    }

    //When it collides with a bullet do..
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            Destroy(collision.gameObject);
            TakeDamage(10);
        }
    }

    //Script to dumbly follow the player based on a vector that points to the player and some speed
    void FollowPlayer()
    {
        Vector3 positionToPlayer = player.position - transform.position;
        rb.linearVelocity = positionToPlayer.normalized * speed;
    }

    void TakeDamage(int damage)
    {
        health -= damage;
        UpdateSlider();
    }

    void UpdateSlider()
    {
        healthBar.value = health / maxHealth;
    }
}
