using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

/// <summary>
/// プレイヤーが画面外に出たことを判定する
/// </summary>
public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PhotonView view = collision.gameObject.GetPhotonView();

            if (view && view.IsMine)
            {
                // イベントコード2はリタイアを表す
                RaiseEventOptions target = new RaiseEventOptions();
                target.Receivers = ReceiverGroup.All;
                SendOptions sendOptions = new SendOptions();
                PhotonNetwork.RaiseEvent(2, null, target, sendOptions);
                PhotonNetwork.Destroy(view);
            }
        }
    }
}
