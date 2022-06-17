using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VOrb.SantaJam;

public class FinalClick : MonoBehaviour
{
  public void OnClick()
    {
        int lastLevelCheck = (int)GameStorageOperator.GetFromDevice(GameStorageOperator.PlayerParamNames.Level + DataBaseManager.Instance.LevelsInfo[DataBaseManager.Instance.LevelsInfo.Count - 1].LevelNumber, 0);
        if (lastLevelCheck>0)
        {
            UIWindowsManager.GetWindow<StartWindow>().ShowWinnerScreen();
        }
    }
}
