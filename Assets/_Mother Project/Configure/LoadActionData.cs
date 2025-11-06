using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadActionData : MonoBehaviour
{
    [SerializeField] Transform actionPanel;
    [SerializeField] Button actionButtonPrefab;
    [SerializeField] Transform actionDetailsPanel;
    [SerializeField] GameObject actionDataInputField_txt;
    [SerializeField] public GameObject ActionPanel;
    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadActions()
    {
        ActionPanel.SetActive(true);
        ImprovedActionStat[] improvedActionStats = Resources.LoadAll<ImprovedActionStat>("ActionMoves");
        actionPanel.gameObject.SetActive(true);
        foreach (Transform child in actionPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (ImprovedActionStat imp in improvedActionStats)
        {
            Button actionButton = Instantiate(actionButtonPrefab,actionPanel);
            actionButton.GetComponentInChildren<TextMeshProUGUI>().text = imp.name;
            actionButton.onClick.AddListener(() => LoadFromActionData(imp));

        }
    }

    public void LoadFromActionData(ImprovedActionStat action)
    {
        foreach (Transform child in actionDetailsPanel)
        {
            Destroy(child.gameObject);
        }
        CreateActionInputField(actionDataInputField_txt.gameObject, "Action Name", action.ActionName, (value) => action.ActionName = value);
        CreateActionInputField(actionDataInputField_txt.gameObject, "Action Description", action.Description, (value) => action.Description = value);
        CreateActionInputField(actionDataInputField_txt.gameObject, "Action Range" , action.ActionRange.ToString(), (value) => action.ActionRange = int.Parse(value));
        CreateActionInputField(actionDataInputField_txt.gameObject, "Action Accuracy", action.ActionAccuracy.ToString(), (value) => action.ActionAccuracy = int.Parse (value));
        CreateActionInputField(actionDataInputField_txt.gameObject, "Action Point(AP)", action.APCost.ToString(), (value) => action.APCost = int.Parse (value));
       // CreateActionInputField(actionDataInputField_txt.gameObject, "Action Stance", action.actionStance, (value) => action.actionStance = value);
       // CreateActionInputField(actionDataInputField_txt.gameObject, "Action Type", action.APCost.ToString(), (value) => action.APCost = int.Parse (value));
    }

    void CreateActionInputField(GameObject prefab, string label, string defaultValue, System.Action<string> onValueChanged)
    {
        // Instantiate the InputField prefab
        GameObject inputFieldGO = Instantiate(prefab, actionDetailsPanel);
        TMP_InputField inputField = inputFieldGO.transform.Find("Action InputField (TMP)").GetComponent<TMP_InputField>();
        TextMeshProUGUI labelComponent = inputFieldGO.transform.Find("Label").GetComponent<TextMeshProUGUI>();
        if (labelComponent != null)
        {
            labelComponent.text = label;
        }
        // Set the default value
        inputField.text = defaultValue + "\n";

        // Add listener for when the value is changed
        inputField.onValueChanged.AddListener((value) => {

            onValueChanged(value);


        });

    }

}
