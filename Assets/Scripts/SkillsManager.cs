using UnityEngine.EventSystems;
using UnityEngine;

public class SkillsManager : MonoBehaviour
{
    private PlayerController playerController;
    private AudioSource gameControllerAudioSource;
    [SerializeField] private AudioClip selectSkillSFX; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        gameControllerAudioSource = GameObject.Find("GameController").GetComponent<AudioSource>(); 
    }

    public void LifeSkill()
    {
        playerController.AddMaxHealt(20f);
        gameObject.SetActive(false);
        Time.timeScale = 1;
        gameControllerAudioSource.PlayOneShot(selectSkillSFX); 
    }
    public void DamageSkill()
    {
        playerController.AddDamage(4);
        gameObject.SetActive(false);
        Time.timeScale = 1; 
        gameControllerAudioSource.PlayOneShot(selectSkillSFX); 
    }
       public void SpeedSkill()
    {
        playerController.AddSpeed(0.1f); 
        gameObject.SetActive(false);
        Time.timeScale = 1; 
        gameControllerAudioSource.PlayOneShot(selectSkillSFX);
    }
    
}
