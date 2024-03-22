using System.Collections;
using System.Collections.Generic;
using Core.Components;
using TMPro;
using UnityEngine;

public class InGameUI : UIBase
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    
    public void SetScore(int score)
    {
        _scoreText.SetText("Score\n{0}", score);
    }
}
