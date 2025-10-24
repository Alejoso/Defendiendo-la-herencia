using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class IntroAnimationController : MonoBehaviour
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

    void Start()
    {
        textIndex = 0;
        StartTyping(texts[textIndex]);
        interaction.SetActive(false);
        buttonInteracion.SetActive(false); 
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

        if (textIndex == 4)
        {
            abuelo.sprite = abueloFaces[0];
        }
        else if (textIndex < 7)
        {
            abuelo.sprite = abueloFaces[1];
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

    public void LoadGame()
    {
        SceneManager.LoadScene(2); //Load game
    }
    public void StartTypingBadResponse(string text)
    {
        canContinue = false;
        fullText = text;
        StopAllCoroutines();
        StartCoroutine(TypeTextBadResponse());

    }
    
        private IEnumerator TypeTextBadResponse()
    {
        textDisplay.text = "";
        foreach (char c in fullText)
        {
            textDisplay.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(2); 
    }
    public void Badresponse()
    {
        abuelo.sprite = abueloFaces[2];
        desition.SetActive(false);
        StartTypingBadResponse("Mijo, ¿usted se apendejó o es que ese tistos lo está controlando mucho? Vaya, pues, mijo, y me hace el cruce.");
    }
}
