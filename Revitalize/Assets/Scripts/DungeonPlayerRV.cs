using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonPlayerRV : MonoBehaviour
{
    public float xinput;
    private bool prevxheld;
    private float prevxinput;
    public float yinput;
    private bool prevyheld;
    private float prevyinput;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    public float speed;
    public float xaccel;
    public float yaccel;
    private int frameCounter;
    
    // Start is called before the first frame update
    void Start()
    {
        ObjectSetup();
        Reset();
    }

    void Reset()
    {
        xinput = 0;
        prevxheld = false;
        prevxinput = 0;
        yinput = 0;
        prevyheld = false;
        prevyinput = 0;
        speed = 5;
        rb.position = new Vector3(0f,0f,0f);
        rb.velocity = new Vector3(0f,0f,0f);
        xaccel = 0f;
        yaccel = 0f;
        frameCounter = 0;
    }

    void ObjectSetup()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        frameCounter++;
        if (Input.GetButton("Left")  && !Input.GetButton("Right")) {
            xinput = -1f;
            prevxheld = false;
        } else if (Input.GetButton("Right")  && !Input.GetButton("Left")) {
            xinput = 1f;
            prevxheld = false;
        } else if (Input.GetButton("Left")  && Input.GetButton("Right")) {
            if (prevxheld == false) {
                xinput = prevxinput*-1f;
            } else {
                xinput = prevxinput;
            }
            prevxheld = true;
        } else {
            xinput = 0f;
            prevxheld = false;
        }
        if (Input.GetButton("Down")  && !Input.GetButton("Up")) {
            yinput = -1f;
            prevyheld = false;
        } else if (Input.GetButton("Up")  && !Input.GetButton("Down")) {
            yinput = 1f;
            prevyheld = false;
        } else if (Input.GetButton("Down")  && Input.GetButton("Up")) {
            if (prevyheld == false) {
                yinput = prevyinput*-1f;
            } else {
                yinput = prevyinput;
            }
            prevyheld = true;
        } else {
            yinput = 0f;
            prevyheld = false;
        }
        if (Mathf.Abs(xaccel/speed) < 1 || xinput == 0) {
            xaccel = Mathf.Lerp(xaccel, speed*xinput, 0.2f);
        }
        if (Mathf.Abs(yaccel/speed) < 1 || yinput == 0) {
            yaccel = Mathf.Lerp(yaccel, speed*yinput, 0.2f);
        }
        rb.velocity = new Vector2(xaccel,yaccel);
        if (frameCounter % 2 == 0) {
            CreateGhost();
        }
        if (Input.GetButtonDown("Action1")) {
            //Instantiate(Resources.Load<GameObject>("HugAttack") as GameObject);
        }
    }

    void CreateGhost() {
        GameObject ghost = new GameObject();
        ghost.transform.position = transform.position;
        SpriteRenderer ghostsr = ghost.AddComponent<SpriteRenderer>();
        ghostsr.sprite = sr.sprite;
        ghostsr.sortingOrder = -3;
        ghost.AddComponent<FaderRV>();
        ghost.name = "playerGhost";
    }
}
