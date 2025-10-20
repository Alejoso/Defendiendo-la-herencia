using UnityEngine;

public class CameraController : MonoBehaviour
{
    Transform playerPosition;
    float offset = -10f; 
    void Start()
    {
        playerPosition = GameObject.Find("Player").GetComponent<Transform>(); 
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = playerPosition.position + new Vector3(0,0, offset);  
    }
}
