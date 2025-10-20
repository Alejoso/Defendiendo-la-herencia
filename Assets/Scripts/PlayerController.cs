using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; // top-down
    }

    void FixedUpdate()
    {
        //Get keys down
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        //Vector with those values
        Vector2 input = new Vector2(h, v);

        if (input.sqrMagnitude > 1f) input.Normalize(); // no √2 boost on diagonals

        rb.linearVelocity = input * speed; // constant speed in any direction
    }
}