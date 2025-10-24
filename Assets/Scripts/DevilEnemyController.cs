using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]

public class DevilEnemyController : MonoBehaviour
{
    //Enemies variables
    [SerializeField] private float speed;

    [SerializeField] private float maxHealth;
    [SerializeField] private float health;
    [SerializeField] private float dracuPalleteDropProbability;
    private Rigidbody2D rb;

    //Player position
    private Transform player;

    private PlayerController playerController;

    //DracuPlallete prefab
    [SerializeField] private GameObject dracuPallete;

    // Attack animations
    [Header("Attack Animation Settings")]
    [SerializeField] private Animator animator; // Assign the enemy's Animator in the Inspector (optional)
    [SerializeField] private float attackInterval = 5f; // seconds between attacks
    private float attackTimer = 0f;
    private bool nextLeft = true; // alternate between left and right

    [Header("Facing Settings")]
    [SerializeField] private float facingSpriteOffset = 90f; // adjust depending on sprite art (up=90, right=0)

    // Special Attack: Tackle
    [Header("Special Attack: Tackle")]
    [SerializeField] private float tackleInterval = 10f; // seconds between tackles
    [SerializeField] private float backOffDuration = 1.0f; // charge back time (required 1s)
    [SerializeField] private float backOffDistance = 2.0f; // how far to move backward during charge
    [SerializeField] private float dashSpeed = 30f; // how fast to dash to the kept position
    [SerializeField] private string tackleStateName = "Tackle"; // Animator state name
    private float tackleTimer = 0f;
    private bool isPerformingTackle = false;

    //Life UI
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI currentHealthText; 

    void Awake()
    {
        //Organize values to the health bar slider
        health = maxHealth;
        healthBar.fillAmount = 1;
        currentHealthText.text = health + " / " + maxHealth; 


        player = GameObject.Find("Player").GetComponent<Transform>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Only chase when not tackling
        if (!isPerformingTackle)
        {
            FollowPlayer();
        }

        // Face the player (top-down 2D: rotate on Z axis only)
        if (player != null)
        {
            Vector2 toPlayer = player.position - transform.position;
            if (toPlayer.sqrMagnitude > 0.0001f)
            {
                float angle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
                rb.MoveRotation(angle + facingSpriteOffset);
            }
        }

        // Tackle timing
        if (!isPerformingTackle)
        {
            tackleTimer += Time.deltaTime;
            if (tackleTimer >= tackleInterval)
            {
                tackleTimer = 0f;
                StartCoroutine(TackleRoutine());
            }
        }

        // Alternate attacks every attackInterval seconds
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackInterval)
        {
            PlayAlternateAttack();
            attackTimer = 0f;
        }

        if (health <= 0)
        {
            Death();
        }

        if (health <= 3000)
        {
            tackleInterval /= 2;
            attackInterval /= 2; 
        }
    }

    void PlayAlternateAttack()
    {
        if (animator == null) return;

        // Ensure animation state names match Animator's Base Layer states
        string state = nextLeft ? "LeftHandPunch" : "RightHandPunch";
        // Play immediately from start (layer 0)
        animator.Play(state, 0, 0f);
        nextLeft = !nextLeft;
    }

    System.Collections.IEnumerator TackleRoutine()
    {
        if (player == null) yield break;

        isPerformingTackle = true;

        // Stop immediate movement
        rb.linearVelocity = Vector2.zero;

        // Snapshot the target position (player's current position)
        Vector2 keptTargetPos = player.position;

        // Play tackle animation if available
        if (animator != null && !string.IsNullOrEmpty(tackleStateName))
        {
            animator.Play(tackleStateName, 0, 0f);
        }

        // Determine direction away from player
        Vector2 startPos = rb.position;
        Vector2 awayDir = (startPos - (Vector2)player.position).normalized;
        if (awayDir.sqrMagnitude < 0.0001f)
        {
            // Fallback: use current facing direction rotated 90 degrees
            float z = rb.rotation * Mathf.Deg2Rad;
            awayDir = new Vector2(Mathf.Cos(z), Mathf.Sin(z));
        }

        // Charge back for backOffDuration, moving to a point behind
        Vector2 backTarget = startPos + awayDir * backOffDistance;
        float elapsed = 0f;
        while (elapsed < backOffDuration)
        {
            float t = elapsed / backOffDuration;
            Vector2 pos = Vector2.Lerp(startPos, backTarget, t);
            rb.MovePosition(pos);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        rb.MovePosition(backTarget);

        // Dash quickly to the kept player position
        const float stopDistance = 0.3f;
        Vector2 dashStartPos = rb.position;
        float maxDashDistance = Vector2.Distance(dashStartPos, keptTargetPos) + 1f; // Allow overshoot
        float dashedDistance = 0f;

        while (dashedDistance < maxDashDistance)
        {
            Vector2 dir = (keptTargetPos - rb.position).normalized;
            Vector2 step = dir * dashSpeed * Time.fixedDeltaTime;

            // Check if we'd overshoot
            float stepDist = step.magnitude;
            float distToTarget = Vector2.Distance(rb.position, keptTargetPos);

            if (distToTarget <= stopDistance || dashedDistance + stepDist >= maxDashDistance)
            {
                break; // Close enough or reached max distance
            }

            rb.MovePosition(rb.position + step);
            dashedDistance += stepDist;
            yield return new WaitForFixedUpdate();
        }

        // Clear velocity to prevent sticking
        rb.linearVelocity = Vector2.zero;

        isPerformingTackle = false;
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
        healthBar.fillAmount = health / maxHealth;
        currentHealthText.text = health + " / " + maxHealth; 

    }
}
