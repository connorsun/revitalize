using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinCryRV : MonoBehaviour
{
    public float dir;
    private float initstamp;
    private Rigidbody2D rb;
    private float speed;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        speed = 5;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        speed -= 0.1f;
        if (speed < 0.05f) {
            Destroy(gameObject);
        }
        rb.velocity = new Vector2(Mathf.Cos(dir)*speed, Mathf.Sin(dir)*speed);
    }
}
