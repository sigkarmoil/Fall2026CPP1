using UnityEngine;

public class flipAttackBox : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Collider2D attackBox;
    [SerializeField] private float xOffset = 0.211f;

    private void LateUpdate()
    {
        Transform box = attackBox.transform;
        Vector3 pos = box.localPosition;
        pos.x = sr.flipX ? -xOffset : xOffset;
        box.localPosition = pos;
    }
}