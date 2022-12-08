using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchKick : MonoBehaviour
{
    [SerializeField] Animator playerAnimator;

    [SerializeField] float punchDamage = 15;
    [SerializeField] float punchAnimationTime = 0.5f;
    [SerializeField] float punchForce = 1f;

    [SerializeField] float kickDamage = 30;
    [SerializeField] float kickAnimationTime = 1f;
    [SerializeField] float kickForce = 2f;

    [SerializeField] float hitImpulseAngle = 15f;

    private float _forceMultiplier = 1f;
    private float _damageMultiplier = 1f;

    Movement movementScript;
    void Awake()
    {
        movementScript = GetComponent<Movement>();
    }
    void Start()
    {
        UpdateMultipliers();
    }

    void Update()
    {
        UpdateMultipliers();
    }

    void UpdateMultipliers()
    {
        if (GameManager._instance != null && GameManager.GAME_STATE == GameStatus.PREGAME)
        {
            if (GameManager._instance.ForceMultiplier > 0)
            {
                _forceMultiplier = GameManager._instance.ForceMultiplier;
            }
            if (GameManager._instance.DamageMultiplier > 0)
            {
                _damageMultiplier = GameManager._instance.DamageMultiplier;
            }
        }
    }
}
