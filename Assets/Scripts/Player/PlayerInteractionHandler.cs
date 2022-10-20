
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionHandler : MonoBehaviour
{
    [SerializeField] Vector3 size;

    private InputActions _input;

    [Header("Item slot")]
    [SerializeField] Transform _itemSlot;

    [Header("Pickup")]
    [SerializeField] bool _canPickup = true;
    [SerializeField] LayerMask _throwableItemMask;
    [SerializeField] Transform _lootArea;
    [SerializeField] BoxCollider _lootAreaCollider;
    [SerializeField] float _maxPickupDistance = 1.0f;

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
        //ExtDebug.DrawBox(_lootArea.position, _lootArea.localScale, transform.rotation, Color.red);
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


                // TODO: FIX THIS

                //if (Physics.BoxCast(_lootArea.position, _lootArea.localScale, transform.forward, out pickupRay, transform.rotation, _maxPickupDistance, _throwableItemMask, QueryTriggerInteraction.Collide))
                if (Physics.BoxCast(_lootAreaCollider.bounds.center, _lootArea.localScale, transform.forward, out pickupRay, transform.rotation, _throwableItemMask))
                {
                    if (pickupRay.collider.attachedRigidbody)
                    {
                        Debug.Log("2 - Throwable item found, name: " + pickupRay.collider.name);
                        GameObject item = pickupRay.collider.gameObject;

                        if (item.transform.parent != null) //Item is picked up already
                            return;

                        /* TODO
                         * The parent's scale influences the children as well. When it is parented, divide the player's scale by the platform's scale.
                         * For example, if player is scaled (1,1,1) and the platform is (0.7, 0.2, 0.7) the player will need to be scaled to (1.4286, 5, 1.4286)
                         * to keep it's correct size when parented.
                         * Of course when it is unparented, revert the scale back.
                         */

                        item.transform.parent = _itemSlot;
                        item.GetComponent<Collider>().enabled = false;
                        item.GetComponent<Rigidbody>().useGravity = false;
                        item.transform.localPosition = Vector3.zero;
                        item.transform.rotation = Quaternion.identity;
                        item.transform.rotation = transform.rotation;
                    }
                }
                Debug.Log(pickupRay);
            }
        }
        else
        {
            if (_canDrop)
            {
                //Drop
                if (_itemSlot.childCount == 1)
                {
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
            if(_itemSlot.childCount == 1)
            {
                GameObject item = _itemSlot.GetChild(0).gameObject;

                if (item != null)
                {
                    ThrowableItem throwableItem = item.GetComponent<ThrowableItem>();

                    if(throwableItem != null)
                    {
                        throwableItem.ThrowItem();
                    }
                }
            }
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
