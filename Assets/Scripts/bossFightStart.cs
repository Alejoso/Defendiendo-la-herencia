using UnityEngine;
using UnityEngine.UI;

public class bossFightStart : MonoBehaviour
{
    private GameProgression gameProgression;

    [SerializeField] private GameObject[] enemy;

    [Header("Boss Fight Effects")]
    [SerializeField] private ParticleSystem bossParticleEffect;
    [SerializeField] private GameObject bosslightAppear;
    [SerializeField] private GameObject rainEffect;
    [SerializeField] private float cameraShakeDuration = 2f;
    [SerializeField] private float cameraShakeIntensity = 0.5f;
    [SerializeField] private float cameraMoveToPositionDuration = 1.5f;

    [Header("Boss Spawn")]
    [Tooltip("Boss prefab to instantiate after the intro finishes.")]
    [SerializeField] private GameObject bossPrefab;
    [Tooltip("Optional specific spawn point. If empty, this object's position will be used.")]
    [SerializeField] private Transform bossSpawnPoint;

    [Header("Screen Fade")]
    [Tooltip("Full-screen white Image on a UI Canvas. We'll fade its alpha in/out.")]
    [SerializeField] private Image whiteScreenImage;
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float fadeOutDuration = 1f;

    private Camera mainCamera;
    private MonoBehaviour cameraController;
    private bool bossTriggered = false;

    [SerializeField] private Animator arrowLastAnimator;
    [SerializeField] private GameObject currentObjective;
    void Awake()
    {
        gameProgression = GameObject.Find("GameController").GetComponent<GameProgression>();

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

        // Ensure fade image starts transparent
        if (whiteScreenImage != null)
        {
            var c = whiteScreenImage.color;
            c.a = 0f;
            whiteScreenImage.color = c;
            if (!whiteScreenImage.gameObject.activeSelf)
                whiteScreenImage.gameObject.SetActive(true);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !bossTriggered && gameProgression.currentObjective == "Vengar al abuelo")
        {
            bossTriggered = true;
            Debug.Log("Boss Fight Started!");

            arrowLastAnimator.Play("Arrow fade");
            currentObjective.SetActive(false); 

            InvokeRepeating("GenerateEnemies", 20f, 7f);

            // Play boss music from this object's AudioSource
            var audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning("No AudioSource found on bossFightStart object for boss music!");
            }

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

        // Gradually move camera to specific position for boss fight
        if (mainCamera != null)
        {
            Vector3 startPosition = mainCamera.transform.position;
            Vector3 targetPosition = new Vector3(-6.92f, 14.98f, mainCamera.transform.position.z);
            float moveElapsed = 0f;

            while (moveElapsed < cameraMoveToPositionDuration)
            {
                moveElapsed += Time.deltaTime;
                float t = moveElapsed / cameraMoveToPositionDuration;
                // Smooth interpolation
                mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null;
            }

            // Ensure exact final position
            mainCamera.transform.position = targetPosition;
        }

        // Activate boss light appearance effect
        if (bosslightAppear != null)
        {
            bosslightAppear.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Boss light appearance effect not assigned!");
        }

        // Store this position for shaking
        Vector3 originalPosition = mainCamera != null ? mainCamera.transform.localPosition : Vector3.zero;
        float elapsed = 0f;

        while (elapsed < cameraShakeDuration)
        {
            // Generate random offset from the ORIGINAL position
            float x = Random.Range(-1f, 1f) * cameraShakeIntensity;
            float y = Random.Range(-1f, 1f) * cameraShakeIntensity;

            if (mainCamera != null)
            {
                mainCamera.transform.localPosition = originalPosition + new Vector3(x, y, 0f);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Return to the original position
        if (mainCamera != null)
        {
            mainCamera.transform.localPosition = originalPosition;
        }

        // Fade in to white, spawn boss when fully covered, then fade out
        if (whiteScreenImage != null)
        {
            // Fade in
            yield return StartCoroutine(FadeImage(whiteScreenImage, 0f, 1f, fadeInDuration));

            // Spawn boss while covered
            TrySpawnBoss();

            // Fade out
            yield return StartCoroutine(FadeImage(whiteScreenImage, 1f, 0f, fadeOutDuration));
        }
        else
        {
            // No fade image assigned, just spawn immediately
            TrySpawnBoss();
        }

        // Disable light
        if (bosslightAppear != null)
        {
            bosslightAppear.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Boss light appearance effect not assigned!");
        }

        // Activate rain effect
        if (rainEffect != null)
        {
            rainEffect.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Rain effect not assigned!");
        }

        // Re-enable CameraController after transition
        if (cameraController != null)
        {
            cameraController.enabled = true;
        }
    }

    private void TrySpawnBoss()
    {
        if (bossPrefab != null)
        {
            Vector3 spawnPos = bossSpawnPoint != null ? bossSpawnPoint.position : transform.position;
            Quaternion spawnRot = bossSpawnPoint != null ? bossSpawnPoint.rotation : Quaternion.identity;
            Instantiate(bossPrefab, spawnPos, spawnRot);
        }
        else
        {
            Debug.LogWarning("bossFightStart: Boss prefab is not assigned. Skipping boss spawn.");
        }
    }

    private System.Collections.IEnumerator FadeImage(Image img, float from, float to, float duration)
    {
        if (img == null)
            yield break;

        if (duration <= 0f)
        {
            var cInst = img.color;
            cInst.a = to;
            img.color = cInst;
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(from, to, t / duration);
            var c = img.color;
            c.a = a;
            img.color = c;
            yield return null;
        }
        // Ensure exact final value
        var cFinal = img.color;
        cFinal.a = to;
        img.color = cFinal;
    }

    void GenerateEnemies()
    {
        int enemiesToSpawn = Random.Range(2, 4);
        GameObject enemyToSpawn;
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            if (Random.value < 0.5)
            {
                enemyToSpawn = enemy[0];
                Instantiate(enemyToSpawn, gameProgression.GenerateRandomEnemySpawn(), enemyToSpawn.transform.rotation);
                return;
            }
            else if (Random.value < 0.7)
            {
                enemyToSpawn = enemy[1];
                Instantiate(enemyToSpawn, gameProgression.GenerateRandomEnemySpawn(), enemyToSpawn.transform.rotation);
                return;

            }

        }


    }
}
