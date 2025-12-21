using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasterClass : CharacterBaseClasses
{
    public override void ArcanaLevelUp()
    {
    }

    public override void DexterityLevelUp()
    {

    }

    public override void EnduranceLevelUp()
    {
     
    }

    public override void HealthLevelUp()
    {
       
    }

    public override void IntelligenceLevelUp()
    {
       
    }
    public override void StregthLevelUp()
    {
       
    }
    public override void LevelUp()
    {
        // Increase the character's attributes based on a predetermined formula.
        Strength = (int)(Strength + (Level * 1.5f) + (Level * Level * 0.01)); // 1st value growth factor. 2nd value scaling factor
        Debug.Log(Strength);
        Dexterity = (int)(Dexterity + (Level * 1.02f) + (Level * Level * 0.1));
        Intelligence = (int)(Intelligence + (Level * 1.5f) + (Level * Level * 0.1));
        Arcana = (int)(Arcana + (Level * 1.5f) + (Level * Level * 0.1));
        Endurance = (int)(Endurance + (Level * 1.5f) + (Level * Level * 0.02));
        DamageMultiplier = (int)(DamageMultiplier + (Level * 0.2f) + (Level * Level * 0.01));
        MaxExperiencePoint += 100;
        // Increase the character's maximum health based on a predetermined formula.
        HealthPoints = (int)(HealthPoints + (Level * 1.5f) + (Level * Level * 0.1));

        // Increase the character's level by 1.
        Level++;
        // Add any additional logic or side effects as needed.
    }

  
}
