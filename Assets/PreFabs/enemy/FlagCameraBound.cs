using UnityEngine;

public class FlagCameraBound : MonoBehaviour
{
    [SerializeField] private CameraFollow attachedCamera;

    private bool used;

    private void Awake()
    {
        // Fallback if the Inspector slot is empty.
        if (attachedCamera == null && Camera.main != null)
        {
            attachedCamera = Camera.main.GetComponent<CameraFollow>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used || !other.CompareTag("Player"))
            return;

        if (attachedCamera == null)
        {
            Debug.LogError("FlagCameraBound: no CameraFollow assigned.", this);
            return;
        }

        attachedCamera.currentState = CameraFollow.CameraState.Bound;
        used = true;
    }
}