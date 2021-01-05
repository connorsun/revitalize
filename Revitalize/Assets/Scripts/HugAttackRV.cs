using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HugAttackRV : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject parent;
    public Vector2 dir;
    private float initStamp;
    void Start()
    {
        initStamp = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time > initStamp + 0.5f) {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Enemy")) {
            parent.transform.position = (Vector3) ((Vector2) other.transform.position - dir * 0.3f);
            parent.GetComponent<Rigidbody2D>().velocity = new Vector3(0f, 0f, 0f);
            parent.GetComponent<DungeonPlayerRV>().xaccel = 0;
            parent.GetComponent<DungeonPlayerRV>().yaccel = 0;
        }
    }
}
