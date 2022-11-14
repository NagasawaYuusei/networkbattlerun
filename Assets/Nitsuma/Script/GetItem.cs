using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetItem : MonoBehaviour
{
    [SerializeField] List<Itembase> _items = new List<Itembase>();

    [SerializeField] string _itemTag = "";
    Itembase _currentItem;

    [SerializeField] Image _itemImage;

    public Image ItemImage { get => _itemImage; set => _itemImage = value; }

    private void Awake()
    {

    }

    private void Update()
    {
        if (!_currentItem) return;

        if (Input.GetButtonDown("Fire1"))
        {
            _currentItem.Use(this.transform.position);
            _currentItem = null;

            if(ItemImage)
            {
                ItemImage.enabled = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(_itemTag)) return;

        Destroy(collision.gameObject);

        //ƒAƒCƒeƒ€‚ğ‚Á‚Ä‚¢‚½‚çæ“¾‚µ‚È‚¢
        if (_currentItem) return;

        RandamItemGet();
    }

    void RandamItemGet()
    {
        var num = Random.Range(0, _items.Count);

        _currentItem = _items[num];

        if(ItemImage)
        {
            ItemImage.enabled = true;
            ItemImage.sprite = _currentItem?.Sprite;
        }
    }
}