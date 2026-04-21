using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject detailsPrefab;

    ImprovedActionStat actionScriptable;
    [SerializeField]
    string actionName;
    string directory = "ActionMoves/";
    [SerializeField]
    TMP_Text actionNameText;
    [SerializeField]
    TMP_Text apCost;
    [SerializeField]
    TMP_Text range;

    private void Start()
    {
        actionScriptable = DAOScriptableObject.instance.GetImprovedActionData(directory, actionName);

        if (actionScriptable != null)
        {
            actionNameText.text = actionScriptable.ActionName;
            apCost.text = "Ap Cost : " + actionScriptable.APCost;
            range.text = "Range : " + actionScriptable.ActionRange;
        }





    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (actionName == "Move")
        {
            ShowMoveRange();
            return;

        }

        if (detailsPrefab != null && actionScriptable != null)
        {
            actionNameText.text = actionScriptable.ActionName;
            apCost.text = "Ap Cost : " + actionScriptable.APCost;
            range.text = "Range : " + actionScriptable.ActionRange;
            detailsPrefab.SetActive(true);
            ShowActionRange();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (actionScriptable != null || actionName == "Move")
        {
            detailsPrefab.SetActive(false);
            HideActionRange();
        }
    }

    void ShowActionRange()
    {
        TemporaryStats attackerStats = TempManager.instance.attacker.GetComponent<TemporaryStats>();
        GridMovement.instance.InAdjacentMatrix(attackerStats.currentPlayerGridPosition, attackerStats.CharacterTeam, actionScriptable.ActionRange, Color.red);
    }

    void ShowMoveRange()
    {
        TemporaryStats attackerStats = TempManager.instance.attacker.GetComponent<TemporaryStats>();
        GridMovement.instance.InAdjacentMatrix(attackerStats.currentPlayerGridPosition, attackerStats.CharacterTeam, attackerStats.CurrentDex, Color.red);
    }

    void HideActionRange()
    {
        GridMovement.instance.ResetHighlightedPath();
    }

}
