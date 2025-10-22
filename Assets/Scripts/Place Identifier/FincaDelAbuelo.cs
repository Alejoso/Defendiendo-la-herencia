using UnityEngine;

public class FincaDelAbuelo : MonoBehaviour
{
    private GameController gameController;
    private GameObject invisibleBorders;

    private string thisPlace = "Casa del abuelo"; 


    void Awake()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        invisibleBorders = transform.Find("Invisible barriers").gameObject; 
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            gameController.ChangePlayerLocation(thisPlace);

            string currentObjective = gameController.currentObjective;

            if(currentObjective == thisPlace)
            {
                invisibleBorders.SetActive(true); 
            }
        }
        
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            gameController.ChangePlayerLocation(""); 
        }
    }
}
