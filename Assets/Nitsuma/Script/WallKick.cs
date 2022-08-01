using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nitsuma
{
    public class WallKick : MonoBehaviour
    {
        [SerializeField]
        float _kickPower = 5f;
        [SerializeField]
        string _objTag;
        [SerializeField, Tooltip("壁を判定するレイヤー")]
        LayerMask _layer;

        bool _iswallKick;
        float _kickedDir;
        float h;

        Rigidbody2D _rb2d;
        /// <summary>
        /// 壁キックしているかどうか
        /// </summary>
        public bool IswallKick { get => _iswallKick; }

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
                //ヒット判定にPysics2D.Raycastを使用
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, maxDistance, _layer);

                if (hit.collider)
                {
                    if (Input.GetButtonDown("Jump"))
                    {
                        WallKickJump();
                    }
                }
            }
            Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.green);
        }

        private void FixedUpdate()
        {
            if (_iswallKick)
            {
                _rb2d.AddForce(new Vector2(_kickPower * _kickedDir * -1, 0));
            }
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == _objTag)
            {
                _iswallKick = false;
            }
        }

        void WallKickJump()
        {
            _rb2d.AddForce(new Vector2(_kickPower * h * -1, 0), ForceMode2D.Impulse);
            _iswallKick = true;
            _kickedDir = h;
            var scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
}