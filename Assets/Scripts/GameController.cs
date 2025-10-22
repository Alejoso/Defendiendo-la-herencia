using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject enemy;

    //Wave interaction variables
    private int randomWaveAdd;
    [SerializeField] private int minWave;
    [SerializeField] private int totalWaves; 
    [SerializeField] private int currentWave;
    private bool canSpawnWave;

    //Spawn variables
    private Camera cam;
    [SerializeField] private float minSpawnDistance;
    [SerializeField] private float maxSpawnDistance;


    //See where the player is positioned
    [SerializeField] private string playerLocation;

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

    void Awake()
    {
        cam = Camera.main;
        audioSource = GetComponent<AudioSource>(); 
        randomWaveAdd = Random.Range(0, 4); //Generate a random wave to add to the min waves

        //Set up basic variables
        totalWaves = randomWaveAdd + minWave; 
        currentWave = 1;
        WaveSpawner(currentWave);
        canSpawnWave = true;
        playerLocation = "Spawn";

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
        //Spawn waves when there are no more enemies
        int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Count();
        if (currentEnemies == 0 && canSpawnWave && (totalWaves >= currentWave))
        {
            canSpawnWave = false;
            currentWave++;
            WaveSpawner(currentWave);
        }

    }

    //Spawn random enemies multiplying the wave number in a random between 4 and 7
    void WaveSpawner(int currentWave)
    {
        int enemiesToSpawn = currentWave * Random.Range(4, 7);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Instantiate(enemy, GenerateRandomEnemySpawn(), enemy.transform.rotation);
        }

        canSpawnWave = true;
    }

    //Generate enemies on a circle around the player
    Vector3 GenerateRandomEnemySpawn()
    {
        //Generate random values
        float angle = Random.Range(0f, 360f) * Mathf.Rad2Deg;
        float distance = Random.Range(minSpawnDistance, maxSpawnDistance);

        //Calculate the circular spawn position between those boundaries
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

        //Get the random spawn position
        Vector3 cameraPosition = cam.transform.position;
        Vector3 spawnPosition = new Vector3(cameraPosition.x + offset.x, cameraPosition.y + offset.y, 0);

        return spawnPosition;
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
    

    public void ChangePlayerLocation(string newPlayerLocation)
    {
        playerLocation = newPlayerLocation;
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
