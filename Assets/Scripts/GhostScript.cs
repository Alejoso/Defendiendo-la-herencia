using UnityEngine;

public class GhostScript : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float fadeTimer = 0f;
    [SerializeField] private float fadeInterval = 5f;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float minAlpha = 0.08f; // 🔹 Nuevo: nivel mínimo de visibilidad
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

            // 🔹 Fade out (hasta minAlpha, no hasta 0)
            float t = 0f;
            while (t < fadeOutTime)
            {
                float alpha = Mathf.Lerp(1f, minAlpha, t / fadeOutTime);
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                t += Time.deltaTime;
                yield return null;
            }
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, minAlpha);

            // 🔹 Espera un momento con poca visibilidad
            yield return new WaitForSeconds(fadeDuration * 0.5f);

            // 🔹 Fade in
            t = 0f;
            while (t < fadeInTime)
            {
                float alpha = Mathf.Lerp(minAlpha, 1f, t / fadeInTime);
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                t += Time.deltaTime;
                yield return null;
            }
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
        }
        isFading = false;
    }
}
