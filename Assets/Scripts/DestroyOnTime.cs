using UnityEngine;

public class DestroyOnTime : MonoBehaviour
{
    [Header("Destroy Settings")]
    [SerializeField] private bool destroyAfterParticleSystem = true;
    [SerializeField] private float customDelay = 0f; // Only used if not using particle system

    private ParticleSystem _particleSystem;

    void Start()
    {
        if (destroyAfterParticleSystem)
        {
            _particleSystem = GetComponent<ParticleSystem>();

            if (_particleSystem != null)
            {
                // Destroy after the particle system duration + lifetime
                float totalDuration = _particleSystem.main.duration + _particleSystem.main.startLifetime.constantMax;
                Destroy(gameObject, totalDuration);
            }
            else
            {
                Debug.LogWarning("DestroyOnTime: No ParticleSystem found. Using custom delay instead.");
                Destroy(gameObject, customDelay);
            }
        }
        else
        {
            // Use custom delay
            Destroy(gameObject, customDelay);
        }
    }
}
