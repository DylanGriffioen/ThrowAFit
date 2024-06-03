using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DisableOnPlay : MonoBehaviour
{
    private PlayerInput Input;

    void Start()
    {
        Input = GetComponent<PlayerInput>();
        Input.actions = null;
        gameObject.SetActive(false);
    }
}
