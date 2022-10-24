using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetItem : MonoBehaviour
{
    List<Itembase> _items = new List<Itembase>();
    readonly SpeedUPItem _speedUPItem = new SpeedUPItem();
    readonly TestItem _testItem = new TestItem();
    readonly BallMove _greenItem = new BallMove();
    readonly BananaControl _bananaItem = new BananaControl();

    [SerializeField] string _itemTag = "";
    Itembase _currentItem;

    private void Awake()
    {
        _items.Add(_speedUPItem);
        _items.Add(_testItem);
        _items.Add(_greenItem);
        _items.Add(_bananaItem);
    }

    private void Update()
    {
        if (!_currentItem) return;

        if(Input.GetButtonDown("Fire1"))
        {
            _currentItem.Use();
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
}
