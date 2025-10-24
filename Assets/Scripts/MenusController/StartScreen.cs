using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class StartScreen : MonoBehaviour
{
    [SerializeField] private GameObject canvasTransition;
    private Animator transition;

    [SerializeField] private float transitionTime; 


    void Awake()
    {
        transition = canvasTransition.GetComponent<Animator>(); 
    }
    public void LoadMainGame()
    {
        StartCoroutine(LoadLevel(1)); 
    }

    IEnumerator LoadLevel(int LevelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime); 
        SceneManager.LoadScene(1);

    }


    public void LeaveGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
                Application.Quit(); // original code to quit Unity player
#endif
    }
}
