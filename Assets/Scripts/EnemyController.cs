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
    private float dolexProbabillity = 0.06f; 
    private Rigidbody2D rb;

    //Player position
    private Transform player;

    private PlayerController playerController;

    //DracuPlallete prefab
    [SerializeField] private GameObject dracuPallete;

    [SerializeField] private GameObject dolex; 

    void Awake()
    {
        //Organize values to the health bar slider
        health = maxHealth;
        healthBar.value = 1;

        player = GameObject.Find("Player").GetComponent<Transform>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
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
        {
             Instantiate(dracuPallete, transform.position, dracuPallete.transform.rotation);
        } else
        {
            if(Random.value <= dolexProbabillity)
            {
                Instantiate(dolex, transform.position, dolex.transform.rotation);
            }
        }
           
        Destroy(gameObject);
    }

    //When it collides with a bullet do..
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            Destroy(collision.gameObject);
            TakeDamage(playerController.GetDamage());
        }
        else if (collision.gameObject.CompareTag("MeleeHit"))
        {
            TakeDamage(playerController.GetDamage());
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
