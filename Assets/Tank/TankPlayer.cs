using UnityEngine;

public class TankPlayer : MonoBehaviour
{
    //public Rigidbody rb;
    private Rigidbody rb;

    [Header("Gun data")]
    [SerializeField] private Transform gunPointTransform; // Reference to the gun point's transform
    [SerializeField] private float bulletSpeed;
    [SerializeField] private GameObject bulletPrefab;

    [Header("Movement data")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;

    private float verticalInput;
    private float horizontalInput;

    [Header("Tower data")]
    [SerializeField] private Transform tankTowerTransform; // Reference to the tower's transform  
    [SerializeField] private float towerRotationSpeed;

    [Header("Aim data")]
    // LayerMask to specify what layers the player can aim at
    [SerializeField] private LayerMask whatIsAimMask;
    [SerializeField] private Transform aimTransform;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Player Start");

        // Get the Rigidbody component attached to the player
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAim();
        CheckInputs();
    }

    // Check for player input and update the vertical and horizontal input values
    private void CheckInputs()
    {

        //if (Input.GetButtonDown("Fire1")) Or
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();
        }

        // Get the vertical and horizontal input values from the Input system
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");

        // Di chuyển lùi về phía sau nếu nhấn phím S
        if (verticalInput < 0)
        {
            // Invert the horizontal input to rotate in the opposite direction when moving backward
            horizontalInput = -Input.GetAxis("Horizontal");
        }
    }

    private void FixedUpdate()
    {

        ApplyForwardMovement();
        ApplyTankBodyRotation();
        ApplyTankTowerRotation();

    }

    // Method to handle shooting logic
    private void Shoot()
    {
        // Instantiate a bullet at the gun point's position and rotation
        GameObject bullet = Instantiate(bulletPrefab, gunPointTransform.position, gunPointTransform.rotation); // Get a bullet from the pool
    
        bullet.GetComponent<Rigidbody>().velocity = gunPointTransform.forward * bulletSpeed; // Set the bullet's velocity to move forward


        Destroy(bullet, 7); // Destroy the bullet after 7 seconds to prevent memory leaks
    }

    // Apply rotation to the tank tower to look at the aim point
    private void ApplyTankTowerRotation()
    {
        // Calculate the direction from the tower to the aim point
        Vector3 direction = aimTransform.position - tankTowerTransform.position;
        direction.y = 0; // Keep the tower's Y rotation fixed

        // Calculate the target rotation for the tower to look at the aim point
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Rotate the tower towards the target rotation at a specified speed
        tankTowerTransform.rotation = Quaternion.RotateTowards(tankTowerTransform.rotation, targetRotation, towerRotationSpeed);
    }

    // Apply rotation to the tank body based on horizontal input
    private void ApplyTankBodyRotation()
    {
        // Rotate the player based on horizontal input
        transform.Rotate(0, horizontalInput * rotationSpeed, 0);
    }

    // Apply forward movement based on vertical input
    private void ApplyForwardMovement()
    {
        // Move the player forward based on vertical input
        Vector3 forwardMovement = transform.forward * verticalInput * moveSpeed;
        rb.velocity = forwardMovement;
    }

    private void UpdateAim()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, whatIsAimMask))
        {
            // aimTransform.position = hit.point;

            float fixedY = aimTransform.position.y; // Keep the Y position fixed
            aimTransform.position = new Vector3(hit.point.x, fixedY, hit.point.z);

        }


    }
}
