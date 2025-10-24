using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public class GameProgression : MonoBehaviour
{
    [SerializeField] private GameObject[] enemy;
    //Player

    private PlayerController playerController;

    // See where the player is positioned and the objectives
    [SerializeField] public string playerLocation;
    [SerializeField] public string currentObjective;

    private bool hasPlayerChangedLocation;

    [SerializeField] private string[] keyLocations;
    [SerializeField] private int indexKeyLocation;

    public bool didPlayerCompleteObjective;


    //Wave interaction variables
    private int randomWaveAdd;
    [SerializeField] private int minWave;
    [SerializeField] private int totalWaves;
    [SerializeField] private int currentWave;
    [SerializeField] private bool canSpawnWave;

    //Spawn variables
    private Camera cam;
    [SerializeField] private float minSpawnDistance;
    [SerializeField] private float maxSpawnDistance;


    private HashSet<string> visitedZones = new HashSet<string>();

    //Title of places animations 
    private Animator playerLocationTextAnimator;
    [SerializeField] private TextMeshProUGUI playerLocationText;

    [SerializeField] private float typeSpeed;

    private string fullText;


    //Information for the player
    [SerializeField] private GameObject currentWaveObject;
    [SerializeField] private TextMeshProUGUI currentObjectiveText;
    [SerializeField] private TextMeshProUGUI currentWaveText;
    [SerializeField] private GameObject arrowToObjective;
    private Animator arrowToObjectiveAnimator;

    private ArrowPointToCurrentObjective arrowPointToCurrentObjective;

    //Shootgun

    [SerializeField] GameObject shotgun;

    [SerializeField] private Animator transition; 


    void Awake()
    {
        cam = Camera.main;

        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        playerLocationTextAnimator = GameObject.Find("Place Titles").GetComponent<Animator>();
        currentWave = 1;
        //Set the first location to grandpa's house
        indexKeyLocation = 0;
        currentObjective = keyLocations[indexKeyLocation];

        //Set variables for porgression
        didPlayerCompleteObjective = true;

        StartTyping("Objetivo: " + currentObjective);

        currentWaveObject.SetActive(false);

        arrowPointToCurrentObjective = arrowToObjective.transform.Find("ArrowImage").GetComponent<ArrowPointToCurrentObjective>();
        arrowToObjectiveAnimator = arrowToObjective.transform.Find("ArrowImage").GetComponent<Animator>();
        arrowPointToCurrentObjective.changeCurrentObjetive(indexKeyLocation);

        hasPlayerChangedLocation = true;

        shotgun.SetActive(false);
        currentWaveObject.SetActive(false);

    }

    void Update()
    {
        //Spawn waves when there are no more enemies
        int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Count();
        if (currentEnemies == 0 && canSpawnWave && (totalWaves > currentWave) && !didPlayerCompleteObjective)
        {
            canSpawnWave = false;
            currentWave++;
            currentWaveObject.SetActive(false);
            currentWaveText.text = "Wave " + currentWave + " / " + totalWaves;
            currentWaveObject.SetActive(true);
            WaveSpawner(currentWave);
        }


    }

    void LateUpdate()
    {
        int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Count();

        if (currentWave == totalWaves && (currentObjective == playerLocation) && currentEnemies == 0)
        {
            if (indexKeyLocation == 1)
            {
                ShootgunAppear();
            }

            currentWaveObject.SetActive(false);

            currentWave = 1;
            didPlayerCompleteObjective = true;
            indexKeyLocation++;
            currentObjective = keyLocations[indexKeyLocation];

            StartTyping("Objetivo: " + currentObjective);
            arrowToObjectiveAnimator.Play("Arrow appear", 0, 0f);
            arrowPointToCurrentObjective.changeCurrentObjetive(indexKeyLocation);
            hasPlayerChangedLocation = true;
        }
    }

    public void ChangePlayerLocation(string newPlayerLocation)
    {

        if (string.IsNullOrEmpty(newPlayerLocation))
        {
            playerLocation = "";
            return;
        }

        playerLocation = newPlayerLocation;
        playerLocationText.text = newPlayerLocation;

        if (!visitedZones.Contains(newPlayerLocation))
        {
            visitedZones.Add(newPlayerLocation); //Add it to the hashset
            playerLocationTextAnimator.Play("Text fade in and out", -1, 0f); //Play the animation

        }

        if (playerLocation == currentObjective && hasPlayerChangedLocation)
        {
            //Start new rounds
            hasPlayerChangedLocation = false;
            arrowToObjectiveAnimator.Play("Arrow fade", 0, 0f);
            didPlayerCompleteObjective = false;
            GenerateWaveCounts();
            currentWaveText.text = "Wave " + currentWave + " / " + totalWaves;
            currentWaveObject.SetActive(true);

            WaveSpawner(currentWave);

            StartTyping("Objetivo: Defender " + currentObjective);
        }

    }

    void GenerateWaveCounts()
    {
        if (indexKeyLocation == 0)
        {
            minWave = 5;
            randomWaveAdd = Random.Range(0, 4); //Generate a random wave to add to the min waves
            totalWaves = randomWaveAdd + minWave;
            return;
        }

        if (indexKeyLocation == 1)
        {
            minWave = 7;
            randomWaveAdd = Random.Range(2, 4); //Generate a random wave to add to the min waves
            totalWaves = randomWaveAdd + minWave;
            return;
        }

        if (indexKeyLocation == 3)
        {
            minWave = 10;
            randomWaveAdd = Random.Range(3, 4); //Generate a random wave to add to the min waves
            totalWaves = randomWaveAdd + minWave;
            return;
        }

        if (indexKeyLocation == 4)
        {
            minWave = 12;
            randomWaveAdd = Random.Range(3, 6); //Generate a random wave to add to the min waves
            totalWaves = randomWaveAdd + minWave;
            return;
        }
    }

    //Spawn random enemies multiplying the wave number in a random between 4 and 7
    void WaveSpawner(int currentWave)
    {
        GameObject enemyToSpawn = enemy[0];
        int enemiesToSpawn = 0;

        //Sapwn enemies in the Casa del abuelo
        if (indexKeyLocation == 0)
        {
            enemiesToSpawn = currentWave * Random.Range(2, 5);
            minWave = 5;

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                //Sacar un fantasmita con probabilidad variable segun la ronda
                if (Random.value < (0.04 * currentWave))
                {
                    enemyToSpawn = enemy[1];
                } else
                {
                    enemyToSpawn = enemy[0];
                }

                Instantiate(enemyToSpawn, GenerateRandomEnemySpawn(), enemyToSpawn.transform.rotation);
            }
        }

        //Spawn enemies in El Taller
        if (indexKeyLocation == 1)
        {
            enemiesToSpawn = currentWave * Random.Range(3, 5);
            minWave = 7;
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                //Sacar un fantasmita con 50% de probabilidad
                if (Random.value < 0.5)
                {
                    enemyToSpawn = enemy[1];
                } else
                {
                    enemyToSpawn = enemy[0];
                }

                Instantiate(enemyToSpawn, GenerateRandomEnemySpawn(), enemyToSpawn.transform.rotation);
            }

        }

        //Spawn enemies in Lago
        if (indexKeyLocation == 3)
        {
            enemiesToSpawn = currentWave * Random.Range(3, 5);
            minWave = 10;

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                //Sacar una pata sola con 40% , fantasmita con 70% de probabilidad
                if (Random.value < 0.4)
                {
                    enemyToSpawn = enemy[2];
                }
                else if (Random.value < 0.7)
                {
                    enemyToSpawn = enemy[1];
                } else
                {
                    enemyToSpawn = enemy[0];
                }

                Instantiate(enemyToSpawn, GenerateRandomEnemySpawn(), enemyToSpawn.transform.rotation);
            }

        }

        //Spawn enemies in Almacen
        if (indexKeyLocation == 4)
        {
            enemiesToSpawn = currentWave * Random.Range(3, 5);
            minWave = 12;
            for (int i = 0; i < enemiesToSpawn; i++)
            {

                if (Random.value < 0.3)
                {
                    enemyToSpawn = enemy[3];
                }
                else if (Random.value < 0.7)
                {
                    enemyToSpawn = enemy[2];
                } else
                {
                    enemyToSpawn = enemy[1];
                }

                Instantiate(enemyToSpawn, GenerateRandomEnemySpawn(), enemyToSpawn.transform.rotation);
            }
        }

        canSpawnWave = true;
    }

    //Generate enemies on a circle around the player
    public Vector3 GenerateRandomEnemySpawn()
    {
        //Generate random values
        float angle = Random.Range(0f, 360f) * Mathf.Rad2Deg;
        float distance = Random.Range(minSpawnDistance, maxSpawnDistance);

        //Calculate the circular spawn position between those boundaries
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

        //Get the random spawn position
        Vector3 cameraPosition = cam.transform.position;
        Vector3 spawnPosition = new Vector3(cameraPosition.x + offset.x, cameraPosition.y + offset.y, 0);

        Collider2D hit = Physics2D.OverlapCircle(spawnPosition, 0.5f);

        //Prevent enemies from spwning out of bounds and from spawning inside an object
        if (spawnPosition.x > 60f || spawnPosition.x < -80f || spawnPosition.y > 60f || spawnPosition.y < -30f)
        {
            return GenerateRandomEnemySpawn();
        }

        if (hit)
        {
            return GenerateRandomEnemySpawn();
        } else
        {
            return spawnPosition;
        }

    }

    void ShootgunAppear()
    {
        shotgun.SetActive(true);
    }

    public void AddOneToIndexKeyLocation()
    {
        indexKeyLocation++;
        arrowPointToCurrentObjective.changeCurrentObjetive(indexKeyLocation);
        currentObjective = keyLocations[indexKeyLocation];
        StartTyping("Objetivo: " + currentObjective);

    }

    public void StartTyping(string text)
    {
        fullText = text;
        StopAllCoroutines();
        StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        currentObjectiveText.text = "";
        foreach (char c in fullText)
        {
            currentObjectiveText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    public void LoadWinScene()
    {
        StartCoroutine(LoadLevel(2)); 
    }

    IEnumerator LoadLevel(int LevelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(1.5f); 
        UnityEngine.SceneManagement.SceneManager.LoadScene("WinScene");

    }


}
