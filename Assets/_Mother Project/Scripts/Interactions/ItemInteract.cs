using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemInteract : MonoBehaviour, IInteractable
{
    
    public async UniTask itemInterracted(GameObject p)
    {
        Debug.Log("Box says bye");
        Objective currentObjective =  ObjectiveManager.Instance.GetObjective("OBJ_1");
        SwitchMC.Instance.mainCharacter.GetComponent<TemporaryStats>().CurrentResolve++; //wip for now
        ObjectiveManager.Instance.TriggerEvaluation(currentObjective);
        await CutsceneManager.instance.PlayAnimationForCharacter(p, "Fall on back");
        Destroy(this.gameObject);
        if(currentObjective.State == ObjectiveState.Completed)
        {
            Vector2 deadPlayerGridPosition = GridSystem.instance.WorldToGrid(transform.position);
            GridSystem.instance._gridArray[(int)deadPlayerGridPosition.x, (int)deadPlayerGridPosition.y].GetComponent<GridStat>().ClearGrid();
            HealthManager.instance.GridStop(); // give condition to stop grid
        }
      
    }

    public void Interact(GameObject p)
    {
        itemInterracted(p);

    }
    public bool IsGridTrigger()
    {
        return false;
    }
}
