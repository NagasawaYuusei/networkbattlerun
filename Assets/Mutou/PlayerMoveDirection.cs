using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// プレイヤーがどっちの方向に移動しなければならないかを判定するクラス
/// </summary>
public class PlayerMoveDirection : MonoBehaviour
{
    MoveDirection _moveDirection = MoveDirection.Right;
    PhotonView _view;

    public MoveDirection MoveDirection { get => _moveDirection;}
    public PhotonView View { get => _view;}

    private void Awake()
    {
         _view = GetComponent<PhotonView>();
    }

    /// <summary>
    /// ローカルプレイヤーの移動方向を変更するRPC
    /// </summary>
    [PunRPC]
    public void ChangeDirection()
    {
        if(_moveDirection == MoveDirection.Left)
        {
            _moveDirection = MoveDirection.Right;
        }
        else
        {
            _moveDirection = MoveDirection.Left;
        }
    }
}
