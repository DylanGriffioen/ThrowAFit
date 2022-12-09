using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] float hitForceMultipler = 1f;

    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth;
    [SerializeField] float respawnTime = 5f;
    [SerializeField] bool alive;
    [SerializeField] Life lifes;
    private Vector3 _respawnSpot;

    PlaySFX sfxPlayer;

    // Start is called before the first frame update
    void Awake()
    {
        if (GameManager._instance != null)
        {
            maxHealth = GameManager._instance.MaxHealth > 0 ? GameManager._instance.MaxHealth : maxHealth;
            respawnTime = GameManager._instance.RespawnTime > 0 ? GameManager._instance.RespawnTime : respawnTime;
        }
        currentHealth = maxHealth;
        alive = true;
        _respawnSpot = new Vector3(0f, 100f, 0f);
        sfxPlayer = GameObject.Find("SFX").GetComponent<PlaySFX>();
    }
    public float GetHitForceMultiplier() { return hitForceMultipler; }

    private void Update()
    {
        if(GameManager.GAME_STATE == GameStatus.PREGAME && GameManager._instance != null)
        {
            maxHealth = GameManager._instance.MaxHealth > 0 ? GameManager._instance.MaxHealth : maxHealth;
            currentHealth = maxHealth;
            respawnTime = GameManager._instance.RespawnTime > 0 ? GameManager._instance.RespawnTime : respawnTime;
        }
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
            hitForceMultipler += amount * 0.01f;
            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                //Kill();
            }
        }
        sfxPlayer.TakeDamage();
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
            PlayerComponents(false);
            gameObject.transform.position = _respawnSpot;
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
        hitForceMultipler = 1.0f;
        alive = true;
        Respawn playerRespawner = gameObject.GetComponent<Respawn>();
        if(playerRespawner != null)
        {
            PlayerComponents(true);
            GameObject spawnArea = GameObject.Find("ThrowAFitBuilding");
            float dropHeight = 2f;
            float distanceToEdgePercent = 90f;
            gameObject.GetComponentInChildren<ItemInteraction>().DropDestroyItem();
            //give player random location
            if(spawnArea != null)
            {
                playerRespawner.RespawnPlayer(RandomLocation.GetRandomLocationOnObject(spawnArea, distanceToEdgePercent, dropHeight));
            }
            else
            {
                playerRespawner.RespawnPlayer();
            }
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
