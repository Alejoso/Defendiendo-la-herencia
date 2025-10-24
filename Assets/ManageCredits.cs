using UnityEngine;

public class ManageCredits : MonoBehaviour
{
    [SerializeField] private GameObject creditsPanel;

    [SerializeField] private GameObject controlsPanel; 
    void Start()
    {
        // Ensure credits panel is hidden at start
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }

        if(controlsPanel != null)
        {
            controlsPanel.SetActive(false);
        }
    }

    // Call this from a button to open the credits
    public void OpenCredits()
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Credits panel is not assigned!");
        }
    }

    // Call this from a button to close the credits
    public void CloseCredits()
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Credits panel is not assigned!");
        }
    }

    public void OpenControls()
    {
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Controls panel is not assigned!");
        }
        
    }
    
    public void CloseControls()
    {
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false); 
        }
        else
        {
            Debug.LogWarning("Controls panel is not assigned!");
        }
    }
}
