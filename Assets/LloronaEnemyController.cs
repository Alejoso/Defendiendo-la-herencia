using UnityEngine;

public class LloronaEnemyController : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private CircleCollider2D attackCollider;
    [SerializeField] private ParticleSystem attackParticles;
    [SerializeField] private float attackInterval = 10f; // time between attacks
    [SerializeField] private float attackDuration = 3f; // how long attack lasts
    
    private EnemyController enemyController;
    private float attackTimer = 0f;
    private bool isAttacking = false;
    private float savedSpeed = 0f;

    void Awake()
    {
        // Get reference to EnemyController on same GameObject
        enemyController = GetComponent<EnemyController>();
        
        if (enemyController == null)
        {
            Debug.LogError("LloronaEnemyController: EnemyController not found on same GameObject!");
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
        yield return new WaitForSeconds(attackDuration);
        
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
