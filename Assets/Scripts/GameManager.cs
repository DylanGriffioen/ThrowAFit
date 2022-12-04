using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
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
    [SerializeField] float impactForceMultiplier = 1;
    public float ImpactForceMultiplier { get { return impactForceMultiplier; } set { impactForceMultiplier = value; } }


    public int PlayerCount { get; private set; }
    [SerializeField] public GameObject[] _players;


    private void Awake()
    {
        GAME_STATE = GameStates.PREGAME;
        _players = new GameObject[8];

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(playerManager);
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

    public void NewPlayerJoined()
    {
        PlayerCount++;
        _players = GameObject.FindGameObjectsWithTag("Player");

    }

    public void StartGame()
    {
        GAME_STATE = GameStates.GAME;
        DontDestroyPlayersOnLoad();
        //Swap scene
        //Spawn players to random locations
        SceneManager.LoadScene("Game Scene");

    }

    public void Pause() 
    {
        GAME_STATE = GameStates.PAUSE;
        //UI -> RESUME | MAIN MENU | OPTIONS | QUIT GAME
    }

    public void GameOver()
    {
        GAME_STATE = GameStates.GAME_OVER;
        // UI -> MAIN MENU | QUIT
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


}
