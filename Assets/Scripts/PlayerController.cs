using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;

    Rigidbody2D rb;
    Camera cam;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; // top-down
        cam = Camera.main;
    }

    void FixedUpdate()
    {
        MovePlayer();
        RotateSprite(); 
    }

    void MovePlayer()
    {
        //Move the player
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        //Vector with those values
        Vector2 input = new Vector2(h, v);

        if (input.sqrMagnitude > 1f) input.Normalize(); // no √2 boost on diagonals

        rb.linearVelocity = input * speed; // constant speed in any direction
    }
    
    void RotateSprite()
    {
        // --- Mirar hacia el mouse ---
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 toMouse = (mouseWorld - transform.position);
        float angleDeg = Mathf.Atan2(toMouse.y, toMouse.x) * Mathf.Rad2Deg;

        // Ajusta este offset según cómo esté dibujado tu sprite:
        // si "mira" hacia ARRIBA en el arte, usa -90f; si mira a la DERECHA, usa 0f.
        float spriteOffset = -90f;

        rb.MoveRotation(angleDeg + spriteOffset);
    }
    
}