using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 *  Created (DD/MM/YY - 06.10.2022) 
 *  Edited (DD/MM/YY - 07.10.2022)
 *  @Janek Tuisk
 */

/** TODO
 * Need to recalculate raycast position ( height )
 * Make pickedup object "follow" player
 * Throw object math
 * Get values from throwable object
 */

public class PlayerInteractionHandler : MonoBehaviour
{

    [SerializeField] private LayerMask pickableLayer;
    [SerializeField] private float pickupRange = 2f;
    [SerializeField] private float throwForce = 500f;
    [SerializeField] private float throwUpwardForce = 300f;
    private PlayerControls playerControls;

    private RaycastHit hit;

    [SerializeField] private GameObject currentObject;
    [SerializeField] private GameObject objectParent; // Pickable object parent

    // Start is called before the first frame update
    void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnPickupDrop(InputValue movementValue)
    {
        if (currentObject != null)
        {
            Debug.Log("Drop");
            DropObject();
        }
        else
        {
            Debug.Log("Pickup");
            if (Physics.Raycast(transform.position, transform.forward, out hit, pickupRange, pickableLayer))
            {
                PickupObject(hit.collider.gameObject);
            }
        }
    }

    private void OnThrow(InputValue value)
    {
        Debug.Log("Throw");
        if (currentObject == null)
            return;

        Vector3 force = transform.forward * throwForce + transform.up * throwUpwardForce;
        currentObject.GetComponent<Rigidbody>().AddForce(force);
        DropObject();
    }

    private void OnPause(InputValue value)
    {
        Debug.Log("Pause");
    }

    /** (https://learn.unity.com/tutorial/taking-advantage-of-the-input-system-scripting-api?uv=2020.1&projectId=5fc93d81edbc2a137af402b7#5fcad3efedbc2a0020f781e1)
     * For the Input System commands to work, you will need to add both the OnEnable() and OnDisable() methods next.
     * Because the Input System is modular, you will be able to swap between different input assets or Control Schemes in the future.
     * */
    private void OnEnable()
    {
        playerControls.Player.Enable();
    }
    private void OnDisable()
    {
        playerControls.Player.Disable();
    }


    private void PickupObject(GameObject go)
    {
        if (objectParent == null)
            return;

        currentObject = go;
        currentObject.transform.position = Vector3.zero;
        currentObject.transform.rotation = Quaternion.identity; // do we need this?
        currentObject.transform.SetParent(objectParent.transform, false);
    }

    private void DropObject()
    {
        if (objectParent == null)
            return;

        currentObject.transform.parent = null;
        currentObject = null;
    }
}
