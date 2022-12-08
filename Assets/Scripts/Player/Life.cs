using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Life : MonoBehaviour
{

    [SerializeField] int maxLives = 3;
    [SerializeField] int currentLives;
    public bool Alive { get; private set; }

    void Awake()
    {
        if (GameManager._instance != null)
        {
            maxLives = GameManager._instance.MaxLives > 0 ? GameManager._instance.MaxLives : maxLives;
        }
        else
        {
            //Debug.Log("1");
        }
        currentLives = maxLives;
        Alive = true;
    }
    private void Update()
    {
        if (GameManager.GAME_STATE == GameStatus.PREGAME && GameManager._instance != null)
        {
            maxLives = GameManager._instance.MaxLives > 0 ? GameManager._instance.MaxLives : maxLives;
            currentLives = maxLives;
        }
    }
    public void Gain(int amount)
    {
        if (!Alive) { return; }
        currentLives += amount;
        if (currentLives > maxLives) { currentLives = maxLives; }
        print("Lives: " + currentLives);
    }

    public void Lose(int amount)
    {
        if (!Alive || !(GameManager.GAME_STATE == GameStatus.GAME)) { return; }
        currentLives -= amount;
        if (currentLives <= 0) { Kill(); }
        print("Lives: " + currentLives);
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
        gameObject.SetActive(false); //Player input manager gives index out of bounds error..
        //Destroy(gameObject); //Player input manager gives index out of bounds error..
        //Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        //rb.detectCollisions = false;
        //rb.useGravity = false;
        //rb.velocity = Vector3.zero;
        //gameObject.GetComponent<Collider>().enabled = false;
        //gameObject.GetComponent<Movement>().enabled = false;

    }

}
