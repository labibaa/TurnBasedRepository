using Cysharp.Threading.Tasks;
using UnityEngine;

public class SkeletonGrabDOTHandler : BaseDOThandler
{
    private ImprovedActionStat skeletonGrabIAS;

    public void SetSkeletonGrabIAS(ImprovedActionStat stat)
    {
        skeletonGrabIAS = stat;
    }

    protected override async UniTask OnEffectTick()
    {
        if (Target != null)
        {
            Target.playerVisiblity = 0;
            Target.GetComponent<PlayerTurn>().isMoveOn = false;

            if (skeletonGrabIAS != null)
            {
                await CutsceneManager.instance.PlayAnimationForCharacter(
                    Target.gameObject,
                    skeletonGrabIAS.TargetHurtAnimation
                );
            }
        }
    }
}
