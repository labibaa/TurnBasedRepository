using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class ButtonStackManager : MonoBehaviour
{
    public static ButtonStackManager instance;
    public GameObject imagePrefab; // Reference your image prefab
    public Transform stackPanel; // Reference your UI panel with Vertical Layout Group
    GameObject actionPanel;
    [SerializeField]
    Transform parentPanel;
    [SerializeField]
    GameObject ultimateBar;
    [SerializeField]
    RectTransform UltimateRectPanel;
    [SerializeField]
    RectTransform UltimateSpawnRectPanel;
    [SerializeField]
    GameObject endTurnButtonPrefab;
    [SerializeField]
    GameObject moveButtonPrefab;
    [SerializeField]
    GameObject undoButtonPrefab;

    List<GameObject> commonButtons = new List<GameObject>();
    private void Start()
    {
        // Ensure the stackPanel has a Vertical Layout Group component
        if (stackPanel == null || stackPanel.GetComponent<VerticalLayoutGroup>() == null)
        {
            Debug.LogError("The stackPanel is missing or doesn't have a Vertical Layout Group component.");
            return;
        }
    }
    private void Awake()
    {

        instance = this;

    }

    public void OnButtonPressed(GameObject imagePrefa)
    {
        // Clone the image and add it to the stacks
        GameObject newImage = Instantiate(imagePrefa, stackPanel);

        // Add animation to the cloned image (e.g., scale it up)
        newImage.transform.localScale = Vector3.zero; // Set the initial scale to zero
        newImage.transform.DOScale(Vector3.one, 0.3f)
            .OnComplete(() =>
            {
                // When animation completes, reset the scale
                newImage.transform.DOScale(Vector3.one, 0.3f);
            });
    }

    public void ClearStack()
    {
        // Destroy or deactivate all child elements in the stackPanel
        foreach (Transform child in stackPanel)
        {
            Destroy(child.gameObject); // Use Destroy if you want to remove them, or child.gameObject.SetActive(false) if you want to deactivate them.
        }
    }
    public void UndoStackEntry()
    {

        UI.instance.ResetPanels();
        TurnManager.instance.ResetTargetHIghlightVisual();
        GridMovement.instance.ResetHighlightedPath();
        TurnManager.instance.targetsInRange.Clear();
        TurnManager.instance.nonCharacterTargetsInRange.Clear();
        HandleTurnNew.instance.UndoTurn();
        // Get the last child element in the stackPanel
        int childCount = stackPanel.childCount;
        if (childCount > 0)
        {
            Transform lastChild = stackPanel.GetChild(childCount - 1);
            Destroy(lastChild.gameObject); // Remove the last stack entry
        }
    }

    public GameObject PopulateUltimateBar(CharacterBaseClasses player)
    {
        GameObject ultimateBarSpawned = Instantiate(ultimateBar, UltimateRectPanel.position, Quaternion.identity);
        ultimateBarSpawned.transform.SetParent(UltimateRectPanel, false);
        ultimateBarSpawned.name = player.name + "ultimate";
        ultimateBarSpawned.GetComponent<UltimateUI>().maxProgress = player.GetPlayerUltimate().GetultimateThreshold();
        ultimateBarSpawned.GetComponent<UltimateUI>().ultimateBarProgress = 0;
        return ultimateBarSpawned;
    }
    public GameObject PopulateActionPanel(CharacterBaseClasses player)
    {
        // Create a new player panel
        GameObject playerPanel = new GameObject("PlayerPanel");
        playerPanel.transform.SetParent(parentPanel, false);
        playerPanel.name = player.name;

        // Add RectTransform component and set its size
        RectTransform panelRectTransform = playerPanel.AddComponent<RectTransform>();
        VerticalLayoutGroup layoutGroup = playerPanel.AddComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = 10f;

        panelRectTransform.sizeDelta = new Vector2(300, 400); // Set panel size as needed

        //// Replace HorizontalLayoutGroup with GridLayoutGroup for button grid layout
        //GridLayoutGroup gridLayoutGroup = playerPanel.AddComponent<GridLayoutGroup>();
        //gridLayoutGroup.cellSize = new Vector2(70, 70);   // Size of each button
        //gridLayoutGroup.spacing = new Vector2(40, 40);     // Spacing between buttons
        //gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        //gridLayoutGroup.constraintCount = 3;               // Limit to 3 buttons per row

        // Get the available actions from the player
        List<ImprovedActionStat> playerAvailableAction = player.GetAvailableActions();
        foreach (ImprovedActionStat scriptable in playerAvailableAction)
        {
            // Instantiate each action button and set its function based on its name
            GameObject button = Instantiate(scriptable.actionButton, playerPanel.transform);
            

            if (scriptable.actionButton.name == "Block")
            {
                button.GetComponent<Button>().onClick.AddListener(() => ActionArchive.instance.Block());
            }
            else if (scriptable.actionButton.name == "Counter")
            {
                button.GetComponent<Button>().onClick.AddListener(() => ActionArchive.instance.Counter());
            }
            else if (scriptable.actionButton.name == "VenomCloud")
            {
                button.GetComponent<Button>().onClick.AddListener(() => ActionArchive.instance.VenomCloud());
            }
            else if (scriptable.actionButton.name == "SmokeCloud")
            {
                button.GetComponent<Button>().onClick.AddListener(() => ActionArchive.instance.SmokeCloud());
            }
            else if (scriptable.actionButton.name == "BoneShield")
            {
                button.GetComponent<Button>().onClick.AddListener(() => ActionArchive.instance.BoneShield());
            }
            else if (scriptable.actionButton.name == "Imbuement")
            {
                button.GetComponent<Button>().onClick.AddListener(() => ActionArchive.instance.Imbuement());
            } 
            else if (scriptable.actionButton.name == "DaggerSweep")
            {
                button.GetComponent<Button>().onClick.AddListener(() => ActionArchive.instance.DaggerSweep());
            }
            else if (scriptable.actionButton.name == "Impale")
            {
                button.GetComponent<Button>().onClick.AddListener(() => ActionArchive.instance.Impale());
            }
            else
            {
                button.GetComponent<Button>().onClick.AddListener(() => TempManager.instance.ShowTargetList(scriptable.actionButton.name));
                
            }

            // Add button to ActionActivator
            ActionActivator.instance.AddToActionButtons(button);
        }

        // Add additional buttons for special actions like Warp, Dash, GroundBlast
        if (player.GetWarpAction())
        {
            GameObject warpButton = Instantiate(player.GetWarpAction().actionButton, playerPanel.transform);
            warpButton.GetComponent<Button>().onClick.AddListener(() => ActionArchive.instance.WarpSurge());
            ActionActivator.instance.AddToActionButtons(warpButton);
        }

        if (player.GetDashAction())
        {
            GameObject dashButton = Instantiate(player.GetDashAction().actionButton, playerPanel.transform);
            dashButton.GetComponent<Button>().onClick.AddListener(() => ActionArchive.instance.Dash());
            ActionActivator.instance.AddToActionButtons(dashButton);
        }

        if (player.GetGroundBlastAction())
        {
            GameObject groundBlastButton = Instantiate(player.GetGroundBlastAction().actionButton, playerPanel.transform);
            groundBlastButton.GetComponent<Button>().onClick.AddListener(() => ActionArchive.instance.GroundBlast());
            ActionActivator.instance.AddToActionButtons(groundBlastButton);
        }
        if (player.GetMoveAction())
        {
            GameObject moveButton = Instantiate(player.GetMoveAction().actionButton, playerPanel.transform);
            moveButton.GetComponent<Button>().onClick.AddListener(() => ActionArchive.instance.Move());
            ActionActivator.instance.AddToActionButtons(moveButton);
        }

        // Add the undo button
        //GameObject undoButton = Instantiate(undoButtonPrefab, playerPanel.transform);
        //undoButton.GetComponent<Button>().onClick.AddListener(() => ButtonStackManager.instance.UndoStackEntry());
        //ActionActivator.instance.AddToActionButtons(undoButton);

        // Add the ultimate action button
        GameObject ultimateButton = Instantiate(player.GetUltimateScripitable().ultimateButton, playerPanel.transform);
        if (player.GetPlayerUltimate().IsSingleTarget())
        {
            ultimateButton.GetComponent<Button>().onClick.AddListener(() => TurnManager.instance.UltimateTargetList(player.GetUltimateScripitable()));    
           // ultimateButton.GetComponent<Button>().onClick.AddListener(() => ActionArchive.instance.Ultimate());
        }
        else
        {
            ultimateButton.GetComponent<Button>().onClick.AddListener(() => ActionArchive.instance.Ultimate());
        }
        ActionActivator.instance.AddToActionButtons(ultimateButton);


        // Add the move action button
        //GameObject moveButton = Instantiate(moveButtonPrefab, playerPanel.transform);
        //moveButton.GetComponent<Button>().onClick.AddListener(() => ActionArchive.instance.Move());
        //ActionActivator.instance.AddToActionButtons(moveButton);

        return playerPanel;
    }


    

    public void AddCommonListeners()
    {

    }
}