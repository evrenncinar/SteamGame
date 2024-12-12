using UnityEngine;
using Mirror;
using Steamworks;

public class PlayerObjectController : NetworkBehaviour
{
    //Player Data
    [SyncVar] public int _connectionID;
    [SyncVar] public int _playerIDNumber;
    [SyncVar] public ulong _playerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string _playerName;

    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if(manager != null) 
            { 
                return manager; 
            }
            return manager = NetworkManager.singleton as CustomNetworkManager;
        }
    }

    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        LobbyController.instance.FindLocalPlayer();
        LobbyController.instance.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        Manager.GamePlayers.Add(this);
        LobbyController.instance.UpdateLobbyName();
        LobbyController.instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Manager.GamePlayers.Remove(this);
        LobbyController.instance.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayerName(string PlayerName)
    {
        this.PlayerNameUpdate(this._playerName, PlayerName);
    }
    
    public void PlayerNameUpdate(string oldName, string newName)
    {
        if(isServer) // Host
        {
            this._playerName = newName;
        }
        if(isClient)
        {
            LobbyController.instance.UpdatePlayerList();
        }
    }
}
