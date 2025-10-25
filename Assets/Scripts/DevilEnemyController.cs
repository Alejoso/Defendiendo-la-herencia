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
    private bool hasNotChanged = false;

    [Header("Special Attack: Laser")]
    [SerializeField] private GameObject laserObject; // object containing laserShoot and visuals (should be inactive by default)
    [SerializeField] private float laserInterval = 12f; // seconds between laser attacks
    [SerializeField] private float laserDuration = 3f; // how long laser stays active
    [SerializeField] private float laserDamage = 10f; // damage applied per tick
    [SerializeField] private float laserDamageTickInterval = 0.5f; // how often damage is applied while laser hits player
    private float laserTimer = 0f;
    private bool isPerformingLaser = false;
    // Camera shake during laser
    [Header("Camera Shake")]
    [SerializeField] private float laserShakeIntensity = 0.15f;

    //Life UI
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI currentHealthText;

    [Header("Death Effects")]
    [SerializeField] private ParticleSystem bloodParticleSystem;
    [SerializeField] private float deathShakeIntensity = 0.2f;
    [SerializeField] private float deathShakeDuration = 5f;
    private bool isDying = false;

    private DevilEnemyController devilEnemyController;

    private GameProgression gameProgression;

    // Camera references for shaking
    private Camera mainCamera;
    private MonoBehaviour cameraController;

    [SerializeField] private GameObject damageParticle;

    void Awake()
    {
        //Organize values to the health bar slider
        health = maxHealth;
        healthBar.fillAmount = 1;
        currentHealthText.text = health + " / " + maxHealth;


        player = GameObject.Find("Player").GetComponent<Transform>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();

        devilEnemyController = GetComponent<DevilEnemyController>();

        gameProgression = GameObject.Find("GameController").GetComponent<GameProgression>();

        // Ensure laser object is disabled at start (if assigned)
        if (laserObject != null)
            laserObject.SetActive(false);

        // Cache main camera and its CameraController (if present) so we can shake it during attacks
        if (Camera.main != null)
        {
            mainCamera = Camera.main;
            cameraController = mainCamera.GetComponent("CameraController") as MonoBehaviour;
            if (cameraController == null)
            {
                // It's fine if there's no CameraController; we'll still do a simple shake.
                // Debug.Log("No CameraController found on main camera — shake will proceed without disabling controller.");
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Only chase when not tackling (movement allowed during laser)
        if (!isPerformingTackle)
        {
            FollowPlayer();
        }

        // Face the player (top-down 2D: rotate on Z axis only)
        if (player != null && !isPerformingTackle && !isPerformingLaser)
        {
            Vector2 toPlayer = player.position - transform.position;
            if (toPlayer.sqrMagnitude > 0.0001f)
            {
                float angle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
                rb.MoveRotation(angle + facingSpriteOffset);
            }
        }

        // Tackle timing (don't start while performing laser)
        if (!isPerformingTackle && !isPerformingLaser)
        {
            tackleTimer += Time.deltaTime;
            if (tackleTimer >= tackleInterval)
            {
                tackleTimer = 0f;
                StartCoroutine(TackleRoutine());
            }
        }

        // Laser timing (run only when not tackling or already performing laser)
        if (!isPerformingTackle && !isPerformingLaser)
        {
            laserTimer += Time.deltaTime;
            if ((laserTimer >= laserInterval) && health <= maxHealth / 2)
            {
                laserTimer = 0f;
                StartCoroutine(LaserRoutine());
            }
        }

        // Alternate attacks every attackInterval seconds (paused during special attacks)
        if (!isPerformingTackle && !isPerformingLaser)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackInterval)
            {
                PlayAlternateAttack();
                attackTimer = 0f;
            }
        }

        if (health <= 0)
        {
            Death();
        }

        if (health <= 30000 && !hasNotChanged)
        {
            hasNotChanged = true;
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

    System.Collections.IEnumerator LaserRoutine()
    {
        if (laserObject == null || player == null) yield break;

        // Try to get the laserShoot component for precise ray info
        var laserComp = laserObject.GetComponent<laserShoot>();

        isPerformingLaser = true;

        // Activate visuals/effects
        laserObject.SetActive(true);

        // Start camera shake coroutine in parallel with the laser
        StartCoroutine(ShakeCameraDuringLaser(laserDuration));

        float elapsed = 0f;
        float tick = 0f;

        // During the laser duration, periodically raycast along the laser and apply damage ticks when hitting the player
        while (elapsed < laserDuration)
        {
            elapsed += Time.deltaTime;
            tick += Time.deltaTime;

            if (laserComp != null)
            {
                Vector2 origin = laserComp.GetOrigin();
                Vector2 dir = laserComp.GetWorldDirection();
                float maxDist = laserComp.GetMaxDistance();

                RaycastHit2D hit = Physics2D.Raycast(origin, dir, maxDist);
                if (hit.collider != null && hit.collider.CompareTag("Player") && tick >= laserDamageTickInterval)
                {
                    // Inflict damage on player via public API
                    if (playerController != null)
                        playerController.ReceiveDamage(laserDamage);
                    tick = 0f;
                }
            }
            else
            {
                // Fallback: simple ray from enemy to player
                Vector2 origin = rb.position;
                Vector2 dirToPlayer = ((Vector2)player.position - origin).normalized;
                float dist = Vector2.Distance(origin, player.position);
                RaycastHit2D hit = Physics2D.Raycast(origin, dirToPlayer, dist);
                if (hit.collider != null && hit.collider.CompareTag("Player") && tick >= laserDamageTickInterval)
                {
                    if (playerController != null)
                        playerController.ReceiveDamage(laserDamage);
                    tick = 0f;
                }
            }

            yield return null;
        }

        // Deactivate visuals/effects
        laserObject.SetActive(false);

        // Allow behavior again
        isPerformingLaser = false;
    }

    // Shake the main camera for the duration of the laser. Disables CameraController if present to avoid conflicts.
    private System.Collections.IEnumerator ShakeCameraDuringLaser(float duration)
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) yield break;
            if (cameraController == null)
                cameraController = mainCamera.GetComponent("CameraController") as MonoBehaviour;
        }

        // Optionally disable CameraController while we shake so it doesn't overwrite positions
        if (cameraController != null)
            cameraController.enabled = false;

        Vector3 originalPosition = mainCamera.transform.localPosition;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * laserShakeIntensity;
            float y = Random.Range(-1f, 1f) * laserShakeIntensity;
            mainCamera.transform.localPosition = mainCamera.transform.localPosition + new Vector3(x, y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Restore
        if (mainCamera != null)
            mainCamera.transform.localPosition = originalPosition;

        if (cameraController != null)
            cameraController.enabled = true;
    }

    void Death()
    {
        if (isDying) return; // Prevent multiple calls
        isDying = true;

        health = 0;
        UpdateSlider();

        // Stop all movement
        rb.linearVelocity = Vector2.zero;
        speed = 0f;
        isPerformingTackle = true; // Prevent further actions

        // Play blood particle effect
        if (bloodParticleSystem != null)
        {
            bloodParticleSystem.Play();
        }

        devilEnemyController.enabled = false;

        // Drop loot
        if (Random.value <= dracuPalleteDropProbability)
            Instantiate(dracuPallete, transform.position, dracuPallete.transform.rotation);

        // Start death sequence with shake
        StartCoroutine(DeathSequence());
    }

    System.Collections.IEnumerator DeathSequence()
    {
        Vector3 originalPosition = transform.position;
        float elapsed = 0f;

        // Shake for deathShakeDuration (5 seconds)
        while (elapsed < deathShakeDuration)
        {
            float x = Random.Range(-1f, 1f) * deathShakeIntensity;
            float y = Random.Range(-1f, 1f) * deathShakeIntensity;
            transform.position = originalPosition + new Vector3(x, y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Return to original position
        transform.position = originalPosition;

        // Load WinScene
        gameProgression.LoadWinScene();
    }

    //When it collides with a bullet do..
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            Destroy(collision.gameObject);
            TakeDamage(playerController.GetDamage());
            Instantiate(
                damageParticle,
                transform.position,
                transform.rotation
            );
        }
        else if (collision.gameObject.CompareTag("MeleeHit"))
        {
            TakeDamage(playerController.GetDamage());
            Instantiate(
                damageParticle,
                transform.position,
                transform.rotation
            );
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
        if ((health -= damage) <= 0) return;
        health -= damage;
        UpdateSlider();
    }

    void UpdateSlider()
    {
        healthBar.fillAmount = health / maxHealth;
        currentHealthText.text = health + " / " + maxHealth;

    }
}
