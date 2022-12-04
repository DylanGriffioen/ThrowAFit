using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;

    public static GameStates GAME_STATE = GameStates.MAIN_MENU;

    [SerializeField] GameObject playerManager;

    [Header("Game settings")]
    [Range(1, 10)]
    [SerializeField] int maxLifes = 3;
    public int MaxLifes { get { return maxLifes; } set { maxLifes = value; } }

    [Range(1f, 1000f)]
    [SerializeField] float maxHealth = 100f;
    public float MaxHealth { get { return maxHealth; } set { maxHealth = value; } }

    [Range(1f, 10f)]
    [SerializeField] float respawnTime = 3f;
    public float RespawnTime { get { return respawnTime; } set { respawnTime = value; } }

    [Range(0.1f, 10f)]
    [SerializeField] float damageMultiplier = 1;
    public float DamageMultiplier { get { return damageMultiplier; } set { damageMultiplier = value; } }

    [Range(0.1f, 10f)]
    [SerializeField] float impactForceMultiplier = 1;
    public float ImpactForceMultiplier { get { return impactForceMultiplier; } set { impactForceMultiplier = value; } }


    public int PlayerCount { get; set; }   
    [SerializeField] public GameObject[] _players;


    private void Awake()
    {
        GAME_STATE = GameStates.PREGAME;
        _players = new GameObject[8];

        MakeSingleton();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"Player count: {PlayerCount}");
    }
    private void MakeSingleton()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != null)
        {
            Destroy(gameObject);
        }
    }

    public void PlayerJoined()
    {
        PlayerCount++;
        _players = GameObject.FindGameObjectsWithTag("Player");
    }

    public void PlayerLeft()
    {
        PlayerCount--;
        _players = GameObject.FindGameObjectsWithTag("Player");

    }

    public void StartGame()
    {
        if(PlayerCount > 0) //TODO: Should be > 1
        {
            GAME_STATE = GameStates.GAME;
            DontDestroyPlayersOnLoad();
            PlayerInputManager pim = playerManager.GetComponent<PlayerInputManager>();
            pim.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;

            //Swap scene
            //Spawn players to random locations
            SceneManager.LoadScene("Game Scene");
        }
        else
        {
            Debug.Log("Error, not enough players to start game!");
        }

    }

    public void Pause() 
    {
        GAME_STATE = GameStates.PAUSE;
        //UI -> RESUME | MAIN MENU | OPTIONS | QUIT GAME
    }

    public bool LastPlayerStanding()
    {
        return PlayerCount < 2;
    }

    public void GameOver()
    {
        GAME_STATE = GameStates.GAME_OVER;
        Debug.Log("Game Over!");
        // UI -> MAIN MENU | QUIT

        //coroutine (5 sec) -> Destroy everything -> Swap to main Menu
    }

    public void LoadMainMenu()
    {
        GAME_STATE = GameStates.MAIN_MENU;

        Destroy(gameObject);
        Destroy(playerManager);

        SceneManager.LoadScene("MenuScene");
    }


    private void DontDestroyPlayersOnLoad()
    {
        foreach(GameObject go in _players)
        {
            DontDestroyOnLoad(go);
        }
    }

    private void DestroyPlayers()
    {
        foreach (GameObject go in _players)
        {
            Destroy(go);
        }
    }

    public void PlayerLost(GameObject player)
    {
        Debug.Log($"Player: {player} died!");
        for (int i = 0; i < _players.Length; i++)
        {
            if (_players[i].Equals(player))
            {
                _players[i] = null;
            }
        }
        PlayerCount--;

        if (LastPlayerStanding())
        {
            GameOver();
        }
    }
}
