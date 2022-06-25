using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

/// <summary>
/// ゲームを管理するコンポーネント
/// イベントコード 2 をリタイアとする
/// </summary>
public class GameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    void IOnEventCallback.OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 2)
        {
            print($"Player {photonEvent.Sender} retired.");
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            // すぐに Destroy されていないかもしれないので、リタイアした者を除いたプレイヤーリストを作る
            players = players.Where(x => x.GetPhotonView().OwnerActorNr != photonEvent.Sender).ToArray();
            
            if (players.Length == 1)
            {
                PhotonView view = players[0].GetPhotonView();
                print($"Player {view.OwnerActorNr} win");
            }
        }
    }
}
