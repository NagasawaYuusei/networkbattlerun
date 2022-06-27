using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

/// <summary>
/// �v���C���[��Trigger�ɓ�������J�����̒Ǐ]������؂�ւ���
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

        //���������[�J���v���C���[���擪�̃v���C���[��������
        if (player.MoveDirection == cam.MoveDirection)
        {
            if (view && view.IsMine)
            {
                //GameManager�̃C�x���g���Ă�
                RaiseEventOptions target = new RaiseEventOptions();
                target.Receivers = ReceiverGroup.All;
                SendOptions sendOptions = new SendOptions();
                PhotonNetwork.RaiseEvent(3, null, target, sendOptions);
            }
        }

        //���[�J���v���C���[�̈ړ�������ύX����
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
