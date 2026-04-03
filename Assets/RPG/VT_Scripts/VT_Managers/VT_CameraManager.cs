using Cinemachine;
using UnityEngine;

public class VT_CameraManager : MonoBehaviour
{
    public static VT_CameraManager instance;

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer transposer;

    [Header("Camera distance")]
    [SerializeField] private bool canChangeCameraDistance = false;
    [SerializeField] private float distanceChangeRate;
    private float targetCameraDistance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Update()
    {
        UpdateCameraDistance();

    }

    private void UpdateCameraDistance()
    {
        if (canChangeCameraDistance == false)
        {
            return;
        }

        float currentDistance = transposer.m_CameraDistance;

        if (Mathf.Abs(targetCameraDistance - currentDistance) < .01f)
        {
            return;
        }

        transposer.m_CameraDistance = Mathf.Lerp(currentDistance, targetCameraDistance, distanceChangeRate * Time.deltaTime);
    }

    public void ChangeCameraDistance(float distance)
    {
        targetCameraDistance = distance;
    }

}
