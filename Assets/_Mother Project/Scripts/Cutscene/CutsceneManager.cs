using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;
using Cysharp.Threading.Tasks;
using System;

public class CutsceneManager : MonoBehaviour
{

    public static CutsceneManager instance;

    public Animator[] animators;// Array of Animator components
    private Animator charAnimator;
    private Animator ghostAnimator;
    public PlayableDirector timelineDirector;
    public CinemachineVirtualCamera virtualCamera;
    public CinemachineTargetGroup targetGroup;
    public float cameraFollowSpeed;
    public Vector3 followOffset = Vector3.zero;
    public float cameraDamping = 1f;

    

    // to be deleted  

    [SerializeField] private GameObject character;
    [SerializeField] private string animName;

     bool particleFinishedPlaying = false;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        ThrowVFX.hitTrigger += IsParticleFinishedPlaying;
        CurveThrowVFX.hitTrigger += IsParticleFinishedPlaying;
        SpawnVFX.hitAnimation+= IsParticleFinishedPlaying;
    }

    private void OnDisable()
    {
        ThrowVFX.hitTrigger -= IsParticleFinishedPlaying;
        CurveThrowVFX.hitTrigger -= IsParticleFinishedPlaying;
        SpawnVFX.hitAnimation -= IsParticleFinishedPlaying;
    }



    void IsParticleFinishedPlaying()
    {
        particleFinishedPlaying = true;
    }

    // Play the specified animation on the animator of the given character
    public async UniTask PlayAnimationForCharacter(GameObject character, string animationName)
    {
        
         charAnimator = character.GetComponent<Animator>();
         charAnimator.Play(animationName);

       
        character.GetComponent<SpawnVFX>().ActionCameraActivate();
        // Wait until the animation starts playing
        await UniTask.WaitUntil(() =>
            charAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationName) &&
            charAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0f
        );
        Debug.Log($"Animation '{animationName}' is playing.");
        // Wait until animation finishes playing
        await UniTask.WaitUntil(() =>
            !charAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationName)
        );
        character.GetComponent<SpawnVFX>().ActionCameraDeactivate();
        Debug.Log($"Animation '{animationName}' has finished playing.");

        //await UniTask.WaitWhile(() => !particleFinishedPlaying);
        //particleFinishedPlaying = false;

    }

    public async UniTask PlayAnimationForGhost(GameObject character, string animationName, GameObject enemy)
    {


        Quaternion playerRotation = Quaternion.LookRotation(enemy.transform.position - character.transform.position, Vector3.up);
        Vector3 playerEulerRotation = playerRotation.eulerAngles;
        playerEulerRotation.x = 0; // Locking rotation around x-axis
        playerEulerRotation.z = 0; // Locking rotation around z-axis
        playerRotation = Quaternion.Euler(playerEulerRotation);

        character.transform.rotation = playerRotation;
        ghostAnimator = character.GetComponent<Animator>();
        ghostAnimator.Play(animationName);

        if (ghostAnimator)
        {
            // Wait until the animation starts playing
            await UniTask.WaitUntil(() =>
                ghostAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationName) &&
                ghostAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0f
            );
            Debug.Log($"Ghost Animation '{animationName}' is playing.");
        }

    }


}
