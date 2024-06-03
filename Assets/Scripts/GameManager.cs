using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;

    public static GameStatus GAME_STATE = GameStatus.MAIN_MENU;

    [SerializeField] GameObject playerManager;
    [SerializeField] GameObject gameCanvas;
    [SerializeField] Animator gameCanvasAnimator;
    [SerializeField] Animator preGameCanvasAnimator;

    public GameObject GameCanvas { get { return gameCanvas; } }

    [Header("Game settings")]
    [Range(1, 10)]
    [SerializeField] int maxLifes = 3;
    public int MaxLives { get { return maxLifes; } set { maxLifes = value; } }

    [Range(1f, 1000f)]
    [SerializeField] float maxHealth = 100f;
    public float MaxHealth { get { return maxHealth; } set { maxHealth = value; } }

    [Range(0.5f, 10f)]
    [SerializeField] float respawnTime = 3f;
    public float RespawnTime { get { return respawnTime; } set { respawnTime = value; } }

    [Range(0.1f, 10f)]
    [SerializeField] float damageMultiplier = 1;
    public float DamageMultiplier { get { return damageMultiplier; } set { damageMultiplier = value; } }

    [Range(0.1f, 10f)]
    [SerializeField] float forceMultiplier = 1;
    public float ForceMultiplier { get { return forceMultiplier; } set { forceMultiplier = value; } }

    [Range(1, 100)]
    [SerializeField] int maxItemAmount = 10;
    public int MaxItemAmount { get { return maxItemAmount; } set { maxItemAmount = value; } }

    [Range(0.1f, 10f)]
    [SerializeField] float itemSpawnInterval = 3f;
    public float ItemSpawnInterval { get { return itemSpawnInterval; } set { itemSpawnInterval = value; } }

    public int PlayerCount { get; set; }   
    [SerializeField] GameObject[] _players;

    public GameObject[] Players() { return _players; }

    private bool _playersSet = false;

    private GameObject buttonStart;

    private void Awake()
    {
        GAME_STATE = GameStatus.PREGAME;
        _players = new GameObject[8];
        MakeSingleton();
    }

    // Start is called before the first frame update
    void Start()
    {
        buttonStart = GameObject.Find("Start Game");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"Player count: {PlayerCount}");
        if (GAME_STATE == GameStatus.PREGAME)
        {
            if (PlayerCount < 1) //TODO: Should be < 2
            {
                buttonStart.SetActive(false);
            }
            else
            {
                buttonStart.SetActive(true);
            }
        }

        Debug.Log("gameCanvas: " + gameCanvas);
        if (gameCanvas == null)
        {
            if(GameManager.GAME_STATE == GameStatus.GAME) 
            {
                if(SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(2))
                {
                    gameCanvas = GameObject.Find("Game_Canvas");
                    if(gameCanvas != null)
                    {
                        gameCanvasAnimator = gameCanvas.GetComponent<Animator>();
                    }
                }
            }
        }
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
        StartCoroutine(FadeOutToPrepare());
    }

    IEnumerator FadeOutToPrepare()
    {
        float fadeOutTime = 1f;
        preGameCanvasAnimator.SetTrigger("FadeOut");

        yield return new WaitForSeconds(fadeOutTime);
        PrepareGame();
    }

    private void PrepareGame()
    {
        if (PlayerCount > 0) //TODO: Should be > 1 
        {
            DontDestroyPlayersOnLoad();
            PlayerInputManager pim = playerManager.GetComponent<PlayerInputManager>();
            pim.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;

            SceneManager.LoadScene("Game Scene");
            if (!_playersSet)
            {
                GameObject spawnArea = GameObject.Find("ThrowAFitBuilding");
                float dropHeight = 2f;
                float distanceToEdge = 90f;

                //give player random location and move to parent "Players"
                foreach (GameObject player in _players)
                {
                    //remove player held item
                    
                    ItemInteraction itemInteraction = player.GetComponentInChildren<ItemInteraction>();
                    if (itemInteraction != null)
                    {
                        itemInteraction.DropDestroyItem();
                    }
                    PunchKick pk = player.GetComponentInChildren<PunchKick>();
                    if (pk != null)
                    {
                        pk.ObjectsInHitbox.Clear();
                    }


                    if (spawnArea != null)
                        player.transform.position = RandomLocation.GetRandomLocationOnObject(spawnArea, distanceToEdge, dropHeight);

                    //player.GetComponentInChildren<GroundCheck>().onGround = false;
                    //player.GetComponentInChildren<Movement>().onGround = false;
                    //player.GetComponentInChildren<Movement>().jumping = true;

                }
                _playersSet = true;
                GAME_STATE = GameStatus.GAME;
                StartCoroutine(HandleCountDown());
            }
        }
        else
        {
            Debug.Log("Error, not enough players to start game!");
        }
    }

    public bool LastPlayerStanding()
    {
        return PlayerCount < 2;
    }

    public void GameOver()
    {
        if(GAME_STATE == GameStatus.GAME)
        {
            GAME_STATE = GameStatus.GAME_OVER;
            Debug.Log("Game Over!");

            /*GameObject gameOverScreen = gameCanvas.transform.GetChild(1).gameObject;
            if (gameOverScreen != null)
                gameOverScreen.SetActive(true);
            else
                Debug.Log("GameOverScreen not found!");*/

            StartCoroutine(BackToMainMenu());
        }

    }

    public void LoadMainMenu()
    {
        GAME_STATE = GameStatus.MAIN_MENU;

        DestroyPlayers();
        Destroy(playerManager);
        Destroy(gameCanvas);
        Destroy(gameObject);
        //SceneManager.LoadScene("MenuScene");
        SceneManager.LoadScene("MenuScene_SebastianTest");
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
        foreach(GameObject player in _players)
        {
            Destroy(player);
        }
    }

    public void PlayerLost(GameObject player)
    {
        Debug.Log($"Player: {player} died!");
        /*for (int i = 0; i < _players.Length; i++)
        {
            if (_players[i].Equals(player))
            {
                _players[i] = null;
            }
        }*/
        PlayerCount--;

        if (LastPlayerStanding())
        {
            GameOver();
        }
    }

    IEnumerator BackToMainMenu()
    {
        GameObject gameOverScreen = gameCanvas.transform.GetChild(1).gameObject;
        if (gameOverScreen == null)
        {
            LoadMainMenu();
            yield break;
        }

        gameOverScreen.SetActive(true);
        gameCanvasAnimator.SetTrigger("GameOver");
        
        float animationTime = 4.3f;
        
        yield return new WaitForSeconds(animationTime);
        LoadMainMenu();
    }


    IEnumerator HandleCountDown()
    {
        while (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(2))
        {
            Debug.Log($"scene: {SceneManager.GetActiveScene()}%");
            yield return null;
        }

        GAME_STATE = GameStatus.COUNTDOWN;
        GameObject countDownScreen = gameCanvas.transform.GetChild(2).gameObject;
        Debug.Log("CDS: " + countDownScreen);
        if (countDownScreen == null)
            yield break;

        float animationTime = 3.4f;

        FreezePlayer(true);
        countDownScreen.SetActive(true);
        gameCanvasAnimator.SetTrigger("CountDown");

        yield return new WaitForSeconds(animationTime);


        if (countDownScreen != null)
            countDownScreen.SetActive(false);

        FreezePlayer(false);
        GAME_STATE = GameStatus.GAME;
    }


    private void FreezePlayer(bool freeze)
    {
        foreach(GameObject player in _players)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if(rb != null)
            {
                if (freeze)
                {
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                }
                else
                {
                    rb.constraints = RigidbodyConstraints.None;
                    rb.constraints = RigidbodyConstraints.FreezeRotation;
                }
            }
        }
    }
}
