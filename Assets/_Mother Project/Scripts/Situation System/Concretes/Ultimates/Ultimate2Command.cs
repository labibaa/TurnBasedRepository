using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ultimate2Command : IUltimate
{
    CharacterBaseClasses playerCharacter;
    TemporaryStats playerTempStats;
    UltimateActionsFactory ultimateScriptable;

    public Ultimate2Command(UltimateActionsFactory ultimateScritableObject)
    {
        ultimateScriptable = ultimateScritableObject;
    }
    public void Execute()
    {
        throw new System.NotImplementedException();
    }

    public string GetUltimateActionName()
    {
        return ultimateScriptable.UltimateName;
    }

    public int GetultimateThreshold()
    {
        return ultimateScriptable.actionThreshold;
    }

    public bool IsSingleTarget()
    {
        throw new System.NotImplementedException();
    }

    public void setValues(CharacterBaseClasses playerCh, TemporaryStats playerTemp, CharacterBaseClasses targetCh, TemporaryStats targetTemp)
    {
        playerCharacter = playerCh;
        playerTempStats = playerTemp;
    }

    
}
