using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseHandler : MonoBehaviour
{
    public void OnPause(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
            return;

        Debug.Log("PAUSE!");
        if (GameManager.GAME_STATE != GameStatus.GAME)
            return;

        GameObject pauseScreen = GameManager._instance.GameCanvas.transform.GetChild(0).gameObject;
        if (pauseScreen != null)
            return;

        if (GameManager.GAME_STATE != GameStatus.PAUSE)
        {
            Debug.Log("Game paused!");
            Time.timeScale = 0;
            GameManager.GAME_STATE = GameStatus.PAUSE;
            pauseScreen.SetActive(true);
        }
        else
        {
            Debug.Log("Game unpaused!");
            Time.timeScale = 1;
            GameManager.GAME_STATE = GameStatus.GAME;
            pauseScreen.SetActive(false);
        }
    }
}
