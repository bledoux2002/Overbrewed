using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 500f;
    
    [SerializeField]
    private LayerMask _snapLayer;

    Vector3 movement;
    Vector2 direction;
    GameObject _item;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        direction = movement;
        float inputMagnitude = Mathf.Clamp01(direction.magnitude);
        direction.Normalize();

        if (Input.GetButtonDown("Select"))
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * 1f, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.up), 1f, _snapLayer);
            if (hit)
            {
                GameObject snapItem = hit.collider.gameObject.GetComponent<SnapPoint>().item;
                if (_item == null)
                {
                    if (snapItem != null)
                    {
                        _item = snapItem;
                        hit.collider.gameObject.GetComponent<SnapPoint>().item = null;
                        snapItem = null;
                        //Debug.Log("Grabbed " + _item.name);
                    }
                    else
                    {
                        //Debug.Log("Snap Point empty");
                    }
                }
                else
                {
                    if (snapItem == null)
                    {
                        hit.collider.gameObject.GetComponent<SnapPoint>().item = _item;
                        snapItem = _item;
                        _item.transform.position = hit.transform.position;
                        if ((hit.collider.gameObject.CompareTag("Guest")) && (_item.CompareTag("Mug")))
                        {
                            hit.collider.gameObject.GetComponent<Guest>().checkOrder(snapItem.GetComponent<Mug>().components);
                            hit.collider.gameObject.GetComponent<SnapPoint>().item.GetComponent<Mug>().emptyCup();
                        }
                        //Debug.Log("Placed " + _item.name);
                        _item = null;
                    }
                    else
                    {
                        //Debug.Log("Something is already placed there.");
                        if (snapItem.CompareTag("Mug"))
                        {
                            snapItem.GetComponent<Mug>().addIngredient(_item);
                        }
                    }
                }
            }
            else
            {
                //Debug.Log("No snap point in range");
            }
        }
    }

    void FixedUpdate()
    {
        //Movement
        transform.position = transform.position + movement * moveSpeed * Time.fixedDeltaTime;
        if (_item != null)
        {
            _item.transform.position = transform.position;
        }
        if (direction != Vector2.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotateSpeed * Time.deltaTime);
        }
    }
}
