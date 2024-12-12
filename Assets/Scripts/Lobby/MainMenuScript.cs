using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
   public void HostLobby()
   {
       SteamLobby.instance.HostLobby();
   }
}
