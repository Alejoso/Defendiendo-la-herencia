using UnityEngine;
using UnityEngine.SceneManagement; 

public class StartScreen : MonoBehaviour
{
    public void LoadMainGame () {
        SceneManager.LoadScene(2); 
    }
}
