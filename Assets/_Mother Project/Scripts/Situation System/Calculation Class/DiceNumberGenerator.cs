using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DiceNumberGenerator : MonoBehaviour
{
    public static DiceNumberGenerator instance;
    int cluster1Lower = 1;
    int cluster1Upper = 6;
    int cluster2Lower = 6;
    int cluster2Upper = 16;
    int cluster3Lower = 16;
    int cluster3Upper = 21;
    int randomProbability;
    [SerializeField]
    TMP_Text diceValue;


    private void Awake()
    {
        if (instance== null)
        {
            instance = this;
        }
    }
    public int GetDiceValue(int probCluster1, int probCluster2, int probCluster3) //dice value for damage calculation of actions
    {
        // float rand = Random.value;


         randomProbability = Random.Range(1, 101);

        if (randomProbability <= probCluster1)
        {
            return Random.Range(cluster1Lower, cluster1Upper);
        }
        else if (randomProbability<= probCluster1+probCluster2)
        {
            return Random.Range(cluster2Lower, cluster2Upper);
        }

        else 
        {
            return Random.Range(cluster3Lower, cluster3Upper);
        }

    }

   public void DisplayDiceRoll()
    {
        diceValue.text = GetDiceValue(20,60,20).ToString();
    }

}
