using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using TMPro;
using Fusion.Editor;

public class LoadGameManager : Fusion.Behaviour
{ 

    [SerializeField]
    private static NetworkDebugStart _networkDebugStart;
    [SerializeField]
    private static TextMeshProUGUI sessionNameInputField;

    private string _clientCount;

    protected void ValidateClientCount()
    {
        if (_clientCount == null)
        {
            _clientCount = "1";
        }
        else
        {
            _clientCount = System.Text.RegularExpressions.Regex.Replace(_clientCount, "[^0-9]", "");
        }
    }
    protected int GetClientCount()
    {
        try
        {
            return Convert.ToInt32(_clientCount);
        }
        catch
        {
            return 0;
        }
    }
    private static void HandleQuickPlayPress()
    {

        _networkDebugStart.StartMultipleAutoClients(1);

    }

    private static void HandleHostJoinPress()
    {

        _networkDebugStart.StartHost();

    }


}
