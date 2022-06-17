using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VOrb.SantaJam;

public class PathController : MonoBehaviour
{
    [SerializeField] private GameObject _levelContainer;
    [SerializeField] private List<StageView> _levels;
    [SerializeField] private List<level_line> _targets;

    private void UpdatePathInfo()
    {
        _targets = _levelContainer.GetComponentsInChildren<level_line>(true).ToList();
        _levels = _levelContainer.GetComponentsInChildren<StageView>(true).ToList();
    }

    public void DropAllProgress()    
    {
        if (_levels.Count==0) UpdatePathInfo();
        
        foreach (var level in _levels)
        {
            BlockLevel(level);
        }
    }
    
    public void SetLevelStars(int levelNumber, int stars)
    {
        if (_levels.Count == 0) UpdatePathInfo();

        StageView view = _levels.Where(lvl => lvl.StageNumber == levelNumber).FirstOrDefault();

        if (view!=null)
        {

            view.SetLevelStars(stars);

            //save levelInfo 
            GameStorageOperator.PutToDevice(GameStorageOperator.PlayerParamNames.Level + levelNumber, view.Stars);
            GameStorageOperator.PutToDevice(GameStorageOperator.PlayerParamNames.LvlLock + levelNumber, 0);            
            StageView nextView = _levels.Where(lvl => lvl.StageNumber == levelNumber + 1).FirstOrDefault();
            if (nextView != null)
            {
                var nextStageTarget = _targets.Where(t => t.Level == nextView.StageNumber).FirstOrDefault();
                if (nextStageTarget != null)
                {
                    int curentStars = GetStarsCount(nextView.StageNumber - 1);
                    if (curentStars >= nextStageTarget.Value)
                    {
                        nextView.SetLevelStars(0);
                        GameStorageOperator.PutToDevice(GameStorageOperator.PlayerParamNames.LvlLock + nextView.StageNumber, 0);
                    }
                    else
                        BlockLevel(nextView);
                }
                else
                {
                    if (nextView.IsLocked)
                    {
                        nextView.SetLevelStars(0); //открываем если закрыт
                        GameStorageOperator.PutToDevice(GameStorageOperator.PlayerParamNames.LvlLock + nextView.StageNumber, 0);
                    }
                    //else
                    //    UpdateLast();
                }

            }

            UpdateTargetsView();

        }
    }

    public void UpdateTargetsView()
    {
        if (_levels.Count == 0) UpdatePathInfo();
        //последний открытый с нулями
        int lastWinLvl = _levels.Where(l => l.Stars > 0 && !l.IsLocked).OrderByDescending(l=>l.StageNumber).FirstOrDefault().StageNumber;
        var checkLevel = _levels.Where(l => l.StageNumber == lastWinLvl + 1).FirstOrDefault();
        if (checkLevel!=null)
        {
            var stageTarget = _targets.Where(t => t.Level == checkLevel.StageNumber).FirstOrDefault();
            if (stageTarget != null)
            {
                //если он имеет цель по звёздам
                int curentStars = GetStarsCount(lastWinLvl);
                if (curentStars >= stageTarget.Value)
                {
                    //открываем
                    checkLevel.DropStars();
                    GameStorageOperator.PutToDevice(GameStorageOperator.PlayerParamNames.LvlLock + checkLevel.StageNumber, 0);
                    SetLevelStarsFromStorage(checkLevel);
                }
            }
        }
        
        foreach (var target in _targets)
        {
            target.SetCollected(GetStarsCount(target.Level - 1));
        }
    }

    public void UpdateAllLevelsFromStorage()
    {
        if (_levels.Count == 0) UpdatePathInfo();

        foreach (var lvl in _levels)
        {
           SetLevelStarsFromStorage(lvl);
            var stageTarget = _targets.Where(t => t.Level == lvl.StageNumber).FirstOrDefault();
            if (stageTarget != null)
            {
                stageTarget.SetCollected(GetStarsCount(stageTarget.Level - 1));
            }
        }

        

    }
    private void BlockLevel(StageView level)
    {
        level.DropStars();
        level.SetLevelStars(0, level.StageNumber != 1);
    }
    private void SetLevelStarsFromStorage(StageView lvl)
    {
        lvl.DropStars();
        lvl.SetLevelStars((int)GameStorageOperator.GetFromDevice(GameStorageOperator.PlayerParamNames.Level + lvl.StageNumber, 0),
                          lvl.StageNumber != 1 ?
                          (bool)(SafeInt)(int)GameStorageOperator.GetFromDevice(GameStorageOperator.PlayerParamNames.LvlLock + lvl.StageNumber, 1) : false
                           );
    }

    public int GetStarsCount(int levelMax)
    {
        int starsCount = 0;
        if (_levels.Count == 0) UpdatePathInfo();
        for (int i = 0; i < levelMax; i++)
        {
            starsCount += _levels[i].Stars;
        }
        return starsCount;
    }
}
