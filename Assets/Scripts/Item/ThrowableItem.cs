using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableItem : MonoBehaviour
{

    [SerializeField] int _throwableItemLayerNumber;


    [Header("Gravity")]
    [SerializeField] bool _useUnityGravitySystem = true;
    [SerializeField] float _gravityForce = -9.81f;

    [Header("Throw")]
    [SerializeField] float _throwHeightForce = 1.0f;
    [SerializeField] float _throwHeightForceMultiplier = 1.0f;
    [SerializeField] float _throwForwardForce = 1.0f;
    [SerializeField] float _throwForwardForceMultiplier = 1.0f;
    [SerializeField] bool _isThrown = false;
    [SerializeField] float _maxFlyingTime = 10.0f;
    [SerializeField] float _currentFlyingTime = 0;

    [Header("Impact")]
    [SerializeField] float _playerImpactForce = 5.0f;
    [SerializeField] float _playerImpactForceMultiplier = 1.0f;
    [SerializeField] bool _destroyOnHit = false;
    [SerializeField] bool _destroyOnPlayerHit = false;
    [SerializeField] AudioClip _OnHitAudioClip = null;


    [Header("Rigidbody")]
    [SerializeField] float _itemMass = 1.0f;
    [SerializeField] float _itemDrag = 0.0f;
    [SerializeField] float _itemAngularDrag = 0.0f;
    [SerializeField] Rigidbody _rb;

    [Header("Collider")]
    [SerializeField] Collider _col;


    private void Awake()
    {
        UpdateGameobjectLayer();
        _rb = GetRigidbody();
        _col = GetCollider();
    }

    private void FixedUpdate()
    {
        if (_isThrown)
        {
            if(_currentFlyingTime > 0)
            {
                _currentFlyingTime -= Time.fixedDeltaTime;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void UpdateGameobjectLayer()
    {
        if(gameObject.layer != _throwableItemLayerNumber)
            gameObject.layer = _throwableItemLayerNumber;

    }
    private Rigidbody GetRigidbody()
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = _itemMass;
            rb.drag = _itemDrag;
            rb.angularDrag = _itemAngularDrag;
        }
        rb.useGravity = _useUnityGravitySystem;

        return rb;
    }

    private Collider GetCollider()
    {
        return gameObject.GetComponent<Collider>();
    }

    public void ThrowItem()
    {
        if (!_isThrown)
        {
            transform.parent = null;
            if (_useUnityGravitySystem)
                _rb.useGravity = true;

            _col.enabled = true;
            _col.isTrigger = true;

            _rb.AddForce(CalculateThrowForce(), ForceMode.Force);
            _currentFlyingTime = _maxFlyingTime;
            _isThrown = true;
        }
    }

    private Vector3 CalculateThrowForce()
    {
        Vector3 calculatedForce;

        //TODO: Redo this.
        calculatedForce = transform.forward * _throwForwardForce * _throwForwardForceMultiplier;

        return calculatedForce;        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (_isThrown)
        {
            if (other.tag.Equals("Player"))
            {
                Debug.Log("Player hit! "+ other.name);

                PlayerMovement PM = other.GetComponent<PlayerMovement>();

                PM.ApplyMovementForce(ImpactForceDirection(other.transform), ImpactForce());
                OnHitDefault(_destroyOnPlayerHit);

            }
            else if (other.tag.Equals("Ground"))
            {
                Debug.Log("Ground: " + other.name);
                OnHitDefault(_destroyOnHit);
            }
            else
            {
                Debug.Log("other: " + other.tag);
            }
        }
    }

    private Vector3 ImpactForceDirection(Transform other)
    {
        Vector3 calculatedDirection;

        //TODO: Redo this.
        calculatedDirection = transform.forward * _playerImpactForce * _playerImpactForceMultiplier + Vector3.up * _playerImpactForce * _playerImpactForceMultiplier; // * _throwForwardForce * _throwForwardForceMultiplier
        Debug.Log(calculatedDirection);

        return calculatedDirection;
    }

    private float ImpactForce()
    {
        float calculatedForce;

        Debug.Log($"{gameObject.name} - velocity: {_rb.velocity} magnitude: {transform.position.magnitude} mass: {_rb.mass}");

        //TODO: Redo this.
        calculatedForce = _playerImpactForce * _playerImpactForceMultiplier; //* mass, rb.speed? ect..

        return calculatedForce;
    }
    private void OnHitDefault(bool destroyObject)
    {
        //+ sound effect?
        if (destroyObject)
        {
            Destroy(gameObject);
            return;
        }
        _col.isTrigger = false;
        _isThrown = false;
    }
}
