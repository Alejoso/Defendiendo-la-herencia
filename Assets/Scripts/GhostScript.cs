using UnityEngine;

public class GhostScript : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float fadeTimer = 0f;
    [SerializeField] private float fadeInterval = 5f;
    [SerializeField] private float fadeDuration = 1.5f;
    private bool isFading = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        fadeTimer += Time.deltaTime;
        if (!isFading && fadeTimer >= fadeInterval)
        {
            StartCoroutine(FadeGhost());
            fadeTimer = 0f;
        }
    }

    private System.Collections.IEnumerator FadeGhost()
    {
        isFading = true;
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            float fadeOutTime = fadeDuration * 0.5f;
            float fadeInTime = fadeDuration * 0.5f;

            // Fade out
            float t = 0f;
            while (t < fadeOutTime)
            {
                float alpha = Mathf.Lerp(1f, 0f, t / fadeOutTime);
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                t += Time.deltaTime;
                yield return null;
            }
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

            // Wait while invisible
            yield return new WaitForSeconds(fadeDuration * 0.5f);

            // Fade in
            t = 0f;
            while (t < fadeInTime)
            {
                float alpha = Mathf.Lerp(0f, 1f, t / fadeInTime);
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                t += Time.deltaTime;
                yield return null;
            }
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
        }
        isFading = false;
    }
}
