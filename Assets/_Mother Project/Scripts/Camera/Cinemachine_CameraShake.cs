using Cinemachine;
using System;
using UnityEngine;

public class Cinemachine_CameraShake : MonoBehaviour
{

    public CinemachineImpulseSource impulseSource;

    private void OnEnable()
    {
        SpawnVFX.OnRumbleShake += RumbleImpulse;
        SpawnVFX.OnExplotionShake += ExplotionImpulse;
    }

    private void OnDisable()
    {
        SpawnVFX.OnRumbleShake -= RumbleImpulse;
        SpawnVFX.OnExplotionShake -= ExplotionImpulse;
    }
    void Update()
    {
 /*       CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain != null)
        {
            CinemachineVirtualCameraBase activeCam = (CinemachineVirtualCameraBase)brain.ActiveVirtualCamera;
        }
        else
        {
            Debug.LogWarning("No CinemachineBrain found on the main camera. Did you forget to attach it, you chaotic artisan?");
        }*/

        if(Input.GetKeyDown(KeyCode.V))
        {
            SwitchMC.Instance.mainCharacter.GetComponent<CharacterBaseClasses>().LevelUp();
        } 

    }

    private void RumbleImpulse()
    {
        var definition = new CinemachineImpulseDefinition
        {
            m_ImpulseDuration = 0.5f,
            m_ImpulseType = CinemachineImpulseDefinition.ImpulseTypes.Uniform,
            m_ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Rumble
        };

        impulseSource.m_ImpulseDefinition = definition;
        impulseSource.GenerateImpulseWithVelocity(Vector3.right);
    }
    private void ExplotionImpulse()
    {
        var definition = new CinemachineImpulseDefinition
        {
            m_ImpulseDuration = 0.5f,
            m_ImpulseType = CinemachineImpulseDefinition.ImpulseTypes.Uniform,
            m_ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Explosion
        };

        impulseSource.m_ImpulseDefinition = definition;
        impulseSource.GenerateImpulse();
    }
}
