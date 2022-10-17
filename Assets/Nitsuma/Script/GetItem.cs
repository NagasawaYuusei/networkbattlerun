using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetItem : MonoBehaviour
{
    List<Itembase> _items = new List<Itembase>();
    readonly SpeedUPItem _speedUPItem = new SpeedUPItem();
    readonly TestItem _testItem = new TestItem();
    private void Awake()
    {
        _items.Add(_speedUPItem);
        _items.Add(_testItem);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        RandamItemGet();
        Destroy(gameObject);
    }

    void RandamItemGet()
    {
        var num = Random.Range(0, _items.Count);

        _items[num].Use();
    }
}
