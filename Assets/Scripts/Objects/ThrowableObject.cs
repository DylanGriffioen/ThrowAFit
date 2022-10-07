using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableObject : MonoBehaviour
{
    [SerializeField] private float throwForce = 300f;
    [SerializeField] private float throwUpwardForce = 300f;
    [SerializeField] private float forceMultiplier = 5f;
    [SerializeField] private Collider objectCollider;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    /*public bool Pickup(Transform playerTransform)
    {
        return true;
    }

    public bool Throw(Transform playerTransform)
    {
        if (transform.parent == null)
            return false;

        rb.AddForce(getThrowForce(playerTransform));

        if (!Drop())
            return false;

        return true;
    }

    public bool Drop()
    {
        if (transform.parent != null)
            return false;

        transform.parent = null;
        return true;
    } */

    public bool Pickup(GameObject parent)
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity; // do we need this?
        transform.SetParent(parent.transform, false);
        objectCollider.enabled = false;
        rb.useGravity = false;
        return true;
    }
    public bool Drop()
    {
        transform.SetParent(null);
        objectCollider.enabled = true;
        rb.useGravity = true;
        return true;
    }

    public bool Throw(Transform thrower)
    {
        if (!Drop())
            return false;

        rb.AddForce(getThrowForce(thrower));

        return true;
    }

    private Vector3 getThrowForce(Transform playerTransform)
    {
        return playerTransform.forward * throwForce + playerTransform.up * throwUpwardForce;
    }

    private void Hit()
    {

    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag.Equals("Player"))
        {
            GameObject player = col.gameObject;
            float force = col.relativeVelocity.magnitude;

            player.transform.GetComponent<Rigidbody>().AddForce(transform.forward * force * forceMultiplier);
          
        }
    }
}
