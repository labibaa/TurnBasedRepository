using UnityEngine;
using UnityEngine.UI;

public class ToggleChildrenVisibility : MonoBehaviour
{
    [SerializeField] private GameObject parentObject; // Parent GameObject containing the child images
    private bool areChildrenVisible = false; // Start with children being invisible
    private Button button; // The button this script is attached to
    private CanvasGroup canvasGroup; // CanvasGroup to control visibility and interactivity

    void Start()
    {
        // Get the Button and CanvasGroup components
        button = GetComponent<Button>();
        canvasGroup = GetComponent<CanvasGroup>();

        // If CanvasGroup is not already attached, add one
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Ensure children are hidden at the start
        SetChildrenVisibility(false);

        // Check and set button visibility based on GridSystem
       
    }

    // Toggle visibility when the button is pressed
    public void ToggleVisibility()
    {
        areChildrenVisible = !areChildrenVisible;
        SetChildrenVisibility(areChildrenVisible);
    }

    // Set visibility for all child images
    private void SetChildrenVisibility(bool isVisible)
    {
        Image[] childImages = parentObject.GetComponentsInChildren<Image>(true);
        foreach (Image img in childImages)
        {
            img.gameObject.SetActive(isVisible);
        }
    }

    // Continuously check the grid state and update button visibility
    void Update()
    {
        if (!areChildrenVisible)
        {
            // Continuously check and hide any newly instantiated child images
            SetChildrenVisibility(false);
        }
        

    }

   

}
