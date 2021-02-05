using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoveAttackRV : MonoBehaviour
{
    // Start is called before the first frame update
    private float initStamp;
    public Vector2 dir;
    void Start()
    {
        initStamp = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time > initStamp + 0.3f) {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Enemy")) {
            dir = other.gameObject.transform.position - transform.position;
            dir.Normalize();
        }
    }
}
