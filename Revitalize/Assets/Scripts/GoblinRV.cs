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
    
    void Start()
    {
        tm = GameObject.FindWithTag("Walls").GetComponent<Tilemap>();
        roomWidth = 36;
        roomHeight = 20;
        speed = 0.4f;
        rb = GetComponent<Rigidbody2D>();
        wallTile = Resources.Load<Tile>("blacksquare");
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = Pathfind(rb.position, player.transform.position) * speed;
    }

    Vector2 Pathfind(Vector2 pos1, Vector2 pos2) {
        pos1 = new Vector2(pos1.x % roomWidth, pos1.y % roomHeight);
        pos2 = new Vector2(pos2.x % roomWidth, pos2.y % roomHeight);
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
        while (openList.Count > 0) {
            Node currentNode = GetLowestNode(openList);
            if (currentNode == finalNode) {
                List<Node> path =  CalculatePath(finalNode);
                if (Mathf.Abs(rb.position.x - path[0].x) < 0.1 && Mathf.Abs(rb.position.y - path[0].y) < 0.1) {
                    return new Vector2(Mathf.Cos(Mathf.Atan2(path[1].x - rb.position.x, path[1].y - rb.position.y)), Mathf.Sin(Mathf.Atan2(path[1].x - rb.position.x, path[1].y - rb.position.y)));
                } else {
                    return new Vector2(Mathf.Cos(Mathf.Atan2(path[0].x - rb.position.x, path[0].y - rb.position.y)), Mathf.Sin(Mathf.Atan2(path[0].x - rb.position.x, path[0].y - rb.position.y)));
                }
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
