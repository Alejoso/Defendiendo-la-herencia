using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private Vector3 mousePos;
    private Camera cam;
    private Rigidbody2D rb;
    public float speed;

    private float bulletDuration = 3f;
    private float timer; 
    void Start()
    {
        //Get mouse position
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        //Get the direction and rotation the bullet is going to be shot
        Vector3 direction = mousePos - transform.position;
        Vector3 rotation = transform.position - mousePos;

        //Apply velocity in that direction and apply the corresponding rotation
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * speed;
        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 90); 
    }

    // Update is called once per frame
    void Update()
    {
        
        //Destroy bullet after 3 seconds
        timer += Time.deltaTime; 
        if(timer >= bulletDuration)
        {
            Destroy(gameObject); 
        }
    }
}
