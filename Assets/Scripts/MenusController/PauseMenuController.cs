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

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ResumeGame(); 
        }
    }

    public void ResumeGame()
    {        
        StartCoroutine(DisplayAnimation()); 
    }

    IEnumerator DisplayAnimation()
    {
        pauseMenuTransition.Play("Pause menu dissapier" , 0 , 0f);
        yield return new WaitForSecondsRealtime(0.3f);
        Time.timeScale = 1; 
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
