using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Xml.Schema;
using Fusion;

public class ScoreBoard : NetworkBehaviour
{
    [SerializeField]
    internal ScoreKeeper scoreKeeper;
    [SerializeField]
    internal TextMeshProUGUI redTeamScoreText;
    [SerializeField]
    internal TextMeshProUGUI blueTeamScoreText;
    [SerializeField]
    internal float blueTeamScore { get; set; }
    [SerializeField]
    internal float redTeamScore { get; set; }
    [SerializeField]
    internal int totalScore;
    [SerializeField]
    internal Slider redSlider;
    [SerializeField]
    internal Slider blueSlider;
    internal void Start()
    {
        redSlider.value = 50;
        blueSlider.value = 50;

    }
    public override void FixedUpdateNetwork()
    {
        if (scoreKeeper != null)
        {
            totalScore = scoreKeeper.totalScore;
            blueTeamScore = scoreKeeper.blueTeamScore;
            redTeamScore = scoreKeeper.redTeamScore;
            AdjustBars();

        }
        else
        {
            {
                GameObject Go = GameObject.FindWithTag("ScoreKeeper");
                scoreKeeper = GetComponent<ScoreKeeper>();
                //totalScore = scoreKeeper.totalScore;
                //blueTeamScore = scoreKeeper.blueTeamScore;
                //redTeamScore = scoreKeeper.redTeamScore;
                //AdjustBars();
            }
        }
    }
    internal void AdjustBars()
    {


        if (totalScore > 0)
        {

            redTeamScoreText.text = redTeamScore.ToString();
        blueTeamScoreText.text = blueTeamScore.ToString();
        redSlider.value = ((redTeamScore / totalScore) * 100);
        blueSlider.value = ((blueTeamScore / totalScore) * 100);

        }
    }
}
