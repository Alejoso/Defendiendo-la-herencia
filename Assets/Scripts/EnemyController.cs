using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    //Enemies variables
    [SerializeField] private float speed;

    [SerializeField] private float maxHealth;
    [SerializeField] private float health;
    [SerializeField] private float dracuPalleteDropProbability;
    private float dolexProbabillity = 0.07f;
    private Rigidbody2D rb;

    //Player position
    private Transform player;

    private PlayerController playerController;

    //Pathfinding
    [Header("Pathfinding Settings")]
    [SerializeField] private LayerMask obstacleLayer; // Set this to your walls/obstacles layer
    [SerializeField] private float raycastDistance = 2f;
    [SerializeField] private float separationRadius = 1.0f; // Distance to keep from other enemies
    [SerializeField] private float separationForce = 2f; // Strength of separation
    [SerializeField] private int raycastAngles = 8; // Number of raycasts to check around
    
    // Stuck detection
    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    private float stuckCheckInterval = 0.5f;
    private float stuckThreshold = 0.2f; // How little movement means "stuck"
    private Vector2 unstuckDirection;
    private float unstuckForce = 0f;

    //DracuPlallete prefab
    [SerializeField] private GameObject dracuPallete;

    [SerializeField] private GameObject dolex;

    // Particles
    [SerializeField] private GameObject damageParticle;


    void Awake()
    {
        //Organize values to the health bar slider
        health = maxHealth;
        //healthBar.value = 1;

        player = GameObject.Find("Player").GetComponent<Transform>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        
        lastPosition = transform.position;
        unstuckDirection = Random.insideUnitCircle.normalized;
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
            TakeDamage(playerController.GetDamage());
            Destroy(collision.gameObject);
            Instantiate(
                damageParticle,
                transform.position,
                transform.rotation
            );
        }
        else if (collision.gameObject.CompareTag("MeleeHit"))
        {
            TakeDamage(playerController.GetDamage());
            //damageParticle.transform.rotation = transform.rotation; 
            Instantiate(
                damageParticle,
                transform.position,
                transform.rotation
            );
        }

    }

    //Script to follow the player with obstacle avoidance
    void FollowPlayer()
    {
        if (player == null) return;

        // Check if stuck and apply stronger force
        stuckTimer += Time.fixedDeltaTime;
        if (stuckTimer >= stuckCheckInterval)
        {
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);
            if (distanceMoved < stuckThreshold)
            {
                // Enemy is stuck - apply strong random force
                unstuckDirection = Random.insideUnitCircle.normalized;
                unstuckForce = 3f; // Strong initial push
            }
            else
            {
                unstuckForce *= 0.5f; // Decay unstuck force when moving normally
            }
            lastPosition = transform.position;
            stuckTimer = 0f;
        }

        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        
        // Add separation from other enemies
        Vector2 separationVector = CalculateSeparation();
        
        // Find the best clear direction using multiple raycasts
        Vector2 bestDirection = FindBestDirection(directionToPlayer);
        
        // Combine all movement influences
        Vector2 finalDirection = bestDirection;
        finalDirection += separationVector * 0.5f;
        finalDirection += unstuckDirection * unstuckForce;
        
        finalDirection = finalDirection.normalized;
        
        rb.linearVelocity = finalDirection * speed;
    }

    // Find the best direction by checking multiple angles
    Vector2 FindBestDirection(Vector2 targetDirection)
    {
        // First check if direct path is clear
        RaycastHit2D directHit = Physics2D.Raycast(transform.position, targetDirection, raycastDistance, obstacleLayer);
        
        if (directHit.collider == null)
        {
            return targetDirection; // Direct path is clear
        }

        // Direct path blocked - check multiple angles to find best path
        float bestScore = -1f;
        Vector2 bestDir = targetDirection;
        
        for (int i = 0; i < raycastAngles; i++)
        {
            float angle = (360f / raycastAngles) * i;
            Vector2 testDir = Rotate2D(targetDirection, angle);
            
            RaycastHit2D hit = Physics2D.Raycast(transform.position, testDir, raycastDistance, obstacleLayer);
            
            // Score based on: how clear the path is AND how close to target direction
            float clearDistance = hit.collider == null ? raycastDistance : hit.distance;
            float alignmentToTarget = Vector2.Dot(testDir, targetDirection);
            float score = clearDistance * 0.5f + alignmentToTarget * 2f;
            
            if (score > bestScore)
            {
                bestScore = score;
                bestDir = testDir;
            }
        }
        
        return bestDir;
    }

    // Rotate a 2D vector by degrees
    Vector2 Rotate2D(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }

    // Calculate separation force to avoid clustering with other enemies
    Vector2 CalculateSeparation()
    {
        Vector2 separation = Vector2.zero;
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, separationRadius);
        
        int count = 0;
        foreach (Collider2D other in nearbyEnemies)
        {
            if (other.gameObject != gameObject && other.CompareTag("Enemy"))
            {
                Vector2 awayFromOther = (transform.position - other.transform.position);
                float distance = awayFromOther.magnitude;
                
                if (distance > 0)
                {
                    // Closer enemies have stronger push
                    separation += awayFromOther.normalized / distance;
                    count++;
                }
            }
        }
        
        if (count > 0)
        {
            separation = (separation / count) * separationForce;
        }
        
        return separation;
    }

    void TakeDamage(int damage)
    {
        health -= damage;
        UpdateSlider();
    }

    void UpdateSlider()
    {
        //healthBar.value = health / maxHealth;
    }
}
