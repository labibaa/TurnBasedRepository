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


    public async UniTask WaitUntilAnimationFinished(string animationName,Animator animator)
    {
        // Ensure the animator is not null and has the specified animation.
        if (animator != null && animator.HasState(0, Animator.StringToHash(animationName)))
        {
            // Wait until the animation is no longer playing.
            while (animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            {
                await UniTask.Yield();
            }
            
        }
        else
        {
            Debug.LogWarning("Animator or animation not found.");
        }
    }
    // Play the specified animation on the animator of the given character
    public async UniTask PlayAnimationForCharacter(GameObject character, string animationName)
    {
        
         charAnimator = character.GetComponent<Animator>();
         charAnimator.Play(animationName);

        
        //CameraShakeOnDamage.Instance.ShakeCameraOnDamage();
        // await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        //Debug.Log("ng");
        
        Debug.Log($"pppop '{animationName}' has finished playing.");
        await UniTask.WaitWhile(() => charAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);

       // CameraShakeOnDamage.Instance.StopCameraShake();

        //await UniTask.WaitWhile(() => !particleFinishedPlaying);
        //particleFinishedPlaying = false;

        //virtualCamera.Priority = 9;
        // Animation has finished playing

        

      
    }

    async UniTask WaitForAnimationToFinish()
    {
        while (charAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            await UniTask.Yield(); // Use UniTask.Yield to yield control to the main thread
        }
    }


    public async UniTask PlayAnimationForGhost(GameObject character, string animationName, GameObject enemy)
    {

        //Vector3 directionToTarget = enemy.transform.position - character.transform.position;
        //directionToTarget.y = 0; // Zeroing out the y-component to prevent tilting up or down

        //Quaternion targetRotation = Quaternion.LookRotation(-directionToTarget);
        //Vector3 eulerRotation = targetRotation.eulerAngles;
        //eulerRotation.x = 0; // Locking rotation around x-axis
        //eulerRotation.z = 0; // Locking rotation around z-axis
        //targetRotation = Quaternion.Euler(eulerRotation);

        //enemy.transform.rotation = targetRotation;

        // Rotate the player


        Quaternion playerRotation = Quaternion.LookRotation(enemy.transform.position - character.transform.position, Vector3.up);
        Vector3 playerEulerRotation = playerRotation.eulerAngles;
        playerEulerRotation.x = 0; // Locking rotation around x-axis
        playerEulerRotation.z = 0; // Locking rotation around z-axis
        playerRotation = Quaternion.Euler(playerEulerRotation);

        character.transform.rotation = playerRotation;



        
        ghostAnimator = character.GetComponent<Animator>();
        ghostAnimator.Play(animationName);
        


        //await WaitForAnimationToFinish();
        // CameraShakeOnDamage.Instance.ShakeCameraOnDamage();
        // await UniTask.Delay(TimeSpan.FromSeconds(0.1f));

        // character.GetComponent<ArrowSpawner>().OnFinishParticle += IsParticleFinishedPlaying;
        // await UniTask.WaitWhile(() => !particleFinishedPlaying);
        // CameraShakeOnDamage.Instance.StopCameraShake();
        // character.GetComponent<ArrowSpawner>().OnFinishParticle -= IsParticleFinishedPlaying;
        // particleFinishedPlaying = false;
        await UniTask.WaitWhile(() => ghostAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);



        // Animation has finished playing
        Debug.Log($"Animation '{animationName}' has finished playing.");



    }






    public void PlayCameraAnimation(GameObject target)
    {
        if (timelineDirector != null)
        {
            // Play the Timeline
            timelineDirector.Play();

            // Update the virtual camera's follow target and settings
            if (virtualCamera != null)
            {
                //to move camera while game is paused
                CinemachineImpulseManager.Instance.IgnoreTimeScale = true;
                Camera.main.GetComponent<CinemachineBrain>().m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
                Camera.main.GetComponent<CinemachineBrain>().m_IgnoreTimeScale = true;
                //to move camera while game is paused


                CinemachineVirtualCamera virtualCameraComponent = virtualCamera.GetComponent<CinemachineVirtualCamera>();
                virtualCameraComponent.Priority = 15;
                virtualCameraComponent.Follow = target.transform;
                virtualCameraComponent.LookAt = target.transform;

                // Get the Cinemachine transposer component
                CinemachineTransposer transposer = virtualCameraComponent.GetCinemachineComponent<CinemachineTransposer>();

                // Set the follow offset
                transposer.m_FollowOffset = followOffset;

                // Set the camera damping
                transposer.m_XDamping = cameraDamping;
                transposer.m_YDamping = cameraDamping;
                transposer.m_ZDamping = cameraDamping;
                transposer.m_YawDamping = cameraDamping;
            }
        }
    }

    public void PlayCinemachine(GameObject target)
    {

        //to move camera while game is paused
        CinemachineImpulseManager.Instance.IgnoreTimeScale = true;
        Camera.main.GetComponent<CinemachineBrain>().m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
        Camera.main.GetComponent<CinemachineBrain>().m_IgnoreTimeScale = true;
        //to move camera while game is paused


        CinemachineVirtualCamera virtualCameraComponent = virtualCamera.GetComponent<CinemachineVirtualCamera>();
        virtualCameraComponent.Priority = 15;
        //  virtualCameraComponent.m_Lens.FieldOfView = 20f;
        virtualCameraComponent.Follow = target.transform;
        virtualCameraComponent.LookAt = target.transform;


    }

    public void PlayCameraPriorityReset()
    {
        CinemachineVirtualCamera virtualCameraComponent = virtualCamera.GetComponent<CinemachineVirtualCamera>();
        virtualCameraComponent.Priority = 9;
    }
}
