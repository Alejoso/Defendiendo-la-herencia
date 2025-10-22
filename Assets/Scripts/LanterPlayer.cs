using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LanterPlayer : MonoBehaviour
{
    [Header("Candle Flicker Settings")]
    [SerializeField] private Light2D spotLight; // 2D Light (set to Spot)
    [SerializeField] private bool autoFlicker = true;
    [SerializeField] private float minIntensity = 0.7f;
    [SerializeField] private float maxIntensity = 1.4f;
    [SerializeField] private float flickerSpeed = 3.0f; // higher = faster flicker
    [SerializeField] private bool usePerlinNoise = true; // more natural than sine

    // random seed to decorrelate multiple lights
    private float _noiseSeed;

    void Awake()
    {
        if (spotLight == null)
        {
            spotLight = GetComponent<Light2D>();
        }

        if (spotLight != null)
        {
            // Ensure it's a 2D light; set to Point if Spot isn't available in your URP version
            spotLight.lightType = Light2D.LightType.Point; // If you have 2D Spot available, set it in the Inspector
        }

        // randomize seed so multiple candles don't sync
        _noiseSeed = Random.Range(0f, 1000f);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Clamp sanity
        if (minIntensity > maxIntensity)
        {
            var t = minIntensity; minIntensity = maxIntensity; maxIntensity = t;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (autoFlicker)
        {
            UpdateCandleFlicker();
        }
    }

    // Call this to apply the candle flicker for this frame
    public void UpdateCandleFlicker()
    {
        if (spotLight == null) return;

        float t;
        if (usePerlinNoise)
        {
            // Perlin returns [0,1]; use time * speed + seed
            t = Mathf.PerlinNoise(Time.time * flickerSpeed, _noiseSeed);
        }
        else
        {
            // Sine mapped to [0,1]
            t = (Mathf.Sin(Time.time * flickerSpeed) + 1f) * 0.5f;
        }

        float target = Mathf.Lerp(minIntensity, maxIntensity, t);
        spotLight.intensity = target;
    }
}
