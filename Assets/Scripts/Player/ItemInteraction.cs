using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemInteraction : MonoBehaviour
{
    Transform itemSlot;
    Transform battleField;
    GameObject heldObject = null;
    List<GameObject> objectsInLootArea = new List<GameObject>();
    void Awake()
    {
        itemSlot = transform.parent.GetChild(2);
        battleField = GameObject.Find("Battlefield").transform;
    }
    void OnPickupDrop()
    {
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
        else if (itemSlot.childCount != 0)
        {
            heldObject.transform.parent = battleField;
            heldObject.AddComponent<Rigidbody>();
        }
    }
    void OnThrow()
    {
        //Guard Clause
        if (itemSlot.childCount == 0 || heldObject == null)
        {
            return;
        }

        //Throw
        heldObject.transform.parent = battleField;
        heldObject.AddComponent<Rigidbody>();
        var throwScript = heldObject.GetComponent<Throw2>();
        throwScript.thrower = transform.parent;
        throwScript.ThrowItem();
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
