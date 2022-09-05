using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;
using System.Collections.Generic;

public class CameraOutKill : MonoBehaviour
{
    Camera targetCamera; // 映っているか判定するカメラへの参照
    [SerializeField]
    List<PlayerMove2D> targetObjs; // 映っているか判定するPlayerへの参照。inspectorで指定する
    [SerializeField] GameObject _loseTextGo;
    

    Rect rect = new Rect(-0.05f, -0.05f, 1, 1); // 画面内か判定するためのRect


    void Start()
    {
        targetCamera = Camera.main;
    }

    void Update()
    {
        OutsideKill();
    }

    /// <summary>
    /// 部屋に入ってきたPlayerをlistに入れる
    /// </summary>
    /// <param name="player"></param>
    public void AddPlayer(PlayerMove2D player)
    {
        targetObjs.Add(player);
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
                    _loseTextGo.SetActive(true);
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
}
