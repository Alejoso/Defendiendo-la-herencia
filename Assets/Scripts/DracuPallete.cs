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

    [SerializeField] float xpQuantity; 
    Vector3 _startPos;
    bool _collected;

    private GameController gameController;
    private GameObject player;

    [SerializeField] private float dracuPaleteSpeed;
    private bool onMagnetRange; 

    void Awake()
    {
        _startPos = transform.position;
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        player = GameObject.Find("Player");
        onMagnetRange = false; 
    }

    void OnEnable()
    {
        _collected = false;
        _startPos = transform.position; // in case coin is pooled/moved
    }

    void Update()
    {
        if (_collected) return;
        if (onMagnetRange) return; 

        // Bob on the Y axis using a sine wave
        float newY = _startPos.y + Mathf.Sin((Time.time + phaseOffset) * frequency) * amplitude;
        transform.position = new Vector3(_startPos.x, newY, _startPos.z);

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_collected) return;
        if (!other.CompareTag(playerTag)) return;

        _collected = true;

        gameController.AddXp(xpQuantity);

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

    void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Magnet")) return;

        onMagnetRange = true; 
        transform.position = Vector3.MoveTowards(
            transform.position,
            player.transform.position,
            dracuPaleteSpeed * Time.deltaTime
        );

    }




}
