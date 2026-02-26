using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class ActionActivator : MonoBehaviour
{
    public static ActionActivator instance;
    
    public List<GameObject> actionButtons = new List<GameObject>();
   
    [SerializeField]
    Color activateColor;
    [SerializeField]
    Color deactivateColor;
    
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
        actionButtons.Clear();
    }

    public void UpdateAvailableAction(CharacterBaseClasses playerCh, TemporaryStats playerTemp)
    {
        

        foreach (var button in actionButtons)
        {
            ButtonName buttonNameComponent;

            if (button.TryGetComponent(out buttonNameComponent))
            {
                string buttonActionName = buttonNameComponent.GetButtonActionName();
             

                if (buttonActionName!="Ultimate")
                {
                    if (DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, buttonActionName)?.APCost <= playerTemp.CurrentAP  || buttonActionName == "Move" || buttonActionName == "WarpSurge" || (DAOScriptableObject.instance.GetActionData(StringData.directory, buttonActionName)?.APCost <= playerTemp.CurrentAP ))
                    {
                        if (buttonActionName == "PushBack")
                        {
                            
                        }
                        EnableButton(button);
                        
                    }
                    else
                    {
                        DisableButton(button);
                    }
                }
                else
                {
                    if (UltimateSystem._instance.checkUltimateAvailability(playerCh,playerTemp))
                    {
                        
                        EnableButton(button);
                        
                    }
                    else
                    {
                        DisableButton(button);
                    }
                }

                
            }           
        }
    }

    void EnableButton(GameObject buttonGameObject)
    {
      
     

        buttonGameObject.GetComponent<UnityEngine.UI.Button>().interactable = true;
        buttonGameObject.GetComponent<UnityEngine.UI.Image>().color = activateColor;
        
        //buttonGameObject.GetComponent<ButtonHoverAnimation>().enabled = true;
        //buttonGameObject.GetComponent<ButtonHoverAnimation>().GetHoverText().SetActive(true);
    }
    void DisableButton(GameObject buttonGameObject)
    {
        
        buttonGameObject.GetComponent<UnityEngine.UI.Button>().interactable = false;
        buttonGameObject.GetComponent<UnityEngine.UI.Image>().color = deactivateColor;
        //buttonGameObject.GetComponent<ButtonHoverAnimation>().GetHoverText().SetActive(false); 
        //buttonGameObject.GetComponent<ButtonHoverAnimation>().enabled = false;
    }

    public void AddToActionButtons(GameObject button)
    {
        actionButtons.Add(button);

        //NumpadHotkeys.instance.actionButtons.Add(button);
    }
}
