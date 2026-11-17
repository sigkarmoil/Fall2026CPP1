using System;
using UnityEngine;

// Drop on any entity that has a CircleCollider2D set to Is Trigger.
// A tagged target entering the circle enables the raycast; the ray is cast
// every physics step and its length always equals the circle's world radius.
[RequireComponent(typeof(CircleCollider2D))]
public class LineOfSightSensor2D : MonoBehaviour
{
    [SerializeField] private string targetTag = "Player";

    [Tooltip("Layers the ray can hit: the target's layer plus anything that blocks sight (ground). " +
             "Exclude this entity's own layer, or the ray can hit itself.")]
    [SerializeField] private LayerMask hitMask;

    public event Action LineOfSightGained;
    public event Action LineOfSightLost;

    public bool HasLineOfSight { get; private set; }

    // Normalized. Updated every physics step while a target is inside the circle.
    public Vector2 DirectionToTarget { get; private set; }

    public Transform Target { get; private set; }

    private CircleCollider2D range;
    private Collider2D targetCollider;
    private bool canRaycast;

    private void Awake() => range = GetComponent<CircleCollider2D>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(targetTag)) return;

        Target = other.transform;
        targetCollider = other;
        canRaycast = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(targetTag)) return;

        ResetState();
    }

    // A disabled object gets no exit callbacks, so clean up here.
    private void OnDisable() => ResetState();

    private void FixedUpdate()
    {
        if (!canRaycast) return;

        // Target destroyed while inside the circle.
        if (Target == null)
        {
            ResetState();
            return;
        }

        Vector2 origin = transform.position;

        // Aim at the collider center, not the pivot (a pivot at the feet aims at the floor).
        Vector2 aimPoint = targetCollider != null ? (Vector2)targetCollider.bounds.center : (Vector2)Target.position;
        DirectionToTarget = (aimPoint - origin).normalized;

        // bounds.extents.x of a circle = world radius, scale included. Read live, so it always matches.
        float distance = range.bounds.extents.x;

        RaycastHit2D hit = Physics2D.Raycast(origin, DirectionToTarget, distance, hitMask);

        // hit.transform is the Rigidbody2D's transform when one exists (the player root),
        // so a hit on a child collider such as the attack box still counts as the target.
        SetLineOfSight(hit.collider != null && hit.transform == Target);
    }

    private void ResetState()
    {
        canRaycast = false;
        Target = null;
        targetCollider = null;
        SetLineOfSight(false);
    }

    private void SetLineOfSight(bool value)
    {
        if (HasLineOfSight == value) return;

        HasLineOfSight = value;
        if (value) LineOfSightGained?.Invoke();
        else LineOfSightLost?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        CircleCollider2D c = GetComponent<CircleCollider2D>();
        if (c == null) return;

        Gizmos.color = HasLineOfSight ? Color.red : Color.cyan;
        Gizmos.DrawWireSphere(c.bounds.center, c.bounds.extents.x);

        if (canRaycast)
            Gizmos.DrawRay(transform.position, DirectionToTarget * c.bounds.extents.x);
    }
}
