using UnityEngine;

public class GUIFollowCamera : MonoBehaviour
{
    [SerializeField] private GameObject xpBar;
    private Camera cam; 
    void Awake()
    {
        cam = Camera.main; 
    }

    // Update is called once per frame
    void Update()
    {
        xpBar.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, xpBar.transform.position.z);
    }
    
    
}
