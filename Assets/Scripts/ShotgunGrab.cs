using UnityEngine;

public class ShotgunGrab : MonoBehaviour
{
    private PlayerController playerController;
    private GameProgression gameProgression; 

    [SerializeField] private GameObject pistolUI;
    [SerializeField] private GameObject shotgunUI;
    [SerializeField] private AudioClip shotgunGrab;

    float amplitude = 0.3f;   // how high it bobs
    float frequency = 2.0f;    // how fast it bobs
    float phaseOffset = 0f;    // randomize per coin if you want

    Vector3 _startPos; 
    void Awake()
    {
        _startPos = transform.position;
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        gameProgression = GameObject.Find("GameController").GetComponent<GameProgression>(); 


    }

    void Update()
    {
        // Bob on the Y axis using a sine wave
        float newY = _startPos.y + Mathf.Sin((Time.time + phaseOffset) * frequency) * amplitude;
        transform.position = new Vector3(_startPos.x, newY, _startPos.z);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return; 

        AudioSource playerAudio = collision.GetComponent<AudioSource>();
        playerAudio.PlayOneShot(shotgunGrab);

        pistolUI.SetActive(false);
        shotgunUI.SetActive(true);

        playerController.ChangeToShootgun();

        gameObject.SetActive(false);

        gameProgression.AddOneToIndexKeyLocation(); 
        


    }
}
