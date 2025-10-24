using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    //Player variables
    [SerializeField] private float speed = 5f;
    [SerializeField] private float cameraTurnSpeed;
    [SerializeField] private float maxHealth;
    [SerializeField] private int damage;

    //Diaplay player stats
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI damageText;


    Rigidbody2D rb;
    Camera cam;
    private float lookingDirection;

    //Health and damage related things
    [SerializeField] private Slider healthBar;

    [SerializeField] private float currentHealth;

    private bool inmmunityFrame;
    private float inmmunityFrameTimer = 0.67f;


    //Weapon system variables
    [Header("Weapon Loadout")]
    [SerializeField] private Sprite revolverSprite;
    [SerializeField] private Sprite shotgunSprite;
    [SerializeField] private Sprite meleeSprite;
    [SerializeField] private GameObject meleeVisualSprite; // Visual sprite object for melee weapon
    private SpriteRenderer playerSpriteRenderer;

    [Header("Revolver Settings")]
    public GameObject bullet;
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private float timeBetweenShooting;
    [SerializeField] private GameObject bulletSpawn;
    private int currentAmmo;
    private bool canShoot = true;
    private float shootingTimer;

    [Header("Melee Settings")]
    [SerializeField] private GameObject meleeWeapon; // Child object with BoxCollider2D
    [SerializeField] private float meleeAttackCooldown = 0.5f;
    [SerializeField] private float meleeLockoutTime = 8f;
    private bool canMelee = true;
    private float meleeTimer;

    // Weapon state
    private enum WeaponMode { Revolver, Melee }
    private enum WeaponType { Revolver, Shotgun }
    private WeaponMode currentWeaponMode = WeaponMode.Revolver;
    [Header("Ranged Weapon Selection")]
    [SerializeField] private WeaponType weapon = WeaponType.Revolver; // default as requested
    [SerializeField] private float shotgunSpreadAngle = 20f; // degrees to each side
    private bool isLockedInMelee = false;
    private float meleeLockoutTimer;

    //Muzzle flash effect
    [Header("Muzzle Flash")]
    [SerializeField] private UnityEngine.Rendering.Universal.Light2D muzzleFlashLight;
    [SerializeField] private float muzzleFlashIntensity = 10f;
    [SerializeField] private float muzzleFlashFadeSpeed = 30f;
    private float currentMuzzleIntensity = 0f;

    //Shooting VFX
    [Header("Shooting VFX")]
    [SerializeField] private GameObject shootingVfxPrefab;

    //Aduio
    private AudioSource gameControllerAudioSource;
    private AudioSource playerAudioSource;

    [SerializeField] private AudioClip shootingSFX;
    [Header("Footstep Audio")]
    [SerializeField] private AudioClip footstepSFX;

    //Player pistol or shotgun UI

    [SerializeField] private Image reloadImage;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>(); // Get player's sprite renderer
        playerAudioSource = GetComponent<AudioSource>(); // Get player's AudioSource
        gameControllerAudioSource = GameObject.Find("GameController").GetComponent<AudioSource>();
        rb.freezeRotation = true; // top-down
        cam = Camera.main;

        //Set up variables
        currentHealth = maxHealth;
        healthBar.value = currentHealth / maxHealth;

        // Initialize weapon system
        currentAmmo = maxAmmo;
        SwitchToRevolver();

        healthText.text = currentHealth + "/" + maxHealth;
        speedText.text = (speed * 10f).ToString();
        damageText.text = damage.ToString();

        // Disable melee collider and visual initially
        if (meleeWeapon != null)
        {
            var meleeCollider = meleeWeapon.GetComponent<Collider2D>();
            if (meleeCollider != null) meleeCollider.enabled = false;
        }

        if (meleeVisualSprite != null)
        {
            meleeVisualSprite.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
        RotateSprite();
        HandleWeaponSystem();
        UpdateMuzzleFlash();
        HandleFootstepAudio();
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

        if (toMouse.sqrMagnitude < 0.1f)  // 0.05f ≈ distancia mínima, puedes ajustar
            return;

        lookingDirection = Mathf.Atan2(toMouse.y, toMouse.x) * Mathf.Rad2Deg;

        // Ajusta este offset según cómo esté dibujado tu sprite:
        // si "mira" hacia ARRIBA en el arte, usa -90f; si mira a la DERECHA, usa 0f.
        float spriteOffset = 90f;

        float targetRotation = lookingDirection + spriteOffset;
        float smoothedRotation = Mathf.LerpAngle(rb.rotation, targetRotation, cameraTurnSpeed * Time.fixedDeltaTime);

        rb.MoveRotation(smoothedRotation);

    }

    // ============== WEAPON SYSTEM ==============

    void HandleWeaponSystem()
    {
        // Handle melee lockout timer
        if (isLockedInMelee)
        {
            meleeLockoutTimer -= Time.deltaTime;

            reloadImage.fillAmount = meleeLockoutTimer / meleeLockoutTime;
            if (meleeLockoutTimer <= 0)
            {
                reloadImage.fillAmount = 0;
                // Unlock and return to revolver
                isLockedInMelee = false;
                currentAmmo = maxAmmo; // Reload ammo
                SwitchToRevolver();
            }
        }

        // Handle current weapon mode
        if (currentWeaponMode == WeaponMode.Revolver && !isLockedInMelee)
        {
            HandleRevolver();
        }
        else if (currentWeaponMode == WeaponMode.Melee)
        {
            HandleMelee();
        }
    }

    void HandleRevolver()
    {
        // Shooting cooldown timer
        if (canShoot == false)
        {
            shootingTimer += Time.deltaTime;

            if (shootingTimer >= timeBetweenShooting)
            {
                canShoot = true;
                shootingTimer = 0;
            }
        }

        // Shoot when left click and can shoot
        if (Input.GetMouseButton(0) && canShoot && currentAmmo > 0)
        {
            canShoot = false;
            currentAmmo--;

            // Compute aim direction from spawn to mouse
            Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 toMouse = mouseWorld - bulletSpawn.transform.position;
            Vector2 baseDir = new Vector2(toMouse.x, toMouse.y).normalized;

            // Fire based on weapon selection
            if (weapon == WeaponType.Revolver)
            {
                SpawnBulletWithDirection(baseDir);
            }
            else // Shotgun: 3 bullets with spread
            {
                Vector2 leftDir = Rotate2D(baseDir, -shotgunSpreadAngle);
                Vector2 rightDir = Rotate2D(baseDir, shotgunSpreadAngle);
                SpawnBulletWithDirection(baseDir);
                SpawnBulletWithDirection(leftDir);
                SpawnBulletWithDirection(rightDir);
            }

            gameControllerAudioSource.PlayOneShot(shootingSFX);

            // Spawn shooting VFX particle system
            if (shootingVfxPrefab != null)
            {
                float playerRotation = transform.eulerAngles.z;
                Quaternion vfxRotation = Quaternion.Euler(playerRotation + 90, -90, 0);
                Instantiate(shootingVfxPrefab, bulletSpawn.transform.position, vfxRotation);
            }

            // Trigger muzzle flash
            if (muzzleFlashLight != null)
            {
                currentMuzzleIntensity = muzzleFlashIntensity;
            }

            // Check if out of ammo
            if (currentAmmo <= 0)
            {
                SwitchToMelee();
                isLockedInMelee = true;
                meleeLockoutTimer = meleeLockoutTime;
                reloadImage.fillAmount = 1f;
            }
        }
    }

    // --- Helpers for shotgun ---
    private void SpawnBulletWithDirection(Vector2 dir)
    {
        var go = Instantiate(bullet, bulletSpawn.transform.position, Quaternion.identity);
        var bs = go.GetComponent<BulletScript>();
        if (bs != null)
        {
            bs.useOverrideDirection = true;
            bs.overrideDirection = dir.normalized;
        }
    }

    private static Vector2 Rotate2D(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }

    void HandleMelee()
    {
        // Melee cooldown timer
        if (canMelee == false)
        {
            meleeTimer += Time.deltaTime;
            if (meleeTimer >= meleeAttackCooldown)
            {
                canMelee = true;
                meleeTimer = 0;
            }
        }

        // Melee attack on left click
        if (Input.GetMouseButton(0) && canMelee)
        {
            canMelee = false;
            StartCoroutine(MeleeAttack());
        }
    }

    IEnumerator MeleeAttack()
    {
        // Enable melee collider and animate melee visual swing during the active window
        const float attackDuration = 0.2f; // active hit window duration

        // Prepare swing visual (optional)
        if (meleeVisualSprite != null)
        {
            // Start at -80 degrees on Z
            meleeVisualSprite.transform.localRotation = Quaternion.Euler(0f, 0f, -200f);
        }

        if (meleeWeapon != null)
        {
            var meleeCollider = meleeWeapon.GetComponent<Collider2D>();
            if (meleeCollider != null)
            {
                meleeCollider.enabled = true;

                // Animate rotation from -80 to -200 over attackDuration while hitbox is active
                float elapsed = 0f;
                while (elapsed < attackDuration)
                {
                    if (meleeVisualSprite != null)
                    {
                        float t = elapsed / attackDuration;
                        float z = Mathf.LerpAngle(-200f, -80f, t);
                        meleeVisualSprite.transform.localRotation = Quaternion.Euler(0f, 0f, z);
                    }

                    elapsed += Time.deltaTime;
                    yield return null;
                }

                // Ensure final rotation at end of swing
                if (meleeVisualSprite != null)
                {
                    meleeVisualSprite.transform.localRotation = Quaternion.Euler(0f, 0f, -80f);
                }

                meleeCollider.enabled = false;
            }
        }
    }

    void SwitchToRevolver()
    {
        currentWeaponMode = WeaponMode.Revolver;
        if (playerSpriteRenderer != null)
        {
            if (weapon == WeaponType.Shotgun && shotgunSprite != null)
                playerSpriteRenderer.sprite = shotgunSprite;
            else if (revolverSprite != null)
                playerSpriteRenderer.sprite = revolverSprite;
        }

        // Hide melee visual sprite
        if (meleeVisualSprite != null)
        {
            meleeVisualSprite.SetActive(false);
        }

        Debug.Log("Switched to Revolver - Ammo: " + currentAmmo);
    }

    void SwitchToMelee()
    {
        currentWeaponMode = WeaponMode.Melee;
        if (playerSpriteRenderer != null && meleeSprite != null)
        {
            playerSpriteRenderer.sprite = meleeSprite;
        }

        // Show melee visual sprite
        if (meleeVisualSprite != null)
        {
            meleeVisualSprite.SetActive(true);
        }

        Debug.Log("Switched to Melee - Locked for " + meleeLockoutTime + " seconds");
    }

    //Update muzzle flash light intensity (fade out effect)
    void UpdateMuzzleFlash()
    {
        if (muzzleFlashLight != null && currentMuzzleIntensity > 0)
        {
            currentMuzzleIntensity -= muzzleFlashFadeSpeed * Time.deltaTime;
            if (currentMuzzleIntensity < 0) currentMuzzleIntensity = 0;
            muzzleFlashLight.intensity = currentMuzzleIntensity;
        }
    }

    // Handle footstep audio - play only when moving
    void HandleFootstepAudio()
    {
        bool isMoving = rb.linearVelocity.sqrMagnitude > 0.01f;

        if (playerAudioSource != null && footstepSFX != null)
        {
            if (isMoving)
            {
                // Play looping footstep sound if not already playing
                if (!playerAudioSource.isPlaying)
                {
                    playerAudioSource.clip = footstepSFX;
                    playerAudioSource.loop = true;
                    playerAudioSource.Play();
                }
            }
            else
            {
                // Stop footsteps when not moving
                if (playerAudioSource.isPlaying)
                {
                    playerAudioSource.Stop();
                }
            }
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
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("HitboxNotCollide") && !inmmunityFrame)
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
        healthText.text = currentHealth + "/" + maxHealth;
        StartCoroutine(InnmunityFrameTimer());
    }

    //Co-rutine for InmmunityFrames
    IEnumerator InnmunityFrameTimer()
    {
        yield return new WaitForSeconds(inmmunityFrameTimer);
        inmmunityFrame = false;
    }

    public void AddMaxHealt(float healthToAdd)
    {
        maxHealth += healthToAdd;
        currentHealth += healthToAdd;
        healthText.text = currentHealth + "/" + maxHealth;

    }

    public void AddDamage(int damageToAdd)
    {
        damage += damageToAdd;
        damageText.text = damage.ToString();
    }

    public void AddSpeed(float speedMultiplier)
    {
        speed = (speed * speedMultiplier) + speed;
        speedText.text = (speed * 10f).ToString();

    }

    public int GetDamage()
    {
        return damage;

    }

    public void Heal(int HealAmount)
    {
        if ((currentHealth + HealAmount) > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += HealAmount;
        }

        healthText.text = currentHealth + "/" + maxHealth;
        healthBar.value = currentHealth / maxHealth;

    }

    public void ChangeToShootgun()
    {
        weapon = WeaponType.Shotgun;
        SwitchToMelee();
        SwitchToRevolver();
        meleeLockoutTimer = 0.1f;
    }


}