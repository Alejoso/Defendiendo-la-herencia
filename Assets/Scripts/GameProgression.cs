using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI; 

public class GameProgression : MonoBehaviour
{
    [SerializeField] private GameObject[] enemy;

    // See where the player is positioned and the objectives
    [SerializeField] public string playerLocation;
    [SerializeField] public string currentObjective;

    [SerializeField] private string[] keyLocations;
    private int indexKeyLocation;

    public bool didPlayerCompleteObjective; 


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


    private HashSet<string> visitedZones = new HashSet<string>();

    //Title of places animations 
    private Animator playerLocationTextAnimator;
    [SerializeField] private TextMeshProUGUI playerLocationText;

    //Information for the player
    [SerializeField] private TextMeshProUGUI currentObjectiveText;
    [SerializeField] private TextMeshProUGUI currentWaveText;
    [SerializeField] private GameObject arrowToObjective;
    private ArrowPointToCurrentObjective arrowPointToCurrentObjective; 

    void Awake()
    {
        cam = Camera.main;

        playerLocationTextAnimator = GameObject.Find("Place Titles").GetComponent<Animator>();
        currentWave = 1; 
        //Set the first location to grandpa's house
        indexKeyLocation = 0;
        currentObjective = keyLocations[indexKeyLocation];

        //Set variables for porgression
        didPlayerCompleteObjective = true;

        currentObjectiveText.text = "Current objective: " + currentObjective;

        currentWaveText.text = "Wave " + currentWave + " / " + totalWaves;

        arrowPointToCurrentObjective = arrowToObjective.transform.Find("ArrowImage").GetComponent<ArrowPointToCurrentObjective>();

        arrowPointToCurrentObjective.changeCurrentObjetive(indexKeyLocation); 

    }

    void Update()
    {
        //Spawn waves when there are no more enemies
        int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Count();
        if (currentEnemies == 0 && canSpawnWave && (totalWaves > currentWave) && !didPlayerCompleteObjective )
        {
            canSpawnWave = false;
            currentWave++;
            currentWaveText.text = "Wave " + currentWave + " / " + totalWaves; 
            WaveSpawner(currentWave);
        }

        if (currentWave == totalWaves && (currentObjective == playerLocation) && currentEnemies == 0)
        {
            currentWaveText.text = ""; 
            currentWave = 0;
            didPlayerCompleteObjective = true;
            indexKeyLocation++;
            currentObjective = keyLocations[indexKeyLocation];
            currentObjectiveText.text = "Objetivo: " + currentObjective;
            arrowToObjective.SetActive(true); 
            arrowPointToCurrentObjective.changeCurrentObjetive(indexKeyLocation); 

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

        if (playerLocation == currentObjective)
        {
            //Start new rounds
            arrowToObjective.SetActive(false); 
            didPlayerCompleteObjective = false;
            minWave = 5;
            randomWaveAdd = Random.Range(0, 4); //Generate a random wave to add to the min waves
            totalWaves = randomWaveAdd + minWave;

            WaveSpawner(currentWave);

            currentObjectiveText.text = "Objetivo: Defiende " + currentObjective; 
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
            enemyToSpawn = enemy[0];
            enemiesToSpawn = currentWave * Random.Range(2, 5);

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                Instantiate(enemyToSpawn, GenerateRandomEnemySpawn(), enemyToSpawn.transform.rotation);
            }
        }

        //Spawn enemies in El Taller
        if (indexKeyLocation == 1)
        {
            enemiesToSpawn = currentWave * Random.Range(3, 5);

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
        if (indexKeyLocation == 2)
        {
            enemiesToSpawn = currentWave * Random.Range(2, 5);

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                //Sacar una pata sola con 40% , fantasmita con 70% de probabilidad
                if(Random.value < 0.4)
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
        if (indexKeyLocation == 3)
        {
            enemiesToSpawn = currentWave * Random.Range(2, 5);

            for (int i = 0; i < enemiesToSpawn; i++)
            {

                if(Random.value < 0.3)
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

    


}
