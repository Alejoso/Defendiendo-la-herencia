using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private float speed;
    
    [SerializeField] private float maxHealth;
    [SerializeField] private float health;

    private Rigidbody2D rb; 
    private Transform player;

    void Awake()
    {
        //Organize values to the health bar slider
        health = maxHealth;
        healthBar.value = 1; 

        player = GameObject.Find("Player").GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>(); 
    }

    // Update is called once per frame
    void Update()
    {
        FollowPlayer();

        if (health <= 0)
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
        rb.linearVelocity = positionToPlayer.normalized * speed * Time.deltaTime;
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
