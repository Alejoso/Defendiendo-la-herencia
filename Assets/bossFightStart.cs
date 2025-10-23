using UnityEngine;

public class bossFightStart : MonoBehaviour
{
    [Header("Boss Fight Effects")]
    [SerializeField] private ParticleSystem bossParticleEffect;
    [SerializeField] private float cameraShakeDuration = 2f;
    [SerializeField] private float cameraShakeIntensity = 0.5f;

    private Camera mainCamera;
    private MonoBehaviour cameraController;
    private bool bossTriggered = false;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // Find CameraController script on the camera
            cameraController = mainCamera.GetComponent("CameraController") as MonoBehaviour;
            if (cameraController == null)
            {
                Debug.LogWarning("CameraController script not found on main camera!");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !bossTriggered)
        {
            bossTriggered = true;
            Debug.Log("Boss Fight Started!");

            // Start camera shake coroutine
            StartCoroutine(CameraShake());

            // Activate particle effect
            if (bossParticleEffect != null)
            {
                bossParticleEffect.Play();
            }
            else
            {
                Debug.LogWarning("Boss particle effect not assigned!");
            }
        }
    }

    System.Collections.IEnumerator CameraShake()
    {
        // Disable CameraController during shake
        if (cameraController != null)
        {
            cameraController.enabled = false;
        }

        // Set camera to specific position for boss fight
        Vector3 bossPosition = new Vector3(-6.92f, 14.98f, mainCamera.transform.position.z);
        mainCamera.transform.position = bossPosition;
        
        // Store this position for shaking
        Vector3 originalPosition = mainCamera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < cameraShakeDuration)
        {
            // Generate random offset from the ORIGINAL position
            float x = Random.Range(-1f, 1f) * cameraShakeIntensity;
            float y = Random.Range(-1f, 1f) * cameraShakeIntensity;

            mainCamera.transform.localPosition = originalPosition + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Return to the original position
        mainCamera.transform.localPosition = originalPosition;

        // Re-enable CameraController
        if (cameraController != null)
        {
            cameraController.enabled = true;
        }
    }
}
