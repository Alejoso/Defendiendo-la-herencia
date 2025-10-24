using UnityEngine;

public class RainFollowCamera : MonoBehaviour
{
    private Camera cam; 
    void Start()
    {
        cam = Camera.main; 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y + 10, cam.transform.position.z); 
    }
}
