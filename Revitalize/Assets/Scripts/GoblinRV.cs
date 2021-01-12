using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GoblinRV : MonoBehaviour
{
    // Start is called before the first frame update
    public int emotionType;
    private Tilemap tm;
    private Rigidbody2D rb;
    private Tile wallTile;
    private int roomWidth;
    private int roomHeight;
    private GameObject player;
    public float speed;
    private int hitByType;
    public bool stop;
    private float stopTimer;
    private bool attackStunned;
    public int health;
    private Vector2 savePushbackDir;
    private float attackStamp;
    public Vector2 nextNode;
    private Vector2 prevPos;
    private float stoppedTimer = -9.0f;
    
    void Start()
    {
        emotionType = Random.Range(0,2);
        tm = GameObject.FindWithTag("Walls").GetComponent<Tilemap>();
        roomWidth = 36;
        roomHeight = 20;
        speed = 1f;
        rb = GetComponent<Rigidbody2D>();
        wallTile = Resources.Load<Tile>("blacksquare");
        player = GameObject.FindWithTag("Player");
        health = 1;
        attackStamp = Time.time;
        nextNode = Pathfind(rb.position, player.transform.position);
        prevPos = rb.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!stop) {
            if (Mathf.Abs(prevPos.x-rb.position.x) < 0.05 && Mathf.Abs(prevPos.y-rb.position.y) < 0.05) {
                if (Time.time > stoppedTimer + 0.5f) {
                    stoppedTimer = Time.time;
                }
            } else if (Time.time < stoppedTimer + 0.5f) {
                stoppedTimer = Time.time;
            }
            if (Mathf.Abs(nextNode.x-rb.position.x) < 0.1 && Mathf.Abs(nextNode.y-rb.position.y) < 0.1 || Time.time > stoppedTimer + 0.5f) {
                nextNode = Pathfind(rb.position, player.transform.position);
                stoppedTimer = Time.time;
            }
            rb.velocity = new Vector2(Mathf.Cos(Mathf.Atan2(nextNode.y - rb.position.y, nextNode.x - rb.position.x)), Mathf.Sin(Mathf.Atan2(nextNode.y - rb.position.y, nextNode.x - rb.position.x))) * speed;
            /*if (Time.time > attackStamp + 3f) {
                Attack();
            }*/
        }
        if (stop && Time.time > stopTimer) {
            stop = false;
            if (attackStunned) {
                if (hitByType == emotionType) {
                    health -= 1;
                } else {
                    GameObject.FindWithTag("Player").GetComponent<DungeonPlayerRV>().Knockback(-savePushbackDir, 20);
                }
            }
        }
        if (health == 0) {
            Destroy(gameObject);
        }
        prevPos = rb.position;
    }

    void Attack() {
        float initialDir = 0;
        if (emotionType == 1) {
            initialDir = Mathf.PI/4;
        }
        if (emotionType == 2) {
            for (int i = 0; i < 8; i++) {
            GameObject atk = Instantiate(Resources.Load<GameObject>("Prefabs/EnemyAttack") as GameObject);
            atk.transform.position = transform.position;
            atk.GetComponent<GoblinCryRV>().dir = initialDir;
            initialDir += Mathf.PI/4;
            }
        } else {
            for (int i = 0; i < 4; i++) {
                GameObject atk = Instantiate(Resources.Load<GameObject>("Prefabs/EnemyAttack") as GameObject);
                atk.transform.position = transform.position;
                atk.GetComponent<GoblinCryRV>().dir = initialDir;
                initialDir += Mathf.PI/2;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("HugAttack")) {
            stop = true;
            stopTimer = Time.time + 0.5f;
            hitByType = 0;
            attackStunned = true;
            rb.velocity = new Vector2(0f,0f);
            savePushbackDir = other.gameObject.GetComponent<HugAttackRV>().dir;
        }
    }

    Vector2 Pathfind(Vector2 pos1, Vector2 pos2) {
        pos1 = new Vector2((pos1.x * 2) % roomWidth, (pos1.y * 2) % roomHeight);
        pos2 = new Vector2((pos2.x * 2) % roomWidth, (pos2.y * 2) % roomHeight);
        List<List<Node>> allNodes = new List<List<Node>> ();
        for (int x = 0; x < roomWidth; x++) {
            allNodes.Add(new List<Node>());
            for (int y = 0; y < roomHeight; y++) {
                Node node = new Node(x,y);
                node.g = int.MaxValue;
                node.CalcF();
                node.prevNode = null;
                if (tm.GetTile<Tile>(new Vector3Int(x, y, 0)) == wallTile) {
                    node.isWalkable = false;
                }
                allNodes[x].Add(node);
            }
        }
        Node initialNode = allNodes[(int)Mathf.Floor(pos1.x + 18)][(int)Mathf.Floor(pos1.y + 10)];
        Node finalNode = allNodes[(int)Mathf.Floor(pos2.x + 18)][(int)Mathf.Floor(pos2.y + 10)];
        List<Node> openList = new List<Node> {initialNode};
        List<Node> closedList = new List<Node>();
        initialNode.g = 0;
        initialNode.h = CalcDist(initialNode, finalNode);
        initialNode.CalcF();
        if (initialNode != finalNode) {
            while (openList.Count > 0) {
                Node currentNode = GetLowestNode(openList);
                if (currentNode == finalNode) {
                    List<Node> path = CalculatePath(finalNode);
                    return new Vector2((path[1].x-18)/2, (path[1].y-10)/2);
                    /*
                    if (Mathf.Abs(pos1.x - path[0].x) < 0.1 && Mathf.Abs(pos1.y - path[0].y) < 0.1) {
                        print("noice");
                        return new Vector2(Mathf.Cos(Mathf.Atan2(path[1].x - pos1.x, path[1].y - pos1.y)), Mathf.Sin(Mathf.Atan2(path[1].x - pos1.x, path[1].y - pos1.y)));
                    } else {
                        print(path[0].x);
                        print(path[0].y);
                        return new Vector2(Mathf.Cos(Mathf.Atan2(path[0].x - pos1.x, path[0].y - pos1.y)), Mathf.Sin(Mathf.Atan2(path[0].x - pos1.x, path[0].y - pos1.y)));
                    }
                    */
                }
                openList.Remove(currentNode);
                closedList.Add(currentNode);
                foreach(Node neighborNode in GetNeighborList(currentNode, allNodes)) {
                    if (closedList.Contains(neighborNode)) {
                        continue;
                    }
                    if (!neighborNode.isWalkable) {
                        closedList.Add(neighborNode);
                        continue;
                    }
                    int tentativeG = currentNode.g + CalcDist(currentNode, neighborNode);
                    if (tentativeG < neighborNode.g) {
                        neighborNode.prevNode = currentNode;
                        neighborNode.g = tentativeG;
                        neighborNode.h = CalcDist(neighborNode, finalNode);
                        neighborNode.CalcF();
                        if (!openList.Contains(neighborNode)) {
                            openList.Add(neighborNode);
                        }
                    }
                }
            }
        }
        return new Vector2(0f,0f);
    }
    private List<Node> GetNeighborList(Node currentNode, List<List<Node>> allNodes) {
        List<Node> neighborList = new List<Node>();
        if (currentNode.x - 1 >= 0) {
            neighborList.Add(GetNode(currentNode.x - 1, currentNode.y, allNodes));
            if (currentNode.y - 1 >= 0) {
                neighborList.Add(GetNode(currentNode.x - 1, currentNode.y - 1, allNodes));
            }
            if (currentNode.y + 1 >= roomHeight) {
                neighborList.Add(GetNode(currentNode.x - 1, currentNode.y + 1, allNodes));
            }
        }
        if (currentNode.x + 1 <= roomWidth) {
            neighborList.Add(GetNode(currentNode.x + 1, currentNode.y, allNodes));
            if (currentNode.y - 1 >= 0) {
                neighborList.Add(GetNode(currentNode.x + 1, currentNode.y - 1, allNodes));
            }
            if (currentNode.y + 1 >= roomHeight) {
                neighborList.Add(GetNode(currentNode.x + 1, currentNode.y + 1, allNodes));
            }
        }
        if (currentNode.y - 1 >= 0) {
            neighborList.Add(GetNode(currentNode.x, currentNode.y - 1, allNodes));
        }
        if (currentNode.y + 1 <= roomHeight) {
            neighborList.Add(GetNode(currentNode.x, currentNode.y + 1, allNodes));
        }
        return neighborList;
    }

    private Node GetNode(int x, int y, List<List<Node>> allNodes) {
        return allNodes[x][y];
    }

    private List<Node> CalculatePath(Node finalNode) {
        List<Node> path = new List<Node>();
        path.Add(finalNode);
        Node currentNode = finalNode;
        while (currentNode.prevNode != null) {
            path.Add(currentNode.prevNode);
            currentNode = currentNode.prevNode;
        }
        path.Reverse();
        return path;
    }
    private int CalcDist(Node a, Node b) {
        int xdist = Mathf.Abs(a.x - b.x);
        int ydist = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xdist - ydist);
        return 14 * Mathf.Min(xdist, ydist) + 10*remaining;
    }
    private Node GetLowestNode(List<Node> nodeList) {
        Node lowest = nodeList[0];
        for (int i = 1; i < nodeList.Count; i++) {
            if (nodeList[i].f < lowest.f) {
                lowest = nodeList[i];
            }
        }
        return(lowest);
    }
}

public class Node {
    public int x;
    public int y;
    public int g;
    public int h;
    public int f;
    public Node prevNode;
    public bool isWalkable;
    public Node(int x, int y) {
        this.x = x;
        this.y = y;
        isWalkable = true;
    }
    public void CalcF() {
        f = g + h;
    }
}
