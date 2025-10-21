using UnityEngine;

public class DracuPallete : MonoBehaviour
{
    [Header("Idle bounce")]
    [SerializeField] float amplitude = 0.15f;   // how high it bobs
    [SerializeField] float frequency = 2.0f;    // how fast it bobs
    [SerializeField] float phaseOffset = 0f;    // randomize per coin if you want

    [Header("Pickup")]
    [SerializeField] string playerTag = "Player";
    [SerializeField] AudioClip pickupSfx;
    [SerializeField] GameObject pickupVfx;
    [SerializeField] float destroyDelay = 0.02f;

    Vector3 _startPos;
    bool _collected;
    AudioSource _audio;

    void Awake()
    {
        _startPos = transform.position;
        _audio = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        _collected = false;
        _startPos = transform.position; // in case coin is pooled/moved
    }

    void Update()
    {
        if (_collected) return;

        // Bob on the Y axis using a sine wave
        float newY = _startPos.y + Mathf.Sin((Time.time + phaseOffset) * frequency) * amplitude;
        transform.position = new Vector3(_startPos.x, newY, _startPos.z);

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_collected) return;
        if (!other.CompareTag(playerTag)) return;

        _collected = true;

        // Play VFX/SFX
        if (pickupVfx) Instantiate(pickupVfx, transform.position, Quaternion.identity);
        if (pickupSfx)
        {
            if (_audio == null) _audio = gameObject.AddComponent<AudioSource>();
            _audio.PlayOneShot(pickupSfx);
        }

        // Disable visuals & collider immediately; destroy after SFX fires
        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.enabled = false;
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        // TODO: Add your coin logic here (e.g., GameManager.AddCoins(1);)

        //Destroy(gameObject, destroyDelay);
    }
}
