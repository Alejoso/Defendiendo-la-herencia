using UnityEngine;

public class FincaDelAbuelo : MonoBehaviour
{
    private GameProgression gameProgression;
    private GameObject invisibleBorders;

    [SerializeField] private string thisPlace = "Casa del abuelo";


    void Awake()
    {
        gameProgression = GameObject.Find("GameController").GetComponent<GameProgression>();
        invisibleBorders = transform.Find("Invisible barriers").gameObject;
    }

    void Update()
    {
        if(gameProgression.didPlayerCompleteObjective)
        {
            invisibleBorders.SetActive(false); 
        }
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            gameProgression.ChangePlayerLocation(thisPlace);

            string currentObjective = gameProgression.currentObjective;

            if(currentObjective == thisPlace)
            {
                invisibleBorders.SetActive(true); 
            }
        }
        
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameProgression.ChangePlayerLocation("");
        }
    }

}
