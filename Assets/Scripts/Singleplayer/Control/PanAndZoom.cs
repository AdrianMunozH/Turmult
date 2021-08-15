using Cinemachine;
using UnityEngine;

public class PanAndZoom : MonoBehaviour
{
    [SerializeField] private float panSpeed = 20f;
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float zoomInMax = 30f;
    [SerializeField] private float zoomOutMax = 70f;
    [SerializeField] private float leftCamBorder = -60f;
    [SerializeField] private float rightCamBorder = 170f;
    [SerializeField] private float upperCamBorder = 20f;
    [SerializeField] private float lowerCamBorder = -220f;
    
    
    private CinemachineInputProvider _inputProvider;
    private CinemachineVirtualCamera _virtualCamera;
    private Transform _cameraTransform;
    private Quaternion _cameraRotation;
    private Vector3 _cameraPosition;
    
    // Start is called before the first frame update
    private void Awake()
    {
        _inputProvider = GetComponent<CinemachineInputProvider>();
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _cameraTransform = _virtualCamera.VirtualCameraGameObject.transform;
        _cameraPosition = _virtualCamera.VirtualCameraGameObject.transform.position;
        _cameraRotation = _cameraTransform.rotation;
    }
    
    public void ZoomScreen(float increment)
    {
        float fov = _virtualCamera.m_Lens.FieldOfView;
        float target = Mathf.Clamp(fov+increment, zoomInMax,zoomOutMax);
        
        _virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(fov, target, zoomSpeed * Time.deltaTime);
        


    }

    // Update is called once per frame
    void Update()
    {
        float x = _inputProvider.GetAxisValue(0);
        float y = _inputProvider.GetAxisValue(1);
        float z = _inputProvider.GetAxisValue(2);
        if (x != 0 || y != 0)
        {
            PanScreen(x,y);
        }

        if (z != 0)
        {
            ZoomScreen(z);
        }
        
    }

    public Vector3 PanDirection(float x, float y)
    {
        Vector3 direction = Vector2.zero;

        if (x >= Screen.width * 0.95f)
        {
            direction.x += 1;
        }

        if (x <= Screen.width * 0.05f)
        {
            direction.x -= 1;
        }
        
        if (y >= Screen.height * 0.95f)
        {
            direction.z += 1;
        }

        if (y <= Screen.height * 0.05f)
        {
            direction.z -= 1;
        }

        return direction;
    }

    public void PanScreen(float x, float y)
    {
        Vector3 direction = _cameraRotation * PanDirection(x, y);
        direction.y = 0;
        var position = _cameraTransform.position;
        var positionTarget = position;

        positionTarget.x = Mathf.Clamp(positionTarget.x +direction.x * panSpeed,leftCamBorder,rightCamBorder);
        positionTarget.z = Mathf.Clamp(positionTarget.z +direction.z * panSpeed,lowerCamBorder,upperCamBorder);

        position = Vector3.Lerp(position, positionTarget, Time.deltaTime);
        

        _cameraTransform.position = position;
    }
}
