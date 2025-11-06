using UnityEngine;
using Cinemachine;

public class CameraSwitcherNumpad : MonoBehaviour
{
    public CinemachineVirtualCamera cam1;
    public CinemachineVirtualCamera cam2;
    public CinemachineVirtualCamera cam3;
    public CinemachineVirtualCamera cam4;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            SwitchCamera(cam1);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            SwitchCamera(cam2);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            SwitchCamera(cam3);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            SwitchCamera(cam4);
        }
    }

    private void SwitchCamera(CinemachineVirtualCamera activeCamera)
    {
        cam1.Priority = cam1 == activeCamera ? 17 : 0;
        cam2.Priority = cam2 == activeCamera ? 17 : 0;
        cam3.Priority = cam3 == activeCamera ? 17 : 0;
        cam4.Priority = cam4 == activeCamera ? 17 : 0;
    }
}
