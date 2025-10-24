using UnityEngine;

public class LloronaEnemyController : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private CircleCollider2D attackCollider;
    [SerializeField] private ParticleSystem attackParticles;
    [SerializeField] private float attackInterval = 10f; // time between attacks
    [SerializeField] private float attackDuration = 3f; // how long attack lasts

    [Header("Facing Settings")]
    [SerializeField] private float facingSpriteOffset = 90f; // adjust depending on sprite art (up=90, right=0)

    private EnemyController enemyController;
    private Rigidbody2D rb;
    private Transform player;
    private float attackTimer = 0f;
    private bool isAttacking = false;
    private float savedSpeed = 0f;

    void Awake()
    {
        // Get reference to EnemyController on same GameObject
        enemyController = GetComponent<EnemyController>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player")?.GetComponent<Transform>();

        if (enemyController == null)
        {
            Debug.LogError("LloronaEnemyController: EnemyController not found on same GameObject!");
        }

        if (rb == null)
        {
            Debug.LogError("LloronaEnemyController: Rigidbody2D not found!");
        }

        if (player == null)
        {
            Debug.LogError("LloronaEnemyController: Player not found!");
        }

        // Disable attack collider initially
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }

        // Stop particle system initially
        if (attackParticles != null)
        {
            attackParticles.Stop();
        }
    }

    void Update()
    {
        // Face the player (top-down 2D: rotate on Z axis only)
        if (player != null && rb != null)
        {
            Vector2 toPlayer = player.position - transform.position;
            if (toPlayer.sqrMagnitude > 0.0001f)
            {
                float angle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
                rb.MoveRotation(angle + facingSpriteOffset);
            }
        }

        // Attack timer
        if (!isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackInterval)
            {
                attackTimer = 0f;
                StartCoroutine(Attack());
            }
        }
    }

    System.Collections.IEnumerator Attack()
    {
        isAttacking = true;

        // Save current speed and set to 0 in EnemyController
        if (enemyController != null)
        {
            savedSpeed = GetEnemySpeed();
            SetEnemySpeed(0f);
        }

        // Enable attack collider
        if (attackCollider != null)
        {
            attackCollider.enabled = true;
        }

        // Play particle system
        if (attackParticles != null)
        {
            attackParticles.Play();
        }

        // Wait for attack duration
        yield return new WaitForSeconds(attackDuration / 2);

        if (attackCollider != null)
        {
            attackCollider.enabled = false;
            attackCollider.enabled = true;
        }
        yield return new WaitForSeconds(attackDuration / 2);

        // Disable attack collider
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }

        // Stop particle system
        if (attackParticles != null)
        {
            attackParticles.Stop();
        }

        // Restore speed in EnemyController
        if (enemyController != null)
        {
            SetEnemySpeed(savedSpeed);
        }

        isAttacking = false;
    }

    // Helper to get speed using reflection since it's private
    private float GetEnemySpeed()
    {
        var speedField = typeof(EnemyController).GetField("speed",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (speedField != null)
        {
            return (float)speedField.GetValue(enemyController);
        }
        return 0f;
    }

    // Helper to set speed using reflection since it's private
    private void SetEnemySpeed(float value)
    {
        var speedField = typeof(EnemyController).GetField("speed",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (speedField != null)
        {
            speedField.SetValue(enemyController, value);
        }
    }
}
