using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// �v���C���[���ǂ����̕����Ɉړ����Ȃ���΂Ȃ�Ȃ����𔻒肷��N���X
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
    /// ���[�J���v���C���[�̈ړ�������ύX����RPC
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