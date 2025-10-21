using System.CodeDom.Compiler;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

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
    void Awake()
    {
        cam = Camera.main;

        randomWaveAdd = Random.Range(0, 4); //Generate a random wave to add to the min waves

        //Set up basic variables
        totalWaves = randomWaveAdd + minWave; 
        currentWave = 1;
        WaveSpawner(currentWave);
        canSpawnWave = true;

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
        
        if(currentWave > totalWaves)
        {
            Debug.Log("You win"); 
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
}
