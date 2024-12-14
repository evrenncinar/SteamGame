using System;
using Steamworks;
using TMPro;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private TMP_InputField _lobbyCode;

    public void HostLobby()
    {
        SteamLobby.instance.HostLobby();
    }

    public void JoinLobby()
    {
        ulong _lobbyID = Convert.ToUInt64(_lobbyCode.text);
        SteamLobby.instance.JoinLobby((CSteamID)_lobbyID);
    }
}
