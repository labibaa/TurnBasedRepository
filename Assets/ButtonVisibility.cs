using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonVisibility : MonoBehaviour
{
    private Button button;
    private CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        CheckButtonVisibility();
    }

    // Update is called once per frame
    void Update()
    {
        CheckButtonVisibility();
    }

    private void CheckButtonVisibility()
    {
        if (GridSystem.instance != null && button != null)
        {
            bool isGridOn = GridSystem.instance.IsGridOn; // Cache the grid state


            // Set the button's visibility and interactability based on grid state
            if (isGridOn && TempManager.instance.attacker.GetComponent<TemporaryStats>().CharacterTeam == TeamName.TeamA)
            {
                canvasGroup.alpha = 1f;  // Fully visible
                canvasGroup.interactable = true;  // Interactable
                canvasGroup.blocksRaycasts = true; // Allows clicking
            }
            else
            {
                canvasGroup.alpha = 0f;  // Fully invisible
                canvasGroup.interactable = false;  // Non-interactable
                canvasGroup.blocksRaycasts = false; // Prevents clicking
            }
        }
        else
        {
            Debug.LogWarning("GridSystem instance or button is null!");
        }
    }
}
