using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;


public class GameController : MonoBehaviour
{

    //XP Controll
    public float playerXP;
    [SerializeField] private int playerLevel;
    [SerializeField] private float xpNeedForLevelUP;
    [SerializeField] private Image xpFill;
    [SerializeField] private TextMeshProUGUI lvText;

    //Canvas skill levelUp
    [SerializeField] private GameObject skillMenu;

    //Audios
    [SerializeField] private AudioClip levelUpSFX;

    [HideInInspector] public AudioSource audioSource;

    [SerializeField] private GameObject pauseMenu;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        //playerLocation = "Spawn";

        //Player basic xp variables
        playerLevel = 1;
        playerLevel = 1;
        xpNeedForLevelUP = CalculateXPNeededForLevelUp(playerLevel);
        lvText.text = "LV" + playerLevel;
        playerXP = 0;
        xpFill.fillAmount = playerXP / xpNeedForLevelUP;

        skillMenu.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.SetActive(true); 
            Time.timeScale = 0;
        }
    }

    void HandleLevel()
    {

        if (playerXP >= xpNeedForLevelUP)
        {
            SkillSelection(); 
            playerLevel++;

            lvText.text = "LV " + playerLevel;

            xpNeedForLevelUP = CalculateXPNeededForLevelUp(playerLevel); 
            playerXP = 0;

        }
        
        xpFill.fillAmount = playerXP / xpNeedForLevelUP; 

    }
    

    public void AddXp(float xpToAdd)
    {
        playerXP += xpToAdd;
        HandleLevel();
    }

    float CalculateXPNeededForLevelUp(float playerLevel)
    {
        return 100 * Mathf.Pow(1.3f, playerLevel);
    }
    
    void SkillSelection()
    {
        audioSource.PlayOneShot(levelUpSFX);
        Time.timeScale = 0;
        skillMenu.SetActive(true);

    } 

}
