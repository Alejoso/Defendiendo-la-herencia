using UnityEngine;

public class ManageCredits : MonoBehaviour
{
    [SerializeField] private GameObject creditsPanel;

    void Start()
    {
        // Ensure credits panel is hidden at start
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
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
}
