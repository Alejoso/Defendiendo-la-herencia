using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    private PlayerController playerController;

    void Awake()
    {
        // Get reference to PlayerController from parent
        playerController = GetComponentInParent<PlayerController>();
        
        if (playerController == null)
        {
            Debug.LogError("MeleeWeapon: PlayerController not found in parent!");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if we hit an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Get EnemyController and apply damage
            EnemyController enemy = collision.GetComponent<EnemyController>();
            if (enemy != null && playerController != null)
            {
                // Use the same damage system as bullets
                enemy.GetComponent<EnemyController>().SendMessage("TakeDamage", playerController.GetDamage(), SendMessageOptions.DontRequireReceiver);
                Debug.Log("Melee hit enemy for " + playerController.GetDamage() + " damage!");
            }
        }
    }
}
