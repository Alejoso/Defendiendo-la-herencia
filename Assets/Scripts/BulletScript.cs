using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [Header("Override Direction (optional)")]
    public bool useOverrideDirection = false;
    public Vector2 overrideDirection = Vector2.zero;

    private Vector3 mousePos;
    private Camera cam;
    private Rigidbody2D rb;
    public float speed;

    private float bulletDuration = 3f;
    private float timer;
    void Start()
    {
        // Setup
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();

        Vector2 direction;
        if (useOverrideDirection && overrideDirection != Vector2.zero)
        {
            direction = overrideDirection.normalized;
        }
        else
        {
            // Get mouse position and aim towards it
            mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 toMouse = mousePos - transform.position;
            direction = new Vector2(toMouse.x, toMouse.y).normalized;
        }

        // Apply velocity and rotation
        rb.linearVelocity = direction * speed;
        float rot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 90);
    }

    // Update is called once per frame
    void Update()
    {

        //Destroy bullet after 3 seconds
        timer += Time.deltaTime;
        if (timer >= bulletDuration)
        {
            Destroy(gameObject);
        }
    }
}
