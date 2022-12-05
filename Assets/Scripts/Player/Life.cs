using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Life : MonoBehaviour
{

    [SerializeField] int maxLifes = 3;
    [SerializeField] int currentLifes;
    public bool Alive { get; private set; }

    void Awake()
    {
        if (GameManager._instance != null)
        {
            maxLifes = GameManager._instance.MaxLifes > 0 ? GameManager._instance.MaxLifes : maxLifes;
        }
        currentLifes = maxLifes;
        Alive = true;
    }
    private void Update()
    {
        if (GameManager.GAME_STATE == GameStates.PREGAME && GameManager._instance != null)
        {
            maxLifes = GameManager._instance.MaxLifes > 0 ? GameManager._instance.MaxLifes : maxLifes;
        }
    }
    public void Gain(int amount)
    {
        if (Alive)
        {
            currentLifes = (currentLifes + amount) > maxLifes ? maxLifes : (currentLifes + amount);
        }
    }

    public void Lose(int amount)
    {
        if (Alive && GameManager.GAME_STATE == GameStates.GAME)
        {
            currentLifes -= amount;

            if (currentLifes <= 0)
            {
                Kill();
            }
        }
    }

    public void Kill()
    {
        GameManager._instance.PlayerLost(gameObject);
        Alive = false;
        OnDead();
    }

    private void OnDead()
    {
        // TODO: remove player..
        //gameObject.SetActive(false); //Player input manager gives index out of bounds error..
        //Destroy(gameObject); //Player input manager gives index out of bounds error..
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.detectCollisions = false;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        gameObject.GetComponent<Collider>().enabled = false;
        gameObject.GetComponent<Movement>().enabled = false;
    }

}
