using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nitsuma
{
    public class WallKick : MonoBehaviour
    {
        [SerializeField]
        float _jumpPower = 5f;
        [SerializeField]
        float _kickPower = 5f;

        bool _iswallKick;
        float _kickedDir;
        float h;

        Rigidbody2D _rb2d;
        // Start is called before the first frame update
        void Start()
        {
            _rb2d = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {
            h = Input.GetAxisRaw("Horizontal");
            Ray2D ray = new Ray2D(transform.position + new Vector3(h / 1.95f, 0, 0), new Vector2(h, 0));

            float maxDistance = 0.1f;

            if (h != 0)
            {
                //�q�b�g�����Pysics2D.Raycast���g�p
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, maxDistance);

                if (hit.collider)
                {
                    WallKickJump();
                }
            }
            Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.green);

            if (_iswallKick)
            {
                if (h == 0 || h == _kickedDir)
                    _rb2d.AddForce(new Vector2(_kickPower * _kickedDir * -1, 0));
                else
                {
                    return;
                }
            }


        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.name == "Ground" || (collision.gameObject.tag == "Finish" && _iswallKick))
            {
                _iswallKick = false;
            }
        }

        void WallKickJump()
        {
            if (Input.GetButtonDown("Jump"))
            {
                _rb2d.AddForce(new Vector2(_kickPower * h * -1, _jumpPower), ForceMode2D.Impulse);
                _iswallKick = true;
                _kickedDir = h;
                var scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
            }
        }
    }
}