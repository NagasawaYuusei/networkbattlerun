using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetItem : MonoBehaviour
{
    [SerializeField]
    List<Itembase> _items = new List<Itembase>();
    readonly BallMove _greenItem = new BallMove();
    readonly BananaControl _bananaItem = new BananaControl();

    [SerializeField] string _itemTag = "";

    int ItemsIndex = -1;

    private void Awake()
    {
        _items.Add(_greenItem);
        _items.Add(_bananaItem);
    }

    private void Update()
    {
        if (ItemsIndex < 0) return;

        if (Input.GetButtonDown("Fire1"))
        {
            _items[ItemsIndex].Use(transform.position);
            ItemsIndex = -1;
        }
    }
    //private void OnTriggerEnter2D(Collider2D collision)
    //{

    //}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag(_itemTag)) return;

        //ƒAƒCƒeƒ€‚ðŽ‚Á‚Ä‚¢‚½‚çŽæ“¾‚µ‚È‚¢
        if (ItemsIndex >= 0) return;

        RandamItemGet();

        collision.gameObject.SetActive(false);
    }

    void RandamItemGet()
    {
        var num = Random.Range(0, _items.Count);
        ItemsIndex = num;
    }
}
