using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class EndScreen : MonoBehaviour
{
    [SerializeField]
    internal GameObject redWinsTextObject;
    [SerializeField]
    internal GameObject blueWinsTextObject;
    [SerializeField]
    internal GameObject tieTextObject;

    [SerializeField]
    internal TextMeshProUGUI blueTotalScoreText;
    [SerializeField]
    internal TextMeshProUGUI redTotalScoreText;

    [SerializeField]
    internal TextMeshProUGUI[] blueTeamScoreText;
    [SerializeField]
    internal TextMeshProUGUI[] blueTeamPlayerText;

    [SerializeField]
    internal TextMeshProUGUI[] redTeamScoreText;
    [SerializeField]
    internal TextMeshProUGUI[] redTeamPlayerText;

    [SerializeField]
    internal float blueTeamScore { get; set; }
    [SerializeField]
    internal float redTeamScore { get; set; }
    internal bool redWins = false;
    internal bool blueWins = false;
    internal bool tie = false;
    [SerializeField]
    internal List<GameObject> redTeamPlayers = new List<GameObject>();
    [SerializeField]
    internal List<GameObject> blueTeamPlayers = new List<GameObject>();
    [SerializeField]
    internal Dictionary<string, float> redPlayers = new Dictionary<string, float>();
    [SerializeField]
    internal Dictionary<string, float> bluePlayers = new Dictionary<string, float>();
    public void PlayAgain()
    {
        Debug.Log("Play Again");
        System.Diagnostics.Process.Start(Application.dataPath.Replace("_Data", ".exe")); //new program
        Application.Quit(); //kill current process
    }

   

    internal void BuildPlayerDict()
    {
        Debug.Log("Building Player Dict");
        foreach (GameObject _player in redTeamPlayers)
        {
            if (_player.TryGetComponent(out Player player))
            {
                Debug.Log("Red Player Added to Dict");
                redPlayers.Add(player.playerName, player.totalFoodPelletsGathered);
                //return;
            }
            else
            {
                Debug.Log("_player(red) Not player, Not added");
                
            }
                if (_player.TryGetComponent(out AiPlayer aiPlayer))
            {
                Debug.Log("Red AI Player Added to Dict");
                redPlayers.Add(aiPlayer.playerName, aiPlayer.totalFoodPelletsGathered);
                //return;
            }
            else
            {
                Debug.Log("_player(red) Not AiPlayer, Not added");

            }
        }

        foreach (GameObject _player in blueTeamPlayers)
        {
            if (_player.TryGetComponent(out Player player))
            {
                Debug.Log("Blue Player Added to Dict");
                bluePlayers.Add(player.playerName, player.totalFoodPelletsGathered);
                
                //return;
            }
            else
            {
                Debug.Log("_player(Blue) Not player, Not added");

            }
            if (_player.TryGetComponent(out AiPlayer aiPlayer))
            {
                Debug.Log("Blue AI Player Added to Dict");
                bluePlayers.Add(aiPlayer.playerName, aiPlayer.totalFoodPelletsGathered);
                //return;
            }
            else
            {
                Debug.Log("_player(Blue) Not AiPlayer, Not added");

            }
        }

        //AssignDictToText();


    }

    internal void AssignDictToText()
    {
        Debug.Log("Assigning Dict to Text");
        int redIndex = 0;
        Debug.Log("redPlayer Count: " + redPlayers.Count);
        foreach (KeyValuePair<string, float> player in redPlayers)
        {
            Debug.Log("Red Player: " + player.Key + " Score: " + player.Value);
            if (player.Key != "" || player.Key != null)
            {
                redTeamPlayerText[redIndex].gameObject.SetActive(true);
                redTeamScoreText[redIndex].gameObject.SetActive(true);
                redTeamPlayerText[redIndex].text = player.Key;
                redTeamScoreText[redIndex].text = player.Value.ToString();
                redIndex++;
            }
            else if (player.Key == "" || player.Key == null)
            {
                Debug.Log("Red Player Name is Null");
                redTeamPlayerText[redIndex].gameObject.SetActive(false);
                redTeamScoreText[redIndex].gameObject.SetActive(false);
            }

        }



        int blueIndex = 0;
        Debug.Log("bluePlayer Count: " + bluePlayers.Count);
        foreach (KeyValuePair<string, float> player in bluePlayers)
        {

            if (player.Key != "" || player.Key != null)
            {
                Debug.Log("Blue Player: " + player.Key + " Score: " + player.Value);
                blueTeamPlayerText[blueIndex].gameObject.SetActive(true);
                blueTeamScoreText[blueIndex].gameObject.SetActive(true);
                blueTeamPlayerText[blueIndex].text = player.Key;
                blueTeamScoreText[blueIndex].text = player.Value.ToString();
                blueIndex++;
            }
            else if (player.Key == "" || player.Key == null)
            {
                Debug.Log("Blue Player Name is Null");
                blueTeamPlayerText[blueIndex].gameObject.SetActive(false);
                blueTeamScoreText[blueIndex].gameObject.SetActive(false);
            }

        }




    }
    internal void GameOver()
    {
        //if (redPlayers.Count == 0 || bluePlayers.Count == 0)
        //{
        //    Debug.Log("No Players found in Dict..... ReBuilding");
        //    BuildPlayerDict();
            
        //}
        if (!tie && !redWins && blueWins)
        {

            blueWinsTextObject.SetActive(true);
            redWinsTextObject.SetActive(false);
            tieTextObject.SetActive(false);
        }
        else if (!tie && redWins && !blueWins)
        {
            redWinsTextObject.SetActive(true);
            blueWinsTextObject.SetActive(false);
            tieTextObject.SetActive(false);

        }
        else if (tie && !redWins && !blueWins)
        {
            tieTextObject.SetActive(true);
            redWinsTextObject.SetActive(false);
            blueWinsTextObject.SetActive(false);
        }

        blueTotalScoreText.text = blueTeamScore.ToString();
        redTotalScoreText.text = redTeamScore.ToString();

        AssignDictToText();
    }




    



}