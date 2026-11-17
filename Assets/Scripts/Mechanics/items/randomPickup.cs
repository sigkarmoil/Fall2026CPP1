using UnityEngine;

public enum pickupState
{
    health,
    power
}

public class randomPickup : Pickup
{
    [Header("Movement")]
    [SerializeField] private float amplitude = 1f;      // sine height in units
    [SerializeField] private float frequency = 1f;      // cycles per second
    [SerializeField] private float phaseOffset = 0f;    // radians; shifts the starting point on the wave

    private Rigidbody2D rb;
    private float baseY;
    private float elapsed;

    private pickupState state;
    private Animator anim;

    public override void OnPlayerPickup(PlayerController interactor)
    {
        if (interactor == null) return;

        switch (state)
        {
            case pickupState.power:
                interactor.currentPower += 1;
                Debug.Log($"Current Power = {interactor.currentPower}");
                break;

            case pickupState.health:
                interactor.currentHealth += 1;
                Debug.Log($"Current Health = {interactor.currentHealth}");
                break;
        }
    }

    void Start()
    {
        // Random pick from every value in the enum
        var values = (pickupState[])System.Enum.GetValues(typeof(pickupState));
        state = values[Random.Range(0, values.Length)];

        // Works whether the Animator is on this object or a child (e.g. the sprite)
        anim = GetComponent<Animator>();
        if (anim != null)
        {
            switch (state)
            {
                case pickupState.power:
                    anim.SetTrigger("power");
                    break;

                case pickupState.health:
                    anim.SetTrigger("health");
                    break;
            }
        }
        else
        {
            Debug.LogWarning($"{name}: no Animator found for randomPickup", this);
        }

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
