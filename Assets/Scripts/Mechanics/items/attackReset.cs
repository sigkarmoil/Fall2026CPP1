
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class getPower : Pickup
{
    [Header("Movement")]
    [SerializeField] private float amplitude = 1f;      // sine height in units
    [SerializeField] private float frequency = 1f;      // cycles per second
    [SerializeField] private float phaseOffset = 0f;    // radians; shifts the starting point on the wave

    private Rigidbody2D rb;
    private float baseY;
    private float elapsed;

    public override void OnPlayerPickup(PlayerController interactor)
    {
        if (interactor != null)
        {
            interactor.currentPower += 1;
            Debug.Log($"Current Power = {interactor.currentPower}");
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        baseY = rb.position.y;
    }

    void FixedUpdate()
    {
        elapsed += Time.fixedDeltaTime;

        float y = baseY + amplitude * Mathf.Sin(2f * Mathf.PI * frequency * elapsed + phaseOffset);

        rb.MovePosition(new Vector2(rb.position.x, y));
    }
}