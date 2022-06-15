using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VOrb;

public class ChallengeInfoBar : InfoBar
{
    protected override void UpdateContent()
    {
        AudioServer.PlaySound(Sound.ButtonClick);
        Challenge current = GameService.Instance.challengeServer.GetCurrent();
        _output.text = current.Reward.ToString();
    }

    public override void Show()
    {
        if (GameService.Instance.challengeServer.GetCurrent().CoinsTarget>0)
        {
            base.Show();
        }
        
    }

}
