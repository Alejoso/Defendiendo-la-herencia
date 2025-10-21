using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera cam;
    private Transform player;
    [SerializeField]
    private float threshold;
    [SerializeField]
    private float cameraSpeed; 
    
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
        cam = Camera.main; 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //New camera follow, so we can have a cool effect where if the mouse moves, the camera also does
        Vector3 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetPosition = (player.position + mousePosition) / 2f;

        targetPosition.x = Mathf.Clamp(targetPosition.x, -threshold + player.position.x, threshold + player.position.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, -threshold + player.position.y, threshold + player.position.y);
        targetPosition.z = -10f; 

        transform.position = Vector3.Lerp(transform.position, targetPosition, cameraSpeed * Time.deltaTime);

    }
}
