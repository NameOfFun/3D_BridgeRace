using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject player;
    [SerializeField] private Color currentColor;
    [SerializeField] private Transform brickHolder;
    private List<Brick> OwnerBricks = new List<Brick>();

    public float moveSpeed = 5f;
    private float rotationSpeed = 5f;
    private Transform transformBrick;
    private int countBrick = 0;
    private float yPos = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        move();
        rb.GetComponent<Renderer>().material.color = currentColor;
        bool ray = CanMoveUp();
    }

    private void move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(moveX, 0f, moveZ).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            float smoothAngle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, rotationSpeed * Time.deltaTime);

            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.CompareTag("Brick") && other.GetComponent<Renderer>().material.color == currentColor)
        //{
        //    FindObjectOfType<SpawnBricks>().MarkForRespawn(other.transform.position, other.GetComponent<Renderer>().material.color);
        //    AddBack(other.gameObject);
        //    //Destroy(other.gameObject);
        //}

        if (other.gameObject.CompareTag("Brick") && other.GetComponent<Renderer>().material.color == currentColor)
        {
            AddBack(other.gameObject);
        }
        if (other.gameObject.CompareTag("Bridge_number"))
        {
            Bridge(other.gameObject);
            if (!HasBrickOnHead() || !CanMoveUp())
            {
                moveSpeed = 0;
            }
            else
            {
                moveSpeed = 5f;
            }
        }
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

        if (brickHolder.childCount == 0 && !CanMoveUp())
        {
            moveSpeed = 0;
        }
    }

    private bool CanMoveUp()
    {
        RaycastHit hit;
        float rayDistance = 2f;
        Vector3 rayOrigin = transform.position + new Vector3(0, 1.5f, 1f);

        Debug.DrawRay(rayOrigin, Vector3.down * rayDistance, Color.black);

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Bridge_number"))
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
    }


}

