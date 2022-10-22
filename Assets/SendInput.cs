using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SendInput : MonoBehaviour
{
    PlayerInteractionHandler itemInteraction;
    void Awake()
    {
        itemInteraction = transform.GetChild(2).GetComponent<PlayerInteractionHandler>();
    }

    void OnPickupDrop()
    {
        //itemInteraction.OnPickup
    }
}
