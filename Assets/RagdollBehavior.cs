using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollBehavior : MonoBehaviour
{
    [System.NonSerialized] public Vector3 velocity;
    [System.NonSerialized] public Movement movementScript;
    [System.NonSerialized] public Color color;
    Rigidbody[] rigidbodies;
    Rigidbody rootRB;
    SkinnedMeshRenderer render;
    Transform root;

    const float noise = 2f;
    void Awake()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        render = transform.parent.GetChild(1).GetComponent<SkinnedMeshRenderer>();
        root = transform.GetChild(0);
        rootRB = root.GetComponent<Rigidbody>();
    }
    void Start()
    {
        render.material.color = color;
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.velocity = velocity + Random.insideUnitSphere*noise;
        }
    }

    void Update()
    {
        if (root.position.y < 2.5f || rootRB.velocity.magnitude < 0.2f)
        {
            movementScript.TurnOffRagdoll();
        }
    }
}
