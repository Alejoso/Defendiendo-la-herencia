using UnityEngine;

public class Dolex : MonoBehaviour
{
    [Header("Idle bounce")]
    [SerializeField] float amplitude = 0.15f;   // how high it bobs
    [SerializeField] float frequency = 2.0f;    // how fast it bobs
    [SerializeField] float phaseOffset = 0f;    // randomize per coin if you want

    [Header("Pickup")]
    [SerializeField] string playerTag = "Player";
    [SerializeField] AudioClip pickupSfx;
    [SerializeField] GameObject pickupVfx;

    [SerializeField] int healthQuantity; 
    Vector3 _startPos;
    bool _collected;

    private PlayerController playerController;

    void Awake()
    {
        _startPos = transform.position;
        playerController = GameObject.Find("Player").GetComponent<PlayerController>(); 
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
        Debug.Log("Colisione"); 
        if (_collected) return;
        if (!other.CompareTag(playerTag)) return;

        _collected = true;
        playerController.Heal(healthQuantity); 

        // Play VFX/SFX
        if (pickupVfx) Instantiate(pickupVfx, transform.position, Quaternion.identity);
        if (pickupSfx)
        {
            AudioSource playerAudio = other.GetComponent<AudioSource>();
            if (playerAudio == null)
            {
                playerAudio = other.gameObject.AddComponent<AudioSource>();
            }
            playerAudio.PlayOneShot(pickupSfx);
        }
        Instantiate(pickupVfx, transform.position, pickupVfx.transform.rotation); 
        Destroy(gameObject);
    }
}
