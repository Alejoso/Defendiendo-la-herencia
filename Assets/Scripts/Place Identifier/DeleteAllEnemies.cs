using System.Linq;
using TMPro;
using UnityEngine;

public class DeleteAllEnemies : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private TextMeshProUGUI enemyCountText; 
    void Update()
    {
        int enemyQuatity = GameObject.FindGameObjectsWithTag("Enemy").Count();
        enemyCountText.text = "Matar a: " + enemyQuatity; 
    }
    public void DeleteEnemies()
    {
        // Encuentra todos los objetos con el tag "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Recorre cada uno y lo destruye
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        Debug.Log($"Se eliminaron {enemies.Length} enemigos.");
    }
}
