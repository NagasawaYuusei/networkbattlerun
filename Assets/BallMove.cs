using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMove : Itembase
{
    [SerializeField]
    float _speed = 3f;
    [SerializeField]
    float _dis = 1.2f;
    [SerializeField]
    LayerMask _layer;
    int _count = 0;
    [SerializeField]
    string _tag;
    MoveDirection _dir = MoveDirection.Right;
    Rigidbody2D _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        var x = _dir == MoveDirection.Right ? 1 : -1;
        Ray2D ray = new Ray2D(transform.position, new Vector2(x, 0));
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, _dis, _layer);
        if (hit.collider)
        {
            _count++;
            if (_count == 2) { Destroy(gameObject); }

            var scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;

            if (_dir == MoveDirection.Right) { _dir = MoveDirection.Left; }
            else { _dir = MoveDirection.Right; }
        }

        Vector3 velocity = _rb.velocity;
        velocity = x * transform.right * _speed;
        velocity.y = _rb.velocity.y;
        _rb.velocity = velocity;

        Debug.DrawRay(ray.origin, ray.direction * _dis, Color.red);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == _tag)
        {
            if (collision.gameObject.GetComponent<PlayerMove2D>())
            {
                collision.gameObject.GetComponent<PlayerMove2D>().DontMove(0.8f);
            }
            Destroy(gameObject);
        }
    }

    public override void Use(Vector3 pos)
    {
        GameObject ball = Resources.Load<GameObject>("GreenBall");
        Instantiate(ball, pos + new Vector3(1.5f, 0), Quaternion.identity);
    }
}
