using Cysharp.Threading.Tasks;
using UnityEngine;

public class GridInteract : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject TargetEnemy;
    public async UniTask GridInterracted(GameObject p)
    {
        if (!GridSystem.instance.IsGridOn)
        {
            ImprovedActionStat meleeScriptable = DAOScriptableObject.instance.GetImprovedActionData(StringData.directory, "Assassinate");
            p.GetComponent<SpawnVFX>().SetTargetAnimator(TargetEnemy);
            p.GetComponent<SpawnVFX>().SetTargetAnimation(meleeScriptable.TargetHurtAnimation);
            p.GetComponent<SpawnVFX>().SetTargetVFXPosition(TargetEnemy.GetComponent<VFXSpawnPosition>().CharacterBodyPosition[meleeScriptable.TargetCharacterBodyLocation]);
            p.GetComponent<SpawnVFX>().SetParticle(meleeScriptable.particle);
            await CutsceneManager.instance.PlayAnimationForCharacter(p, "Assassinate");
           
            WaveManager.instance.GridStartAssasinate(TargetEnemy);

            TargetEnemy.GetComponent<TemporaryStats>().CurrentHealth = HealthManager.instance.HealthCalculation(1000, TargetEnemy.GetComponent<TemporaryStats>().CurrentHealth);
            UI.instance.ShowFlyingText((1000 * -1).ToString(), TargetEnemy.GetComponent<TemporaryStats>().FlyingTextParent, Color.red);
            await HealthManager.instance.PlayerMortality(TargetEnemy.GetComponent<TemporaryStats>(), p.GetComponent<TemporaryStats>());
        }

    }
    public void Interact(GameObject player)
    {
        GridInterracted(player);
    }
    public bool IsGridTrigger()
    {
        return true;
    }
}
