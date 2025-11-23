using UnityEngine;

public class CollectBarrelObjective : MonoBehaviour
{
    void Start()
    {   
        // Create objectives
        Objective collectBarrel = new Objective
        {
            ObjectiveID = "OBJ_1",
            Title = "Collect 2 Barrel",
            ProgressGetter = () => SwitchMC.Instance.mainCharacter.GetComponent<TemporaryStats>().CurrentResolve / 2f,
            Condition = () => SwitchMC.Instance.mainCharacter.GetComponent<TemporaryStats>().CurrentResolve >= 2
        };

   /*     Objective buildFire = new Objective
        {
            ObjectiveID = "OBJ_2",
            Title = "Build a Campfire",
            RequiredObjectives = new Objective[] { collectWood },  // must finish OBJ_1 before OBJ_2
            Condition = () => CampfireSystem.FireBuilt
        };
*/
        // Add them to the manager
        ObjectiveManager.Instance.Objectives.Add(collectBarrel);
        //ObjectiveManager.Instance.Objectives.Add(buildFire);

        // Activate first objective manually (or manager can activate automatically)
        collectBarrel.SetState(ObjectiveState.Active);

    }


}
