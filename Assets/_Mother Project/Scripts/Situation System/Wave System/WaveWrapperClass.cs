using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class WaveWrapperClass
{
    public int WaveID;
    public List<PlayerTurn> CharactersOfTheWave;
    public GameObject GridStartLocation;
    public PlayableDirector WaveTimeline;
    public GameObject Trigger;
    public int rewardExp;
}
