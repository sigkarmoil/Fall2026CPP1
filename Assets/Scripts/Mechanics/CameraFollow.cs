using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region Camera State
    public enum CameraState
    {
        None,
        Follow,
        Bound,
        Transition
    }
    #endregion

    #region public variables
    public CameraState currentState = CameraState.Follow;

    public float minYPos;   // lowest Y the camera may follow to
    public float maxYPos;   // highest Y the camera may follow to

    public Transform target;        // always the player
    public float cameraSpeed = 5f;

    [Header("Bound")]
    [SerializeField] Transform boundAnchor;   // empty object where the camera sits (keep it at scene root)
    [SerializeField] float boundZoom = 3f;    // orthographic size while bound
    [SerializeField] float zoomSpeed = 4f;
    #endregion

    Camera cam;
    float defaultZoom;

    void Awake()
    {
        cam = GetComponent<Camera>();   // script must be on the Camera object
        defaultZoom = cam.orthographicSize;
    }

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("CameraFollow: no Player found. Assign a target.");
                return;
            }
            target = player.transform;
        }
    }

    void LateUpdate()
    {
        switch (currentState)
        {
            case CameraState.Follow: UpdateFollow(); break;
            case CameraState.Bound: UpdateBound(); break;
        }
    }

    void UpdateFollow()
    {
        if (target == null) return;

        Vector3 dest = transform.position;
        dest.x = target.position.x;
        dest.y = Mathf.Clamp(target.position.y, minYPos, maxYPos);

        transform.position = Vector3.MoveTowards(transform.position, dest, cameraSpeed * Time.deltaTime);
        cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, defaultZoom, zoomSpeed * Time.deltaTime);
    }

    void UpdateBound()
    {
        if (boundAnchor == null) return;

        Vector3 dest = boundAnchor.position;
        dest.z = transform.position.z;   // keep the camera's own Z

        transform.position = Vector3.MoveTowards(transform.position, dest, cameraSpeed * Time.deltaTime);
        cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, boundZoom, zoomSpeed * Time.deltaTime);
    }

    public void snapTo(Vector2 position)
    {
        transform.position = new Vector3(
            position.x,
            Mathf.Clamp(position.y, minYPos, maxYPos),
            transform.position.z);
    }

    // Call from a trigger zone or another script
    public void SetBound(Transform anchor)
    {
        boundAnchor = anchor;
        currentState = CameraState.Bound;
    }

    public void SetFollow()
    {
        currentState = CameraState.Follow;
    }
}