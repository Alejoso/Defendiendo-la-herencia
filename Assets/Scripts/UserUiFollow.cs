using UnityEngine;
using UnityEngine.UI; 
public class UserUiFollow : MonoBehaviour
{
    private Transform player;
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        transform.position = player.transform.position;
    }
    
}
