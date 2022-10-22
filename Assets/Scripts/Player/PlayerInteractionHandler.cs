using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionHandler : MonoBehaviour
{
    [SerializeField] Vector3 size;

    private InputActions _input;
    Transform itemSlot;
    Transform battleField;
    GameObject heldObject = null;

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

    List<GameObject> objectsInLootArea = new List<GameObject>();
    void Awake()
    {
        _input = new InputActions();
        itemSlot = transform.parent.GetChild(2);
        battleField = GameObject.Find("Battlefield").transform;
    }
    void Start()
    {

    }
    void Update()
    {
    }

    private void OnPickupDropOLD(InputValue value)
    {
        Debug.Log("pickup/drop");
        if (itemSlot.childCount != 0)
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

                        item.transform.parent = itemSlot;
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
                if (itemSlot.childCount == 1)
                {
                    GameObject item = itemSlot.GetChild(0).gameObject;

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
    void OnPickupDrop()
    {
        //Guard clause
        if (!_canPickup)
        {
            return;
        }

        //Pickup
        if (itemSlot.childCount == 0 && objectsInLootArea.Count != 0)
        {
            //Find closest object in loot area
            float objectDistance;
            float closestDistance = (objectsInLootArea[0].transform.position - transform.parent.position).magnitude;
            int closestIndex = 0;
            for (int i = 1; i < objectsInLootArea.Count; i++)
            {
                objectDistance = (objectsInLootArea[i].transform.position - transform.parent.position).magnitude;
                if (objectDistance < closestDistance)
                {
                    closestDistance = objectDistance;
                    closestIndex = i;
                }
            }

            //Pick up object
            heldObject = objectsInLootArea[closestIndex];
            var heldObjectTransform = heldObject.transform;
            heldObjectTransform.parent = itemSlot;
            heldObjectTransform.localPosition = Vector3.zero;
            heldObjectTransform.localRotation = Quaternion.identity;
            Destroy(heldObject.GetComponent<Rigidbody>());
        }

        //Drop
        else if(itemSlot.childCount != 0)
        {
            heldObject.transform.parent = battleField;
            heldObject.AddComponent<Rigidbody>();
        }
    }
    private void OnThrow(InputValue value)
    {
        //Throw
        if (!_canThrow || itemSlot.childCount == 0 || heldObject == null)
        {
            return;
        }
        heldObject.transform.parent = battleField;
        heldObject.AddComponent<Rigidbody>();
        var throwScript = heldObject.GetComponent<Throw2>();
        throwScript.thrower = transform.parent;
        throwScript.ThrowItem();
    }

    private void OnPause(InputValue value)
    {
        //Pause
        if (_canPause)
        {

        }
        //foreach (GameObject item in )
    }


    private void OnEnable()
    {
        _input.Ingame.Enable();
    }
    private void OnDisable()
    {
        _input.Ingame.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        objectsInLootArea.Add(other.gameObject);
    }
    private void OnTriggerExit(Collider other)
    {
        objectsInLootArea.Remove(other.gameObject);
    }
}
