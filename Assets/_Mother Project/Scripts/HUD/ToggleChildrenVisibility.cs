using UnityEngine;
using UnityEngine.UI;

public class ToggleChildrenVisibility : MonoBehaviour
{
    [SerializeField] private GameObject parentObject; // Parent GameObject containing the child images
    [SerializeField] private GameObject otherParentObject; // Other parent object to hide when showing this one

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
        SetChildrenVisibility(parentObject, false);
        if (otherParentObject != null)
            SetChildrenVisibility(otherParentObject, false);
    }

    // Toggle visibility when the button is pressed
    public void ToggleVisibility()
    {
        // If other parent is visible, hide it and show mine directly
        if (otherParentObject != null && IsChildrenVisible(otherParentObject))
        {
            SetChildrenVisibility(otherParentObject, false);
            areChildrenVisible = true;
            SetChildrenVisibility(parentObject, true);
            return;
        }

        // Otherwise toggle normally
        areChildrenVisible = !areChildrenVisible;
        SetChildrenVisibility(parentObject, areChildrenVisible);
    }

    // Set visibility for all child images under a given parent
    private void SetChildrenVisibility(GameObject parent, bool isVisible)
    {
        if (parent == null) return;

        Image[] childImages = parent.GetComponentsInChildren<Image>(true);
        foreach (Image img in childImages)
        {
            img.gameObject.SetActive(isVisible);
        }
    }

    // Helper to check if any child of a parent is active
    private bool IsChildrenVisible(GameObject parent)
    {
        if (parent == null) return false;

        Image[] childImages = parent.GetComponentsInChildren<Image>(true);
        foreach (Image img in childImages)
        {
            if (img.gameObject.activeSelf)
                return true;
        }
        return false;
    }

    // Continuously check the grid state and update button visibility
    void Update()
    {
        if (!areChildrenVisible)
        {
            // Continuously check and hide any newly instantiated child images under my parent
            SetChildrenVisibility(parentObject, false);
        }
    }
}
