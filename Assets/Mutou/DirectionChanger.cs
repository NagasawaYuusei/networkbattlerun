using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

/// <summary>
/// プレイヤーがTriggerに入ったらカメラの追従方向を切り替える
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class DirectionChanger : MonoBehaviour
{
    Collider2D _col;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
    }
    private void Start()
    {
        _col.isTrigger = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.GetComponent<PlayerMoveDirection>();
        var cam = FindObjectOfType<CameraSwitcher>();

        var view = player.View;

        //入ったローカルプレイヤーが先頭のプレイヤーだったら
        if (player.MoveDirection == cam.MoveDirection)
        {
            if (view && view.IsMine)
            {
                //GameManagerのイベントを呼ぶ
                RaiseEventOptions target = new RaiseEventOptions();
                target.Receivers = ReceiverGroup.All;
                SendOptions sendOptions = new SendOptions();
                PhotonNetwork.RaiseEvent(3, null, target, sendOptions);
            }
        }

        //ローカルプレイヤーの移動方向を変更する
        if(view && view.IsMine)
        {
            view.RPC("ChangeDirection", RpcTarget.All);
        }
    }
}
public enum MoveDirection
{
    Left,
    Right,
}
