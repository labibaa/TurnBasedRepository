using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ButtonStackManager : MonoBehaviour
{
    public static ButtonStackManager instance;
    public GameObject imagePrefab; // Reference your image prefab
    public Transform stackPanel; // Reference your UI panel with Vertical Layout Group
    [SerializeField]
    Transform parentPanel;
    [SerializeField]
    Transform itemParentPanel;
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

    // Maps action button names to their specific ActionArchive methods.
    // Actions not in this map fall through to the default ShowTargetList behavior.
    private Dictionary<string, Action> actionMap;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (stackPanel == null || stackPanel.GetComponent<VerticalLayoutGroup>() == null)
        {
            Debug.LogError("The stackPanel is missing or doesn't have a Vertical Layout Group component.");
            return;
        }

        actionMap = new Dictionary<string, Action>
        {
            { "Block",      () => ActionArchive.instance.Block() },
            { "Counter",    () => ActionArchive.instance.Counter() },
            { "VenomCloud", () => ActionArchive.instance.VenomCloud() },
            { "SmokeCloud", () => ActionArchive.instance.SmokeCloud() },
            { "BoneShield", () => ActionArchive.instance.BoneShield() },
            { "Imbuement",  () => ActionArchive.instance.Imbuement() },
            { "DaggerSweep",() => ActionArchive.instance.DaggerSweep() },
            { "Impale",     () => ActionArchive.instance.Impale() },
        };
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
        UltimateUI ultUI = ultimateBarSpawned.GetComponent<UltimateUI>();
        ultUI.maxProgress = player.GetPlayerUltimate().GetultimateThreshold();
        ultUI.ultimateBarProgress = 0;
        return ultimateBarSpawned;
    }
    private GameObject CreatePanel(string panelName, Transform parent, CharacterBaseClasses player, float spacing)
    {
        GameObject panel = new GameObject(panelName);
        panel.transform.SetParent(parent, false);
        panel.name = player.name;

        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(400, 600);
        panelRect.anchorMin = new Vector2(1f, 0f); // bottom-right
        panelRect.anchorMax = new Vector2(1f, 0f); // bottom-right
        panelRect.pivot = new Vector2(1f, 0f); // pivot bottom-right
        panelRect.anchoredPosition = new Vector2(-10f, 10f); // offset inward

        VerticalLayoutGroup layoutGroup = panel.AddComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = spacing;
        layoutGroup.childControlWidth = false;
        layoutGroup.childControlHeight = false;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childAlignment = TextAnchor.MiddleRight;

        return panel;
    }

    public GameObject PopulateItemPanel(CharacterBaseClasses player)
    {
        GameObject playerItems = CreatePanel("PlayerItems", itemParentPanel, player, 10f);

        List<InventoryItem> playerAvailableItems = player.GetAvailableItems();
        foreach (InventoryItem item in playerAvailableItems)
        {
            GameObject button = Instantiate(item.itemClass.itemButton, playerItems.transform);
            button.GetComponent<ButtonName>().SetButtonName(item.itemClass.itemName);

            TextMeshProUGUI nameComponent = button.GetComponentInChildren<TextMeshProUGUI>();
            if (nameComponent != null)
            {
                nameComponent.text = item.itemClass.itemName;
            }
        }

        return playerItems;
    }

    public GameObject PopulateActionPanel(CharacterBaseClasses player)
    {
        GameObject playerPanel = CreatePanel("PlayerPanel", parentPanel, player, 8f);

        List<ImprovedActionStat> playerAvailableAction = player.GetAvailableActions();
        foreach (ImprovedActionStat scriptable in playerAvailableAction)
        {
            GameObject button = Instantiate(scriptable.actionButton, playerPanel.transform);
            string buttonName = scriptable.actionButton.name;

            if (actionMap.TryGetValue(buttonName, out Action action))
            {
                button.GetComponent<Button>().onClick.AddListener(() => action());
            }
            else
            {
                button.GetComponent<Button>().onClick.AddListener(() => TempManager.instance.ShowTargetList(buttonName));
            }

            ActionActivator.instance.AddToActionButtons(button);
        }

        AddSpecialActionButton(player.GetWarpAction(), playerPanel, () => ActionArchive.instance.WarpSurge());
        AddSpecialActionButton(player.GetDashAction(), playerPanel, () => ActionArchive.instance.Dash());
        AddSpecialActionButton(player.GetGroundBlastAction(), playerPanel, () => ActionArchive.instance.GroundBlast());
        AddSpecialActionButton(player.GetMoveAction(), playerPanel, () => ActionArchive.instance.Move());

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

        return playerPanel;
    }

    private void AddSpecialActionButton(ActionStat actionStat, GameObject panel, Action callback)
    {
        if (actionStat == null) return;

        GameObject button = Instantiate(actionStat.actionButton, panel.transform);
        button.GetComponent<Button>().onClick.AddListener(() => callback());
        ActionActivator.instance.AddToActionButtons(button);
    }
}