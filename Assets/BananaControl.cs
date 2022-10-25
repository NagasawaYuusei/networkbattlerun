using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaControl : Itembase
{
    [SerializeField]
    float _power = 3f;
    [SerializeField]
    Vector2 _throwDir;
    [SerializeField]
    string _tag;
    MoveDirection _dir = MoveDirection.Right;
    Rigidbody2D _rb;
    // Start is called before the first frame update
    void Start()
    {
        _rb = gameObject.AddComponent<Rigidbody2D>() as Rigidbody2D;
        _throwDir.x *= _dir == MoveDirection.Left? -1 : 1;
        _rb.velocity = _throwDir.normalized * _power;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == _tag) { Destroy(gameObject); }
    }

    public override void Use(Vector3 pos)
    {
        GameObject banana = Resources.Load<GameObject>("Banana");
        Instantiate(banana, pos + new Vector3(1.5f,0),Quaternion.identity);
    }
}
