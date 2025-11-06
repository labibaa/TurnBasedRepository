using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformerClass : CharacterBaseClasses
{
    public override void LevelUp()
    {
        HealthPoints += 10 + (Endurance * 2);
    }
}
