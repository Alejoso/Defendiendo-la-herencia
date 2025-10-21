using System.Collections;
using UnityEngine;
using UnityEngine.UI; 

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    //Player variables
    [SerializeField] private float speed = 5f;

    Rigidbody2D rb;
    Camera cam;
    private float lookingDirection;

    //Health and damage related things
    [SerializeField] private Slider healthBar;

    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth;

    private bool inmmunityFrame;
    private float inmmunityFrameTimer = 0.67f;


    //Shooting mecanic variables
    public GameObject bullet; 
    private bool canShoot = true;
    private float shootingTimer;
    [SerializeField] private float timeBetweenShooting;
    [SerializeField] private GameObject bulletSpawn; 


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; // top-down
        cam = Camera.main;

        //Set uo variables
        currentHealth = maxHealth;
        healthBar.value = currentHealth / maxHealth; 

    }

    void FixedUpdate()
    {
        MovePlayer();
        RotateSprite();
        Shooting();

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
        Vector2 toMouse = mouseWorld - transform.position;
        lookingDirection = Mathf.Atan2(toMouse.y, toMouse.x) * Mathf.Rad2Deg;

        // Ajusta este offset según cómo esté dibujado tu sprite:
        // si "mira" hacia ARRIBA en el arte, usa -90f; si mira a la DERECHA, usa 0f.
        float spriteOffset = 90f;

        rb.MoveRotation(lookingDirection + spriteOffset);

    }

    //Shooting mechanic with intervals of when the player can shoot
    void Shooting()
    {
        if (canShoot == false)
        {
            shootingTimer += Time.deltaTime;
            if (shootingTimer >= timeBetweenShooting)
            {
                canShoot = true;
                shootingTimer = 0;
            }
        }

        if (Input.GetMouseButton(0) && canShoot)
        {
            canShoot = false;
            Instantiate(bullet, bulletSpawn.transform.position, Quaternion.identity);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !inmmunityFrame)
        {
            inmmunityFrame = true;
            TakeDamage(10f); 
        }
    }

    //Take damage and update slider
    void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.value = currentHealth / maxHealth;
        StartCoroutine(InnmunityFrameTimer());
    }
    
    //Co-rutine for InmmunityFrames
    IEnumerator InnmunityFrameTimer()
    {
        yield return new WaitForSeconds(inmmunityFrameTimer);
        inmmunityFrame = false; 
    }
    

}