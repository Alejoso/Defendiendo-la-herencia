using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
public class DeathScreenManager : MonoBehaviour
{
    [SerializeField] private string[] texts;
    [SerializeField] private int textIndex;
    [SerializeField] private bool canContinue;

    private string fullText;
    private bool isTyping = false;

    [SerializeField] private TextMeshProUGUI textDisplay;
    [SerializeField] private TextMeshProUGUI interacionDisplay;

    [SerializeField] private GameObject buttonInteracion; 
    [SerializeField] private GameObject interaction;

    [SerializeField] private float typeSpeed = 0.05f;

    [SerializeField] private GameObject desition; 

    //Different faces
    [SerializeField] private Sprite[] abueloFaces;
    [SerializeField] private Image abuelo; 

    [SerializeField] private GameObject canvasTransition;
    private Animator transition;

    [SerializeField] private float transitionTime; 


    void Awake()
    {
        textIndex = 0;
        StartTyping(texts[0]);
        interaction.SetActive(false);
        buttonInteracion.SetActive(false); 
        transition = canvasTransition.GetComponent<Animator>(); 

    }

   void Update()
{
        if (Input.anyKeyDown || (canContinue && textIndex % 2 == 1))
        {
            if (isTyping)
            {
                if (textIndex % 2 == 0)
                {
                    SkipTextAnimation();

                }
                else
                {
                    SkipTextAnimationInteracion();

                }
                // Si todavía se está tipeando → mostrar todo el texto
            }
            else if (canContinue)
            {
                // Si ya terminó → pasar al siguiente texto
                textIndex++;

                if (textIndex > texts.Length )
                {
                    textDisplay.text = "";
                    desition.SetActive(true);
                }
                
                if (textIndex < texts.Length )
                {

                    if (textIndex % 2 == 0)
                    {
                        interaction.SetActive(false);
                        StartTyping(texts[textIndex]);

                    }
                    else
                    {
                        textDisplay.text = "";
                        interaction.SetActive(true);
                        StartTypingInteraction(texts[textIndex]);
                    }
                }
            }
        }

    
}

    void SkipTextAnimation()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            textDisplay.text = fullText;
            isTyping = false;
            canContinue = true;
        }
    }

    void SkipTextAnimationInteracion()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            interacionDisplay.text = fullText;
            isTyping = false;
            buttonInteracion.SetActive(true); 

        }
    }

    public void StartTyping(string text)
    {
        canContinue = false;
        fullText = text;
        StopAllCoroutines();
        StartCoroutine(TypeText());
        
    }

    private IEnumerator TypeText()
    {
        isTyping = true;
        textDisplay.text = "";
        foreach (char c in fullText)
        {
            textDisplay.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
        canContinue = true;
  
    }

    public void StartTypingInteraction(string text)
    {
        canContinue = false;
        fullText = text;
        StopAllCoroutines();
        StartCoroutine(TypeTextInteraction());
    }

    private IEnumerator TypeTextInteraction()
    {
        isTyping = true;
        interacionDisplay.text = "";
        foreach (char c in fullText)
        {
            interacionDisplay.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
        buttonInteracion.SetActive(true); 

    }

    public void CanContinue()
    {
        canContinue = true;
        buttonInteracion.SetActive(false);

    }

    public void LoadMenuScene()
    {
        StartCoroutine(LoadLevel(1)); 
    }

    public void LoadMainGame()
    {
        StartCoroutine(LoadLevel(2)); 
    }

    IEnumerator LoadLevel(int LevelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime); 
        SceneManager.LoadScene(LevelIndex);

    }
}
