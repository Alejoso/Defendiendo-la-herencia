using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class PauseMenuController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Animator transition;
    private Animator pauseMenuTransition; 
    void Awake()
    {
        pauseMenuTransition = GetComponent<Animator>(); 
    }

    public void ResumeGame()
    {        
        Time.timeScale = 1; 
        StartCoroutine(DisplayAnimation()); 
    }

    IEnumerator DisplayAnimation()
    {
        pauseMenuTransition.Play("Pause menu dissapier" , 0 , 0f);
        yield return new WaitForSeconds(0.3f);
        gameObject.SetActive(false); 
    }
    
    public void LoadMenu()
    {
        StartCoroutine(LoadLevel(0)); 
    }

    IEnumerator LoadLevel(int LevelIndex)
    {
        Time.timeScale = 1;
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(LevelIndex); 

    }
}
