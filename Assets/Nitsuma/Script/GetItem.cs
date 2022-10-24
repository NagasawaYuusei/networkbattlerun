using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GetItem : MonoBehaviour
{
    List<Itembase> _items = new List<Itembase>();
    readonly SpeedUPItem _speedUPItem = new SpeedUPItem();
    readonly TestItem _testItem = new TestItem();

    [SerializeField] string _itemTag = "";
    Itembase _currentItem;

    private void Awake()
    {
        _items.Add(_speedUPItem);
        _items.Add(_testItem);
    }

    private void Update()
    {
        if (!_currentItem) return;

        if(Input.GetButtonDown("Fire1"))
        {
            var view = this.gameObject.GetPhotonView();

            if(view && view.IsMine)
            {
                view.RPC("UseItem", RpcTarget.All);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_currentItem.CompareTag(_itemTag)) return;

        //ƒAƒCƒeƒ€‚ğ‚Á‚Ä‚¢‚½‚çæ“¾‚µ‚È‚¢
        if (_currentItem) return;

        RandamItemGet();
    }

    void RandamItemGet()
    {
        var num = Random.Range(0, _items.Count);

        _currentItem = _items[num];
    }

    [PunRPC]
    void UseItem()
    {
        _currentItem.Use();
    }
}
