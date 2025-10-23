using UnityEngine;

public class LightAim : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool smoothRotation = true;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float offsetAngle = 90f; // Adjust if your light sprite/object points in a different direction

    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Debug.LogError("LightAim: No main camera found!");
        }
    }

    void Update()
    {
        if (_mainCamera == null) return;

        // Get mouse position in world space
        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = transform.position.z; // Keep same Z-depth for 2D



        // Calculate direction from light to mouse
        Vector2 direction = (mouseWorldPos - transform.position).normalized;
        
        if (direction.sqrMagnitude < 1f)  // 0.05f ≈ distancia mínima, puedes ajustar
            return;

        // Calculate the angle in degrees
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + offsetAngle;

        if (smoothRotation)
        {
            // Smoothly rotate towards the target angle
            float currentAngle = transform.eulerAngles.z;
            float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, newAngle);
        }
        else
        {
            // Instantly point to mouse
            transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        }
    }
}
