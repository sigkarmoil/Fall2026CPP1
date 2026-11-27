using System.Collections;
using UnityEngine;

// Attach to the player. Call respawn() from a hazard (e.g. SpikeReset).
// Sequence: lock control -> fadeToBlack -> teleport + snap camera -> hold black
//           -> fadeToNormal -> (after controlDelay) unlock control.
//
// Needs on PlayerController:  public bool canControl, public Vector2 teleportLocation, public void cancelActions()
// Needs on CameraFollow:      public void snapTo(Vector2 position)
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Rigidbody2D))]
public class respawnPlayer : MonoBehaviour
{
    #region References
    [Header("References")]
    [SerializeField] private CanvasGroup fadeGroup;       // the CanvasGroup on the black FadeImage
    [SerializeField] private CameraFollow cameraFollow;   // optional, found automatically if left empty
    #endregion

    #region Timing (seconds, unscaled so hit-stop does not stall the fade)
    [Header("Timing (seconds)")]
    [SerializeField, Min(0f)] private float fadeToBlackTime = 0.5f;
    [SerializeField, Min(0f)] private float holdBlackTime = 0.3f;
    [SerializeField, Min(0f)] private float fadeToNormalTime = 0.5f;
    [SerializeField, Min(0f)] private float controlDelay = 0.2f; // counted from the start of fadeToNormal
    #endregion

    private int respawnCount;
    // Read-only flag. Derived from respawnCount so there is no second variable to keep in sync.
    public bool isRespawning => respawnCount > 0;

    [SerializeField] int damagePerHit = 10;

    private PlayerController player;
    private Rigidbody2D rb;


    void Awake()
    {
        player = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();

        if (cameraFollow == null)
            cameraFollow = FindFirstObjectByType<CameraFollow>();// TBR
    }

    // Entry point. Only the first call of a sequence runs; later calls are counted and ignored.
    public void respawn()
    {
        if (fadeGroup == null)
        {
            Debug.LogError("respawnPlayer: fadeGroup is not assigned.");
            return;
        }

        ++respawnCount;
        if (respawnCount != 1) return;

        // Exactly one damage tick per respawn-causing contact.
        player.currentHealth = Mathf.Max(0, player.currentHealth - damagePerHit);

        if (player.currentHealth > 0)
        {
            StartCoroutine(respawnRoutine());
            Debug.Log($"Player Health: {player.currentHealth}");
        }
        else
        {
            GameManager.Instance.LoadGameOver();
        }
    }

    private IEnumerator respawnRoutine()
    {
        // Lock the player: no input, no leftover boost/attack, no falling during the fade.
        player.playHurt();
        player.canControl = false;
       
        player.cancelActions();
        RigidbodyType2D previousBodyType = rb.bodyType;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        yield return fadeToBlack();

        // Screen is fully black here, so the jump is never seen.
        rb.position = player.teleportLocation;
        rb.linearVelocity = Vector2.zero;
        if (cameraFollow != null)
            cameraFollow.snapTo(player.teleportLocation);
        yield return new WaitForFixedUpdate(); // let the Transform catch up with the Rigidbody2D

        yield return new WaitForSecondsRealtime(holdBlackTime);

        StartCoroutine(fadeToNormal());
        yield return new WaitForSecondsRealtime(controlDelay);
        rb.position = player.teleportLocation;
        player.boostTargetY = rb.position.y;   // nothing stale left for MoveTowards to chase
        rb.bodyType = previousBodyType;
        respawnCount = 0;           // right before control returns
        player.canControl = true;
    }

    private IEnumerator fadeToBlack() => fade(0f, 1f, fadeToBlackTime);

    private IEnumerator fadeToNormal() => fade(1f, 0f, fadeToNormalTime);

    private IEnumerator fade(float from, float to, float duration)
    {
        fadeGroup.alpha = from;
        if (duration <= 0f)
        {
            fadeGroup.alpha = to;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            fadeGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        fadeGroup.alpha = to;
    }
}