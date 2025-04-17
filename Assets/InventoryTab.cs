using UnityEngine;
using UnityEngine.UI;

public class InventoryTabManager : MonoBehaviour
{
    public RectTransform characterTab;
    public RectTransform skillsTab;
    public RectTransform consumableTab;

    private RectTransform currentBottomTab;

    void Start()
    {
        // Assuming Consumable is initially in the bottom
        currentBottomTab = consumableTab;
    }

    public void OnCharacterTabPressed()
    {
        if (currentBottomTab != characterTab)
            SwapTabs(characterTab);
    }

    public void OnSkillsTabPressed()
    {
        if (currentBottomTab != skillsTab)
            SwapTabs(skillsTab);
    }

    public void OnConsumableTabPressed()
    {
        if (currentBottomTab != consumableTab)
            SwapTabs(consumableTab);
    }

    private void SwapTabs(RectTransform selectedTab)
    {
        // Swap the anchored positions
        Vector2 tempPos = selectedTab.anchoredPosition;
        selectedTab.anchoredPosition = currentBottomTab.anchoredPosition;
        currentBottomTab.anchoredPosition = tempPos;

        // Update the current bottom tab
        currentBottomTab = selectedTab;
    }
}
