using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaderRV : MonoBehaviour
{
    // Start is called before the first frame update
    private SpriteRenderer sr;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(1f,1f,1f,1f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (sr.color.a > 0.005f) {
            sr.color = new Color(1f,1f,1f,sr.color.a - 0.15f);
        } else {
            Destroy(gameObject);
        }
    }
}
