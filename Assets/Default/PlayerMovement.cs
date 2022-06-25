using UnityEngine;
using Photon.Pun;

/// <summary>
/// プレイヤーの動きを制御するコンポーネント
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(PhotonView), typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float _speed = 5f;
    [SerializeField] float _jumpSpeed = 5f;
    [SerializeField] Color[] _playerColorList;
    PhotonView _view;
    Rigidbody2D _rb;
    SpriteRenderer _sprite;
    bool _isGrounded = false;

    private void Start()
    {
        _view = gameObject.GetPhotonView();
        _rb = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();

        if (PhotonNetwork.IsConnected)
        {
            _sprite.color = _playerColorList[_view.OwnerActorNr % _playerColorList.Length];
        }
        else
        {
            _sprite.color = _playerColorList[Random.Range(0, _playerColorList.Length)];
        }
    }

    private void Update()
    {
        if (!_view.IsMine) return;
        float h = Input.GetAxisRaw("Horizontal");
        Vector2 velocity = _rb.velocity;
        velocity.x = _speed * h;

        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            velocity.y = _jumpSpeed;
            _isGrounded = false;
        }

        _rb.velocity = velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _isGrounded = true;
    }
}
