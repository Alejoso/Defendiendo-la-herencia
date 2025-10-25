using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioMixer audioMixer;

    private float currentVolume; 

    void Awake()
        {
        if (audioMixer.GetFloat("Master", out currentVolume))
        {
            // Normalmente los valores del mixer están en decibelios (de -80 a 0)
            // Si tu slider usa un rango 0-1, necesitas convertirlo:
            volumeSlider.value = currentVolume; 
        }    
        }
    public void ChangeVolume()
    {
        audioMixer.SetFloat("Master", volumeSlider.value); 
    }
}
