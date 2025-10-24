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

    //Pathfinding
    [Header("Pathfinding Settings")]
    [SerializeField] private float obstacleAvoidanceRadius = 1.5f;
    [SerializeField] private LayerMask obstacleLayer; // Set this to your walls/obstacles layer
    [SerializeField] private float raycastDistance = 2f;

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
        }
        else
        {
            if (Random.value <= dolexProbabillity)
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

    //Script to follow the player with obstacle avoidance
    void FollowPlayer()
    {
        if (player == null) return;

        Vector2 directionToPlayer = (player.position - transform.position).normalized;

        // Check for obstacles in the path using raycasts
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, raycastDistance, obstacleLayer);

        if (hit.collider != null)
        {
            // Obstacle detected - try to move around it
            print("Obstacle detected: " + hit.collider.name);
            Vector2 avoidanceDirection = GetAvoidanceDirection(directionToPlayer, hit.normal);
            rb.linearVelocity = avoidanceDirection * speed;
        }
        else
        {
            // No obstacle - move directly toward player
            rb.linearVelocity = directionToPlayer * speed;
        }
    }

    // Calculate direction to avoid obstacle
    Vector2 GetAvoidanceDirection(Vector2 targetDirection, Vector2 obstacleNormal)
    {
        // Try moving perpendicular to the obstacle
        Vector2 right = Vector2.Perpendicular(obstacleNormal);
        Vector2 left = -right;

        // Choose the direction that's closer to the target
        float rightDot = Vector2.Dot(right, targetDirection);
        float leftDot = Vector2.Dot(left, targetDirection);

        Vector2 avoidDir = (rightDot > leftDot) ? right : left;

        // Blend with target direction for smoother movement
        return (avoidDir + targetDirection * 0.5f).normalized;
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
