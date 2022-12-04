using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth;
    [SerializeField] float respawnTime = 5f;
    [SerializeField] bool alive;
    [SerializeField] Life lifes;
    [SerializeField] Vector3 respawnSpot;

    // Start is called before the first frame update
    void Awake()
    {
        currentHealth = maxHealth;
        alive = true;
        respawnSpot = new Vector3(0f, 100f, 0f);
    }

    public void Heal(float amount)
    {
        if (alive)
        {
            currentHealth = (currentHealth + amount) > maxHealth ? maxHealth : (currentHealth + amount);
        }
    }

    public void Damage(float amount)
    {
        if (alive)
        {
            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                Kill();
            }
        }
    }

    public void Kill()
    {
        currentHealth = 0;
        alive = false;
        OnDead();
    }

    private void OnDead()
    {
        if(lifes != null)
        {
            lifes.Lose(1);
            //Animation?
            PlayerComponents(false);
            gameObject.transform.position = respawnSpot;
            if (lifes.Alive)
            {
                StartCoroutine("Respawn");
            }
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        currentHealth = maxHealth;
        Respawn playerRespawner = gameObject.GetComponent<Respawn>();

        if(playerRespawner != null)
        {
            PlayerComponents(true);
            playerRespawner.RespawnPlayer();
        }
    }


    private void PlayerComponents(bool activate)
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.detectCollisions = activate;
        rb.useGravity = activate;
        rb.velocity = Vector3.zero;
        gameObject.GetComponent<Collider>().enabled = activate;
        gameObject.GetComponent<Movement>().enabled = activate;
    }
}
