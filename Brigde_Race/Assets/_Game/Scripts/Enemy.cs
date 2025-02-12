using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Color currentColor;
    [SerializeField] private Transform brickHolder;
    private List<Brick> OwnerBricks = new List<Brick>();

    public float moveSpeed = 5f;
    private int countBrick = 0;
    private float yPos = 0.5f;
    private bool isOnBridge = false; // Biến kiểm tra Enemy đang trên cầu

    // Start is called before the first frame update
    void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        GameObject nearestBrick = FindNearestBrick();

        if (nearestBrick != null)
        {
            agent.SetDestination(nearestBrick.transform.position);
        }
        brickHolder = this.transform;
        rb.GetComponent<Renderer>().material.color = currentColor;
    }

    // Update is called once per frame
    void Update()
    {
        bool ray = CanMoveUp();

        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (isOnBridge)
            {
                // Nếu đang trên cầu và còn gạch thì tiếp tục đi lên cầu
                if (HasBrickOnHead())
                {
                    GameObject nearestBridge = FindNearestBridge();
                    if (nearestBridge != null)
                    {
                        agent.SetDestination(nearestBridge.transform.position);
                    }
                }
            }
            else
            {
                // Nếu không trên cầu thì tìm gạch hoặc đi lên cầu nếu đủ gạch
                if (countBrick >= 10)
                {
                    GameObject nearestBridgeLast = FindNearestBridgeLast();
                    if (nearestBridgeLast != null)
                    {
                        agent.SetDestination(nearestBridgeLast.transform.position);
                    }
                }
                else
                {
                    GameObject nearestBrick = FindNearestBrick();
                    if (nearestBrick != null)
                    {
                        agent.SetDestination(nearestBrick.transform.position);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Brick") && other.GetComponent<Renderer>().material.color == currentColor)
        {
            AddBack(other.gameObject);

            if (countBrick < 10)
            {
                GameObject nearestBrick = FindNearestBrick();
                if (nearestBrick != null)
                {
                    GetComponent<NavMeshAgent>().SetDestination(nearestBrick.transform.position);
                }
            }
            else
            {
                GameObject nearestBridgeLast = FindNearestBridgeLast();
                if (nearestBridgeLast != null)
                {
                    GetComponent<NavMeshAgent>().SetDestination(nearestBridgeLast.transform.position);
                }
            }
        }

        if (other.gameObject.CompareTag("Bridge_number_last"))
        {
            isOnBridge = true; // Đánh dấu Enemy đã vào cầu

            if (HasBrickOnHead())
            {
                GameObject nearestBridge = FindNearestBridge();
                if (nearestBridge != null)
                {
                    GetComponent<NavMeshAgent>().SetDestination(nearestBridge.transform.position);
                }
            }
        }

        if (other.gameObject.CompareTag("Bridge_number"))
        {
            Bridge(other.gameObject);

            if (!HasBrickOnHead()) // Nếu hết gạch thì quay lại tìm gạch
            {
                isOnBridge = false;
                GameObject nearestBrick = FindNearestBrick();
                if (nearestBrick != null)
                {
                    GetComponent<NavMeshAgent>().SetDestination(nearestBrick.transform.position);
                }
            }
        }
    }
    private GameObject FindNearestBridgeLast()
    {
        GameObject[] bridgesLast = GameObject.FindGameObjectsWithTag("Bridge_number_last");
        GameObject nearestBridgeLast = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject bridge in bridgesLast)
        {
            float distance = Vector3.Distance(transform.position, bridge.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestBridgeLast = bridge;
            }
        }
        return nearestBridgeLast;
    }

    private void Bridge(GameObject bridgeObj)
    {
        if (brickHolder.childCount > 0)
        {
            MeshRenderer mesh = bridgeObj.GetComponent<MeshRenderer>();
            mesh.enabled = true;
            mesh.material.color = currentColor;
            int lastBrickIndex = brickHolder.childCount - 1;

            if (lastBrickIndex >= 0)
            {
                Destroy(brickHolder.GetChild(lastBrickIndex).gameObject);
                countBrick--;
                yPos -= 0.2f;
                bridgeObj.GetComponent<BoxCollider>().enabled = false;
            }
        }

        // Chỉ quay lại tìm gạch khi đã hết số gạch trên người
        if (!HasBrickOnHead())
        {
            GameObject nearestBrick = FindNearestBrick();
            if (nearestBrick != null)
            {
                GetComponent<NavMeshAgent>().SetDestination(nearestBrick.transform.position);
            }
            else
            {
                moveSpeed = 0;
            }
        }
    }


    private GameObject FindNearestBrick()
    {
        GameObject[] bricks = GameObject.FindGameObjectsWithTag("Brick");
        GameObject nearestBrick = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject brick in bricks)
        {
            if (brick.GetComponent<Renderer>().material.color == currentColor && brick.transform.parent == null)
            {
                float distance = Vector3.Distance(transform.position, brick.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestBrick = brick;
                }
            }
        }
        return nearestBrick;
    }

    private GameObject FindNearestBridge()
    {
        GameObject[] bridges = GameObject.FindGameObjectsWithTag("Bridge_number");
        GameObject nearestBridge = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject bridge in bridges)
        {
            float distance = Vector3.Distance(transform.position, bridge.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestBridge = bridge;
            }
        }
        return nearestBridge;
    }

    private bool CanMoveUp()
    {
        RaycastHit hit;
        float rayDistance = 2f;
        Vector3 rayOrigin = transform.position + new Vector3(0, 1.2f, 1.5f);

        Debug.DrawRay(rayOrigin, Vector3.down * rayDistance, Color.black);

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Bridge_number") || hit.collider.CompareTag("Bridge_number_last"))
            {
                if (HasBrickOnHead())
                {
                    hit.collider.isTrigger = true;
                    return true;
                }
                else
                {
                    hit.collider.isTrigger = false;
                    return false;
                }
            }
            else if (hit.collider.CompareTag("Brick") || hit.collider.CompareTag("Wall"))
            {
                return true;
            }
        }
        return false;
    }

    private bool HasBrickOnHead()
    {
        return brickHolder.childCount > 0;
    }

    private void AddBack(GameObject obj)
    {
        obj.transform.SetParent(brickHolder);
        obj.transform.rotation = brickHolder.rotation;
        obj.transform.position = brickHolder.position + transform.up * (countBrick * 0.5f) - transform.forward * 1f;
        countBrick++;

        if (countBrick < 10)
        {
            GameObject nearestBrick = FindNearestBrick();
            if (nearestBrick != null)
            {
                GetComponent<NavMeshAgent>().SetDestination(nearestBrick.transform.position);
            }
        }
        else
        {
            GameObject nearestBridge = FindNearestBridge();
            if (nearestBridge != null)
            {
                GetComponent<NavMeshAgent>().SetDestination(nearestBridge.transform.position);
            }
        }
    }

}
