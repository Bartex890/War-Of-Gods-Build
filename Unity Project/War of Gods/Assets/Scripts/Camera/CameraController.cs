using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float panSpeed = 20f;
    [SerializeField]
    private float panBorderThickness = 10f;
    [SerializeField]
    private Vector3 panLimit;

    private Vector2 _mapCenter;

    [SerializeField]
    private float scrollSpeed;
    [SerializeField]
    private float minZ = -100f;
    [SerializeField]
    private float maxZ = -10f;

    private Camera camera;
    [SerializeField]
    private Camera _secondCamera;

    private bool _lockCam = false;

    private float _velocityqMultiplier;
    [SerializeField]
    private float _velocityBuildup;

    private void Start()
    {
        _mapCenter = new Vector2((float)MapManager.Instance.map.tiles.GetLength(0) / 2 - 0.5f, -(float)MapManager.Instance.map.tiles.GetLength(1) / 2 + 0.5f);
        panLimit.x = (float)MapManager.Instance.map.tiles.GetLength(0) / 2;
        panLimit.y = (float)MapManager.Instance.map.tiles.GetLength(1) / 2;
        camera = Camera.main;
    }

    private void Update()
    {
        Vector3 pos = transform.position;
        if (!_lockCam)
        {
            if (Input.GetKey("w") || Input.mousePosition.y >=Screen.height - panBorderThickness)
            {
                pos.y += panSpeed * Time.deltaTime * _velocityqMultiplier;
            }
            if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
            {
                pos.y -= panSpeed * Time.deltaTime * _velocityqMultiplier;
            }
            if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
            {
                pos.x += panSpeed * Time.deltaTime * _velocityqMultiplier;
            }
            if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
            {
                pos.x -= panSpeed * Time.deltaTime * _velocityqMultiplier;
            }
        }
        
        if (pos != transform.position)
        {
            _velocityqMultiplier += Time.deltaTime * _velocityBuildup;
        }
        else
        {
            _velocityqMultiplier = 1;
        }
        
        float scroll =camera.orthographicSize - (scrollSpeed * Input.GetAxis("Mouse ScrollWheel"));
        if (scroll < 1f)
        {
            camera.orthographicSize = 1f;
        }
        else if(scroll > 30f)
        {
            camera.orthographicSize = 30f;
        }
        else
        {
            camera.orthographicSize = scroll;
        }

        if (_secondCamera != null)
            _secondCamera.orthographicSize = camera.orthographicSize;

        pos.x = Mathf.Clamp(pos.x, _mapCenter.x - panLimit.x, _mapCenter.x + panLimit.x);
        pos.y = Mathf.Clamp(pos.y, _mapCenter.y - panLimit.y, _mapCenter.y + panLimit.y);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
        transform.position = pos;

        if (Input.GetKeyDown(KeyCode.F2))
        {
            _lockCam = !_lockCam;
            AlertManager.AddAlert("Camera movement " + (_lockCam ? "" : "un") + "locked");
        }
    }
     
    public Transform GetTransformPositionCamera()
    {
        return transform;
    }
     
    public void SetTransformPosition(Vector3 position)
    {
        transform.position = position;
    }
}