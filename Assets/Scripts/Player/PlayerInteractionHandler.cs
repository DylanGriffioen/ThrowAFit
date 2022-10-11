
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionHandler : MonoBehaviour
{
    [SerializeField] Vector3 size = Vector3.one;

    private InputActions _input;

    [Header("Item slot")]
    [SerializeField] Transform _itemSlot;

    [Header("Pickup")]
    [SerializeField] bool _canPickup = true;
    [SerializeField] float _maxPickupRange = 1.0f;
    [SerializeField] LayerMask throwableItemMask;

    [Header("Drop")]
    [SerializeField] bool _canDrop = true;
    [SerializeField] Vector3 _dropLocation;

    [Header("Throw")]
    [SerializeField] bool _canThrow = true;

    [Header("Pause")]
    [SerializeField] bool _canPause = true;

    void Awake()
    {
        _input = new InputActions();

    }
    private void Update()
    {
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 2.0f, Color.red);
    }

    private void OnPickupDrop(InputValue movementValue)
    {
        Debug.Log("pickup/drop");
        if (!PlayerHasItem())
        {
            //Pickup
            if (_canPickup)
            {
                
                RaycastHit pickupRay;
                float rayCalculatedHeight = 1f;
                Vector3 boxSize = new Vector3(2.0f, 2.0f, 2.0f);
                //need to get new values for this, as doesn't regonise every item.
                if(Physics.BoxCast(transform.position, transform.localScale, transform.forward, out pickupRay, transform.rotation, _maxPickupRange, throwableItemMask))
                {
                    Debug.Log($"distance: {pickupRay.distance}");
                    if (pickupRay.collider.attachedRigidbody)
                    {
                        GameObject item = pickupRay.collider.gameObject;

                        if (item.transform.parent != null) //Item is picked up already
                            return;

                        item.transform.parent = _itemSlot;
                        item.GetComponent<Collider>().enabled = false;
                        item.GetComponent<Rigidbody>().useGravity = false;
                        item.transform.localPosition = Vector3.zero;
                        item.transform.rotation = transform.rotation;
                    }
                }
            }
        }
        else
        {            if (_canDrop)
            {
                //Drop
                GameObject item = _itemSlot.GetChild(0).gameObject;

                if (item != null)
                {
                    item.transform.parent = null;
                    item.GetComponent<Collider>().enabled = true;
                    item.GetComponent<Rigidbody>().useGravity = true;
                }
            }
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(transform.position, transform.forward * _maxPickupRange);
        //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(transform.position + transform.forward * _maxPickupRange, size);
    }
    private bool PlayerHasItem()
    {
        return (_itemSlot.transform.childCount != 0);
    }

    private void OnThrow(InputValue value)
    {
        //Throw
        if (_canThrow)
        {

        }
    }

    private void OnPause(InputValue value)
    {
        //Pause
        if (_canPause)
        {

        }
    }


    private void OnEnable()
    {
        _input.Ingame.Enable();
    }
    private void OnDisable()
    {
        _input.Ingame.Disable();
    }
}
