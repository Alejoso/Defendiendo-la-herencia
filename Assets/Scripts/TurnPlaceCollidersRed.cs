using UnityEngine;

public class TurnPlaceCollidersRed : MonoBehaviour
{
    public Transform player;
    public float maxDistance = 5f; // Hasta dónde se nota el efecto
    public Color nearColor = new Color(1f, 0f, 0f, 0.8f); // Rojo + opacidad alta
    public Color farColor = new Color(1f, 0f, 0f, 0.1f);     // Rojo + transparente

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        // Normalizamos 0–1 dependiendo de la distancia
        float t = Mathf.InverseLerp(maxDistance, 0f, distance);

        // Interpolamos entre color cercano y lejano
        spriteRenderer.color = Color.Lerp(farColor, nearColor, t);
    }
}
