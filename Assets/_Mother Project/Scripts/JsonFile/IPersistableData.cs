using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPersistableData 
{
    // call this interface on every plable character
    void SaveData(PlayerDataSave playerDataSave);

    void LoadData(PlayerDataSave playerDataSave);

}
