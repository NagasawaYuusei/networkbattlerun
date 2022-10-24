using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class GetItem : MonoBehaviour
{
    [SerializeField] List<Itembase> _items = new List<Itembase>();

    [SerializeField] string _itemTag = "";
    Itembase _currentItem;

    [SerializeField] Image _itemImageUI;

    private void Awake()
    {

    }

    private void Update()
    {
        if (!_currentItem) return;

        if (Input.GetButtonDown("Fire1"))
        {
            var view = this.gameObject.GetPhotonView();

            if (view && view.IsMine)
            {
                view.RPC("UseItem", RpcTarget.All);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(_itemTag)) return;

        //アイテムを持っていたら取得しない
        if (_currentItem) return;

        RandamItemGet();
    }

    void RandamItemGet()
    {
        var num = Random.Range(0, _items.Count);

        _currentItem = _items[num];

        if (_itemImageUI)
        {
            var sprite = _currentItem.Sprite;

            if (sprite)
            {
                _itemImageUI.sprite = sprite;
            }
            else
            {
                Debug.LogError("アイテム側でSpriteがセットされていません");
            }
        }
        else
        {
            Debug.LogError("Spriteを表示するUIがセットされていません");
        }
    }

    [PunRPC]
    void UseItem()
    {
        _currentItem.Use();
    }
}
