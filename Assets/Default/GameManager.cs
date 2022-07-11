using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

/// <summary>
/// ゲームを管理するコンポーネント
/// イベントコード 2 をリタイアとする
/// イベントコード 3 をカメラが切り替わるときとする
/// </summary>
public class GameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    //Singlton化
    public static GameManager Instance;
    void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    bool _owner;
    bool _isDuringGame;
    [SerializeField] Button _gameStartButton;

    public bool Owner => _owner;
    public bool IsDuringGame => _isDuringGame;

    public void MineOwner()
    {
        _gameStartButton.gameObject.SetActive(true);
        _owner = true;
    }

    public void GameStart()
    {
        Debug.Log("Closing Room");
        PhotonNetwork.CurrentRoom.IsOpen = false;
        //4 => GameStart
        RaiseEventOptions target = new RaiseEventOptions();
        target.Receivers = ReceiverGroup.All;
        SendOptions sendOptions = new SendOptions();
        PhotonNetwork.RaiseEvent(4, null, target, sendOptions);
    }

    void IOnEventCallback.OnEvent(EventData photonEvent)
    {
        //リタイア
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
        //カメラが切り替わった時
        if(photonEvent.Code == 3)
        {
            print($"切り替わった");
            var cam = FindObjectOfType<CameraSwitcher>();
            cam.ChangeDirection();
        }
        //GameStart
        if(photonEvent.Code == 4)
        {
            _gameStartButton.gameObject.SetActive(false);
            _isDuringGame = true;
        }
    }
}
