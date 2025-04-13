using Cinemachine;
using UnityEngine;

public class Cinemachine_CameraShake : MonoBehaviour
{
    public CinemachineImpulseSource impulseSource;


    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain != null)
        {
            CinemachineVirtualCameraBase activeCam = (CinemachineVirtualCameraBase)brain.ActiveVirtualCamera;
        }
        else
        {
            Debug.LogWarning("No CinemachineBrain found on the main camera. Did you forget to attach it, you chaotic artisan?");
        }

        if(Input.GetKeyDown(KeyCode.V))
        {
            RumbleImpulse();
        } 
        if(Input.GetKeyDown(KeyCode.B))
        {
            ExplotionImpulse();
        }
    }

    private void RumbleImpulse()
    {
        var definition = new CinemachineImpulseDefinition
        {
            m_ImpulseType = CinemachineImpulseDefinition.ImpulseTypes.Uniform,
            m_ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Rumble,
        };

        impulseSource.m_ImpulseDefinition = definition;
        impulseSource.GenerateImpulseWithVelocity(Vector3.right);
    }
    private void ExplotionImpulse()
    {
        var definition = new CinemachineImpulseDefinition
        {
            //call event
            m_ImpulseType = CinemachineImpulseDefinition.ImpulseTypes.Uniform,
            m_ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Explosion,
        };

        impulseSource.m_ImpulseDefinition = definition;
        impulseSource.GenerateImpulse();
    }
}
