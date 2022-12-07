using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemInteraction : MonoBehaviour
{
    public float throwEndLag;
    Movement movementScript;
    Transform itemSlot;
    GameObject heldObject = null;
    List<GameObject> objectsInLootArea = new List<GameObject>();
    float mass, drag, angularDrag;
    Animator animator;
    bool throwNextFrame;
    bool pickupDropEnabled = true;
    bool _canSteal = true;
    void Awake()
    {
        itemSlot = transform.parent.GetChild(2);
        movementScript = transform.parent.GetComponent<Movement>();
        animator = transform.parent.GetComponentInChildren<Animator>();
    }
    public void OnPickupDrop(InputAction.CallbackContext ctx)
    {
        if (!pickupDropEnabled || !ctx.performed) { return; }

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
            objectsInLootArea.RemoveAt(closestIndex);
            var heldObjectTransform = heldObject.transform;

            GameObject parentObject = heldObjectTransform.parent == null ? null : heldObjectTransform.parent.gameObject;
            if(parentObject == null || !parentObject.tag.Equals("PlayerItem"))
            {
                //From ground
                heldObjectTransform.parent = itemSlot;
                heldObjectTransform.localPosition = new Vector3(0f, heldObjectTransform.localScale.y / 2f, 0f);
                heldObjectTransform.localRotation = Quaternion.identity;

                //Store values and destroy the Rigidbody component
                var rb = heldObject.GetComponent<Rigidbody>();
                mass = rb.mass;
                drag = rb.drag;
                angularDrag = rb.angularDrag;
                Destroy(rb);
                movementScript.holdingItem = true;
                movementScript.heldItemMass = mass;

                //Switch carrying bool in animator
                animator.SetBool("Carrying", true);
            }
            else
            {
                if (!_canSteal)
                    return;
                //Steal from player
                GameObject stolenFromPlayer = parentObject.transform.parent.gameObject;
                if(stolenFromPlayer != null)
                {
                    heldObjectTransform.parent = itemSlot;
                    heldObjectTransform.localPosition = new Vector3(0f, heldObjectTransform.localScale.y / 2f, 0f);
                    heldObjectTransform.localRotation = Quaternion.identity;
                    stolenFromPlayer.GetComponentInChildren<ItemInteraction>().LoseItem();
                    movementScript.holdingItem = true;
                    movementScript.heldItemMass = mass;

                    //Switch carrying bool in animator
                    animator.SetBool("Carrying", true);
                }
            }

            /*heldObjectTransform.parent = itemSlot;
            heldObjectTransform.localPosition = new Vector3(0f,heldObjectTransform.localScale.y/2f,0f);
            heldObjectTransform.localRotation = Quaternion.identity;

            //Store values and destroy the Rigidbody component
            var rb = heldObject.GetComponent<Rigidbody>();
            mass = rb.mass;
            drag = rb.drag;
            angularDrag = rb.angularDrag;
            Destroy(rb);

            //Pass values to movement script
            movementScript.holdingItem = true;
            movementScript.heldItemMass = mass;

            //Switch carrying bool in animator
            animator.SetBool("Carrying", true);*/
        }

        //Drop
        else if (itemSlot.childCount != 0)
        {
            movementScript.holdingItem = false;
            heldObject.transform.parent = null; //TODO: If have time: Parent == ItemSpawner.item game object

            //Add Rigidbody and assign stored values
            var rb = heldObject.AddComponent<Rigidbody>();
            rb.mass = mass;
            rb.drag = drag;
            rb.angularDrag = angularDrag;

            //Switch carrying bool in animator
            animator.SetBool("Carrying", false);
        }
    }
    public void LoseItem()
    {
        if (itemSlot.childCount == 0)
        {
            movementScript.holdingItem = false;
            Destroy(heldObject);

            //Switch carrying bool in animator
            animator.SetBool("Carrying", false);
        }
    }

    public void DropDestroyItem()
    {
        if(itemSlot.childCount != 0)
        {
            movementScript.holdingItem = false;
            Destroy(heldObject);

            //Switch carrying bool in animator
            animator.SetBool("Carrying", false);
        }
    }

    public void OnThrow(InputAction.CallbackContext ctx)
    {
        if (itemSlot.childCount == 0 || heldObject == null || !ctx.performed) { return; }
        movementScript.Throw(throwEndLag);
        Invoke("EndThrow", throwEndLag);
        throwNextFrame = true;
        pickupDropEnabled = false;
    }
    void Update()
    {
        if (throwNextFrame)
        {
            throwNextFrame = false;
            ThrowObject();
        }
    }
    void ThrowObject()
    {
        //Throw
        movementScript.holdingItem = false;
        heldObject.transform.parent = null;

        //Add Rigidbody and assign stored values
        var rb = heldObject.AddComponent<Rigidbody>();
        rb.mass = mass;
        rb.drag = drag;
        rb.angularDrag = angularDrag;

        //Call throw function from throw script
        var throwScript = heldObject.GetComponent<ThrowableItem>();
        throwScript.thrower = transform.parent;
        throwScript.ThrowItem();

        //Switch carrying bool in animator
        animator.SetBool("Carrying", false);
        animator.SetBool("Throwing", true);
        animator.SetTrigger("Throw");
    }
    void EndThrow()
    {
        pickupDropEnabled = true;
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
