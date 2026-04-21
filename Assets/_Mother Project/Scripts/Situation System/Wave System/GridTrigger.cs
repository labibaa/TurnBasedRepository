using UnityEngine;

public class GridTrigger : MonoBehaviour
{
    public int WaveTriggerID;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !GridSystem.instance.IsGridOn)
        {
            TempManager.instance.IsObjective = gameObject.CompareTag("ObjectiveGrid");
            WaveManager.instance.IniCurrentWave(gameObject);
            GridSystem.instance.GenerateGridOnButton();
        }
    }
}
