using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerRV : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.position = new Vector3(0f,0f,-10f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
