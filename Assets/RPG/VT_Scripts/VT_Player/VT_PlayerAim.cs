using System;
using UnityEngine;

public class VT_PlayerAim : MonoBehaviour
{
    private VT_Player player;
    private PlayerControls controls;

    [Header("Aim Visual - Laser")]
    [SerializeField] private LineRenderer aimLaser;


    [Header("Aim Control")]
    [SerializeField] private Transform aim;

    [SerializeField] private bool isAimingPrecisly;
    [SerializeField] private bool isLockingToTarget;

    [Header("Camera Control")]
    [SerializeField] private Transform cameraTarget; // Transform để định vị điểm ngắm
    [Range(.5f, 1)]
    [SerializeField] private float minCameraDistance = 1.5f;
    [Range(1, 3f)]
    [SerializeField] private float maxCameraDistance = 4;
    [Range(3f, 5f)]
    [SerializeField] private float cameraSensetivity = 5f; // Độ nhạy

    [Space]

    [SerializeField] private LayerMask aimLayerMask; // LayerMask để xác định lớp nào có thể bị ngắm bắn
    [SerializeField] private Vector3 lookingDirection; // Vector3 để lưu hướng nhìn hiện tại

    private Vector2 mouseInput;
    private RaycastHit lastKnownMouseHit;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<VT_Player>();
        AssignInputEvents();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAimVisuals();
        UpdateAimPosition();
        UpdateCameraPosition();
    }

    private void UpdateAimVisuals()
    {
        Transform gunPoint = player.weapon.GunPoint();
        Vector3 laserDirection = player.weapon.BulletDirection();

        float laserTipLenght = .5f;
        float gunDistance = 4f;

        Vector3 endPoint = gunPoint.position + laserDirection * gunDistance;

        if (Physics.Raycast(gunPoint.position, laserDirection, out RaycastHit hitInfo, gunDistance))
        {
            endPoint = hitInfo.point;
            laserTipLenght = 0;
        }

        aimLaser.SetPosition(0, gunPoint.position); 
        aimLaser.SetPosition(1, endPoint); 
        aimLaser.SetPosition(2, endPoint + laserDirection * laserTipLenght); 

    }

    private void UpdateAimPosition()
    {
        Transform target = Target();

        if (target != null && isLockingToTarget)
        {
            if (target.GetComponent<Renderer>() != null)
            {
                aim.position = target.GetComponent<Renderer>().bounds.center; // Ngắm vào trung tâm của đối tượng

            } else
            {
                aim.position = target.position;
            }

            return;
        }

        // Cập nhật vị trí của điểm ngắm
        aim.position = GetMouseHitInfo().point;

        // Nếu không thể ngắm chính xác, giữ y cố định để tránh điểm ngắm bị chạm đất
        if (isAimingPrecisly == false)
        {
            aim.position = new Vector3(aim.position.x, transform.position.y + 1, aim.position.z); // Giữ y cố định để tránh điểm ngắm bị chạm đất
        }

    }

    public Transform Target()
    {
        Transform target = null;    

        if (GetMouseHitInfo().transform.GetComponent<VT_Target>() != null)
        {
            target = GetMouseHitInfo().transform;
        }

        return target;
    }

    public Transform Aim() => aim;

    // Phương thức để kiểm tra nếu có thể ngắm chính xác
    public bool CanAimPrecisly()
    {
        if (isAimingPrecisly)
        {
            return true;
        }

        return false;
    }

    #region Camera Region
    private void UpdateCameraPosition()
    {
        cameraTarget.position =
                    Vector3.Lerp(cameraTarget.position, DesieredCameraPosition(), cameraSensetivity * Time.deltaTime);
    }

    private Vector3 DesieredCameraPosition()
    {
        float actualMaxCameraDistance;
        bool movingDownwards = player.movement.moveInput.y < -.5f; // Kiểm tra nếu nhân vật đang di chuyển xuống dưới

        if (movingDownwards)
        {

            actualMaxCameraDistance = minCameraDistance;
        }
        else
        {
            actualMaxCameraDistance = maxCameraDistance;
        }
        // OR
        // float actualMaxCameraDistance = player.movement.moveInput.y < -.5f ? minCameraDistance : maxCameraDistance;

        Vector3 desiredCameraPosition = GetMouseHitInfo().point;

        Vector3 aimDirection = (desiredCameraPosition - transform.position).normalized;

        float distanceToDesiredAimPosition = Vector3.Distance(transform.position, desiredCameraPosition);

        // Hàm Mathf.Clamp để giới hạn khoảng cách giữa vị trí nhân vật và điểm ngắm trong khoảng minCameraDistance và maxCameraDistance
        float clampedDistance = Mathf.Clamp(distanceToDesiredAimPosition, minCameraDistance, actualMaxCameraDistance);

        //if (distanceToDesiredAimPosition < minCameraDistance)
        //{
        //    desiredAimPosition = transform.position + aimDirection * minCameraDistance;
        //}
        //else if (distanceToDesiredAimPosition > maxCameraDistance)
        //{
        //    desiredAimPosition = transform.position + aimDirection * maxCameraDistance;
        //}

        desiredCameraPosition = transform.position + aimDirection * clampedDistance;
        desiredCameraPosition.y = transform.position.y + 1;

        return desiredCameraPosition;
    }
    #endregion

    public RaycastHit GetMouseHitInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(mouseInput);

        // Thực hiện raycast để tìm điểm va chạm trên các lớp được chỉ định trong aimLayerMask
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, aimLayerMask))
        {
            lastKnownMouseHit = hitInfo;
            return hitInfo;
        }

        return lastKnownMouseHit;
    }

    private void AssignInputEvents()
    {
        controls = player.controls;

        // Ngắm bắn
        controls.VT_Character.Aim.performed += context => mouseInput = context.ReadValue<Vector2>();
        controls.VT_Character.Aim.canceled += context => mouseInput = Vector2.zero;
    }
}
