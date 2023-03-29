using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] Animator animator;
    private AudioClip Click;
    private AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Click = (AudioClip)Resources.Load("MenuButtonSelect");
    }
    public void StartGame()
    {
        // Need to add scene hierarchy to "file>build settings" when the scenes are ready
        // Now only prints text
        // Switching to the next scene in hierarchy
        StartCoroutine(PlaySound());
        //Debug.Log("Load the game scene!");
    }

    public void Quit()
    {
        // Quits the application, doesn't work in the unity environment
        // Prints the text for indication
        Debug.Log("You pressed the quit button");
        Application.Quit();
    }

    IEnumerator PlaySound()
    {
        audioSource.clip = Click;
        float fadeOutTime = 1f;
        animator.SetTrigger("FadeOut");
        
        yield return new WaitForSeconds(fadeOutTime);
        //yield return new WaitUntil(() => audioSource.isPlaying == false);
        SceneManager.LoadScene("PregameScene");
    }
}
