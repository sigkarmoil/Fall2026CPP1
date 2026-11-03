using UnityEngine;

public class crystalAbsorb : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private SpriteRenderer sr;
    private CapsuleCollider2D capsule2d;

    //private float srTranparency = 1.00f;


    void Start()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        absorbCrystal();
    }

    // Update is called once per frame
    public void absorbCrystal(  )
    {
        Color c = sr.color;
        c.a = 0f;
        sr.color = c;
        Debug.Log("absorbed crystal");

        capsule2d.enabled = false;
    }
}
