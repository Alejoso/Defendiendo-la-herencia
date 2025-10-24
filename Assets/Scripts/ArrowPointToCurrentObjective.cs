using UnityEngine;

public class ArrowPointToCurrentObjective : MonoBehaviour
{
    private GameObject player;

    [SerializeField] GameObject objective0;
    [SerializeField] GameObject objective1;
    [SerializeField] GameObject objective2;
    [SerializeField] GameObject objective3;
    [SerializeField] GameObject objective4;
    [SerializeField] GameObject objective5;



    private GameObject locationToLook; 

    void Start()
    {
        player = GameObject.Find("Player");
    }
    
    public void changeCurrentObjetive(int indexKeyLocation)
    {
        switch (indexKeyLocation)
        {
            case 0:
                locationToLook = objective0; 
                break;

            case 1:
                locationToLook = objective1;
                break;
            
            case 2:
                locationToLook = objective2;
                break;
            
            case 3:
                locationToLook = objective3;
                break;
            
            case 4:
                locationToLook = objective4;
                break;

            case 5:
                locationToLook = objective5;
                break;

            default:
            break;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 2, 0);

        Vector3 direction = locationToLook.transform.position - player.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    
   
}
