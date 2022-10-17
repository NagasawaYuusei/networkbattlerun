using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DoorSwitcher : MonoBehaviour
{
    [SerializeField] GameObject _doorObject;

    private void Start()
    {
        if (!_doorObject) return;

        _doorObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_doorObject) return;
        if (!_doorObject.activeSelf) return;

        if (collision.CompareTag("Player"))
        {
            var view = this.gameObject.GetPhotonView();

            if (view && view.IsMine)
            {
                view.RPC("Switch", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    void Switch()
    {
        _doorObject.SetActive(false);
    }
}
