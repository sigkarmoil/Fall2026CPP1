using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FloatingCholesterol : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;      // drag player_janitor
    [SerializeField] private Collider2D playerAttackBox;   // drag the playerAttack child

    [Header("Tunable")]
    [SerializeField] private int maxHealth = 1;
    //[SerializeField] private float respawnTime = 1.5f;

    [Header("Bounce")]
    [SerializeField] private float bounceDuration = 1.5f;

    private static readonly float[] bounceKeys = { 0.8f, 1.1f, 0.9f, 1f };

    private Rigidbody2D floatingRb;
    private int currentHealth;
    private Vector3 baseScale;
    private Coroutine bounceRoutine;

    private void Awake()
    {
        floatingRb = GetComponent<Rigidbody2D>();
        baseScale = transform.localScale;
        currentHealth = maxHealth;

        // Fallback if the drag is lost (for example, on a prefab).
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                player = playerObj.GetComponent<PlayerController>();
        }

        if (player == null)
            Debug.LogWarning($"{name}: PlayerController not found.", this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (player == null || other != playerAttackBox) return;

        PlayBounce();
        player.freezeTimePellet();
        player.isAttackOff();
        player.startBoost();
        player.isAttack = true;
       
        Debug.Log("cholesterol boost", this);
    }

    private void PlayBounce()
    {
        if (bounceRoutine != null) StopCoroutine(bounceRoutine);
        transform.localScale = baseScale;
        bounceRoutine = StartCoroutine(BounceRoutine());
    }

    private IEnumerator BounceRoutine()
    {
        float segment = bounceDuration / bounceKeys.Length;
        float from = 1f;

        foreach (float key in bounceKeys)
        {
            float elapsed = 0f;
            while (elapsed < segment)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / segment);
                transform.localScale = baseScale * Mathf.Lerp(from, key, t);
                yield return null;
            }
            from = key;
        }

        transform.localScale = baseScale;
        bounceRoutine = null;
    }
}