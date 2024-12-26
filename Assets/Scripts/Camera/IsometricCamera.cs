using UnityEngine;

public class IsometricCamera : MonoBehaviour
{
    private Camera _camera;

    [Header("Movement")]
    public float moveSpeed;

    //public Vector2 limitX;
    //public Vector2 limitZ;

    [Space(10)]
    [Header("Zoom")]
    public float zoomSpeed;
    public float zoomSmoothness;

    public float minZoom;
    public float maxZoom;

    private float _currentZoom;

    [Space(10)]
    [Header("Rotation")]
    public float rotationSpeed;


    public void Awake()
    {
        _camera = GetComponentInChildren<Camera>();
        _currentZoom = _camera.orthographicSize;
    }

    public void Update()
    {
        Move();
        Zoom();
        Rotation();
    }

    private void Move()
    {
        Vector2 position = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        transform.position += Quaternion.Euler(0, _camera.transform.eulerAngles.y, 0) * new Vector3(position.x, 0, position.y) * (moveSpeed * Time.deltaTime);
        //transform.position = new Vector3(Mathf.Clamp(transform.position.x, limitX.x, limitX.y), transform.position.y, Mathf.Clamp(transform.position.z, limitZ.x, limitZ.y));
    }

    private void Zoom()
    {
        _currentZoom = Mathf.Clamp(_currentZoom + Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime, minZoom, maxZoom);
        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _currentZoom, zoomSmoothness * Time.deltaTime);
    }

    private void Rotation()
    {
        if (Input.GetMouseButton(1))
        {
            float mouseDeltaX = Input.GetAxis("Mouse X");

            transform.Rotate(Vector3.up, mouseDeltaX * rotationSpeed * Time.deltaTime);
        }
    }
}
//TODO: bir tuþ ile kamera açýsý defaulta geri dönsün gibisine bir görev => MAÖ
