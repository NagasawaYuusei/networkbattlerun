using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;
using System.Collections.Generic;

public class CameraOutKill : MonoBehaviour
{
    Camera targetCamera; // �f���Ă��邩���肷��J�����ւ̎Q��
    [SerializeField]
    List<PlayerMove2D> targetObjs; // �f���Ă��邩���肷��Player�ւ̎Q�ƁBinspector�Ŏw�肷��
    /// <summary>
    /// ��ʊO�ɂ��������ǂ����m�F����Player
    /// </summary>
    public List<PlayerMove2D> TargetObjs { get => targetObjs; set => targetObjs = value; }

    Rect rect = new Rect(-0.05f, -0.05f, 1, 1); // ��ʓ������肷�邽�߂�Rect


    void Start()
    {
        targetCamera = Camera.main;
    }

    void Update()
    {
        OutsideKill();
    }

    private void OutsideKill()
    {
        if (targetObjs.Count <= 0) { return; }

        foreach (var targetobj in targetObjs)
        {
            if (!targetobj) { continue; }
            var viewportPos = targetCamera.WorldToViewportPoint(targetobj.transform.position);

            if (!rect.Contains(viewportPos))
            {
                PhotonView view = targetobj.gameObject.GetPhotonView();

                if (view && view.IsMine)
                {
                    // �C�x���g�R�[�h2�̓��^�C�A��\��
                    RaiseEventOptions target = new RaiseEventOptions();
                    target.Receivers = ReceiverGroup.All;
                    SendOptions sendOptions = new SendOptions();
                    PhotonNetwork.RaiseEvent(2, null, target, sendOptions);
                    PhotonNetwork.Destroy(view);
                }
            }
        }
    }
}
