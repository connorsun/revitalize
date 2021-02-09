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
    private Vector2 dir;
    public bool inputDisable;
    public float inputDisableTimer = -9.0f;
    public int health;
    public static int resetting;
    private bool action1Waiting;
    private bool action2Waiting;
    private float action1Timer;
    private float action2Timer;
    
    // Start is called before the first frame update
    void Start()
    {
        ObjectSetup();
        Reset();
    }

    void Reset()
    {
        health = 7;
        xinput = 0;
        prevxheld = false;
        prevxinput = 0;
        yinput = 0;
        prevyheld = false;
        prevyinput = 0;
        speed = 4.5f;
        rb.position = new Vector3(0f,0f,0f);
        rb.velocity = new Vector3(0f,0f,0f);
        xaccel = 0f;
        yaccel = 0f;
        frameCounter = 0;
        dir = new Vector2(1f,0f);
        action1Waiting = false;
        action1Timer = -9.0f;
        action2Waiting = false;
        action2Timer = -9.0f;
    }

    void ObjectSetup()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (resetting > 0) {
            resetting --;
        }
        frameCounter++;
        if (Time.time > inputDisableTimer && inputDisable) {
            inputDisable = false;
            sr.color = Color.white;
        }
        if (!inputDisable) {
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
        }
        if (Mathf.Abs(xaccel/speed) < 1 || xinput == 0) {
            xaccel = Mathf.Lerp(xaccel, speed*xinput, 0.2f);
        }
        if (Mathf.Abs(yaccel/speed) < 1 || yinput == 0) {
            yaccel = Mathf.Lerp(yaccel, speed*yinput, 0.2f);
        }
        rb.velocity = new Vector2(xaccel,yaccel);
        Vector2 prevdir = dir;
        dir = new Vector2(xaccel, yaccel);
        if (Mathf.Abs(dir.x) < 0.01f && Mathf.Abs(dir.y) < 0.01f) {
            dir = prevdir;
        }
        dir.Normalize();
        if (frameCounter % 2 == 0) {
            CreateGhost();
        }
        if (Input.GetButton("Action1") && !inputDisable) {
            if (Time.time < action2Timer) {
                LoveAttack();
            } else if (Time.time > action2Timer) {
                action1Timer = Time.time + 0.03f;
                action1Waiting = true;
            }
        }
        if (Input.GetButton("Action2") && !inputDisable) {
            if (Time.time < action1Timer) {
                LoveAttack();
            } else if (Time.time > action2Timer) {
                action2Timer = Time.time + 0.03f;
                action2Waiting = true;
            }
        }
        if (!inputDisable && Time.time > action2Timer && action2Waiting) {
            action2Waiting = false;
            action2Timer = -9.0f;
            GameObject hug = Instantiate(Resources.Load<GameObject>("Prefabs/TalkAttack") as GameObject);
            hug.transform.parent = this.transform;
            hug.transform.localPosition = new Vector3(0f,0f,0f);
            hug.GetComponent<TalkAttackRV>().parent = gameObject;
            inputDisable = true;
            inputDisableTimer = Time.time + 0.8f;
            xinput = 0;
            yinput = 0;
            sr.color = Color.blue;
        }
        if (!inputDisable && Time.time > action1Timer && action1Waiting) {
            action1Waiting = false;
            action1Timer = -9.0f;
            GameObject hug = Instantiate(Resources.Load<GameObject>("Prefabs/HugAttack") as GameObject);
            hug.transform.parent = this.transform;
            hug.transform.localPosition = dir * 0.5f;
            hug.GetComponent<HugAttackRV>().dir = dir;
            hug.GetComponent<HugAttackRV>().parent = gameObject;
            inputDisable = true;
            inputDisableTimer = Time.time + 0.5f;
            xinput = 0;
            yinput = 0;
            sr.color = Color.green;
        }
        if (health < 1) {
            Reset();
            resetting = 2;
        }
    }

    void LoveAttack() {
        action1Waiting = false;
        action1Timer = -9.0f;
        action2Waiting = false;
        action2Timer = -9.0f;
        GameObject hug = Instantiate(Resources.Load<GameObject>("Prefabs/LoveAttack") as GameObject);
        hug.transform.parent = this.transform;
        hug.transform.localPosition = new Vector3(0f,0f,0f);
        inputDisable = true;
        inputDisableTimer = Time.time + 0.3f;
        xinput = 0;
        yinput = 0;
        xaccel = dir.x*20f;
        yaccel = dir.y*20f;
        sr.color = new Color(1f, 0.77f, 0.8f, 1f);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("EnemyAttack")) {
            health -= 1;
            Destroy(other.gameObject);
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

    public void Knockback(Vector2 dir, int power) {
        xaccel = dir.x*power;
        yaccel = dir.y*power;
        inputDisable = true;
        inputDisableTimer = Time.time + 0.5f;
        xinput = 0;
        yinput = 0;
    }
}
