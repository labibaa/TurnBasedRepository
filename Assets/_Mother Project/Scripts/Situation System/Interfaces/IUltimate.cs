using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUltimate 
{
    // Interface for all the ultimate systems playable in grid system
    void setValues(CharacterBaseClasses playerCh, TemporaryStats playerTemp);
    void Execute();
    int GetultimateThreshold();
    string GetUltimateActionName();
}
