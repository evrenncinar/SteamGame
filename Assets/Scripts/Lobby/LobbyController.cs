using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class LobbyController : MonoBehaviour
{
    public static LobbyController instance;

    //UI Elements
    public TMP_Text _lobbyNameText;

    //Player Data
    public GameObject _playerListViewContent;
    public GameObject _playerListItemPrefab;
    public GameObject _localPlayerObject;

    //Other Data
    public ulong _currentLobbyID;
    public bool _playerItemCreated = false;
    private List<LobbyListItemScript> _playerListItems = new List<LobbyListItemScript>();
    public PlayerObjectController _localPlayerObjectController;

    //Manager
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

    private void Awake() 
    {
        if(instance == null) { instance = this; }
    }

    public void UpdateLobbyName()
    {
        _currentLobbyID = Manager.GetComponent<SteamLobby>().CurrentLobbyID;
        _lobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(_currentLobbyID), "Name");
    }

    public void UpdatePlayerList() 
    {
        if(!_playerItemCreated) { CreateHostPlayerItem();} //Host
        if(_playerListItems.Count < Manager.GamePlayers.Count) { CreateClientPlayerItem(); } //Clients
        if(_playerListItems.Count > Manager.GamePlayers.Count) { RemovePlayerItem(); } //Clients
        if(_playerListItems.Count == Manager.GamePlayers.Count) { UpdatePlayerItem(); }
    }

    public void FindLocalPlayer()
    {
        _localPlayerObject = GameObject.Find("LocalGamePlayer");
        _localPlayerObjectController = _localPlayerObject.GetComponent<PlayerObjectController>();
    }

    public void CreateHostPlayerItem()
    {
        foreach(PlayerObjectController player in Manager.GamePlayers)
        {
            GameObject newPlayerItem = Instantiate(_playerListItemPrefab) as GameObject;
            LobbyListItemScript newPlayerItemScript = newPlayerItem.GetComponent<LobbyListItemScript>();

            newPlayerItemScript._playerName = player._playerName;
            newPlayerItemScript._connectionID = player._connectionID;
            newPlayerItemScript._playerSteamID = player._playerSteamID;
            newPlayerItemScript.SetPlayerValues();

            newPlayerItem.transform.SetParent(_playerListViewContent.transform);
            newPlayerItem.transform.localScale = Vector3.one;

            _playerListItems.Add(newPlayerItemScript);
        }
        _playerItemCreated = true;
    }

    public void CreateClientPlayerItem()
    {
        foreach(PlayerObjectController player in Manager.GamePlayers)
        {
            if(!_playerListItems.Any(x => x._connectionID == player._connectionID))
            {
                GameObject newPlayerItem = Instantiate(_playerListItemPrefab) as GameObject;
                LobbyListItemScript newPlayerItemScript = newPlayerItem.GetComponent<LobbyListItemScript>();

                newPlayerItemScript._playerName = player._playerName;
                newPlayerItemScript._connectionID = player._connectionID;
                newPlayerItemScript._playerSteamID = player._playerSteamID;
                newPlayerItemScript.SetPlayerValues();

                newPlayerItem.transform.SetParent(_playerListViewContent.transform);
                newPlayerItem.transform.localScale = Vector3.one;

                _playerListItems.Add(newPlayerItemScript);
            }
        }
    }

    public void UpdatePlayerItem()
    {
        foreach(PlayerObjectController player in Manager.GamePlayers)
        {
            foreach(LobbyListItemScript PlayerListItemScript in _playerListItems)
            {
                if(PlayerListItemScript._connectionID == player._connectionID)
                {
                    PlayerListItemScript._playerName = player._playerName;
                    PlayerListItemScript.SetPlayerValues();
                }   
            }
        }
    }

    public void RemovePlayerItem()
    {
        List<LobbyListItemScript> _playerListItemToRemove = new List<LobbyListItemScript>();

        foreach (LobbyListItemScript playerlistItem in _playerListItems)
        {
            if(!Manager.GamePlayers.Any(x => x._connectionID == playerlistItem._connectionID))
            {
                _playerListItemToRemove.Add(playerlistItem);
            }
        }
        if(_playerListItemToRemove.Count > 0)
        {
            foreach(LobbyListItemScript _playerlistItemToRemove in _playerListItemToRemove)
            {
                GameObject ObjectToRemove = _playerlistItemToRemove.gameObject;
                _playerListItems.Remove(_playerlistItemToRemove);
                Destroy(ObjectToRemove);
                ObjectToRemove = null;
            }
        }
    }
}
