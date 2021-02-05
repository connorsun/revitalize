using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkAttackRV : MonoBehaviour
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
        if (Time.time > initStamp + 0.8f) {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Enemy")) {
            dir = other.gameObject.transform.position - transform.position;
            dir.Normalize();
            parent.GetComponent<Rigidbody2D>().velocity = new Vector3(0f, 0f, 0f);
            parent.GetComponent<DungeonPlayerRV>().xaccel = 0;
            parent.GetComponent<DungeonPlayerRV>().yaccel = 0;
            parent.GetComponent<DungeonPlayerRV>().inputDisableTimer = Time.time + 0.8f;
            parent.GetComponent<DungeonPlayerRV>().xinput = 0;
            parent.GetComponent<DungeonPlayerRV>().yinput = 0;
        
        }
    }
}
