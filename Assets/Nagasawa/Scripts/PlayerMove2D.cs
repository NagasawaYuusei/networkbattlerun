using UnityEngine;
using Photon.Pun;

public class PlayerMove2D : MonoBehaviour
{

    [Tooltip("横移動入力")] float _inputHorizontal;
    [Tooltip("ジャンプ入力")] bool _isJumpInput;
    [Tooltip("ジャンプのカウント")] int _jumpCount;
    Rigidbody2D _rb;
    bool _isGrounded;

    PhotonView _view;
    SpriteRenderer _sprite;

    [Header("MoveSettings")]
    [Tooltip("プレイヤーのスピード"), SerializeField] float _playerSpeed = 3;
    [Tooltip("AddForce時の速度乗数")] float _addForceMoveMultiplier;
    [Tooltip("AddForce時の速度乗数(加速)"), SerializeField] float _accelerationMultiplication = 0.1f;
    [Tooltip("AddForce時の速度乗数(減速)"), SerializeField] float decelerationMultiplication = 10f;

    [Header("JumpSettings")]
    [Tooltip("プレイヤーのジャンプパワー"), SerializeField] float _jumpPower = 2.5f;
    [Tooltip("最大ジャンプ回数"), SerializeField] int _maxJumpCount = 1;

    [Header("Drag")]
    [Tooltip("重力をコントロールするかどうか"), SerializeField] bool _isControlDrag = true;
    [Tooltip("地面にいるときの重力"), SerializeField] float _groundDrag = 1;
    [Tooltip("空中にいるときの重力"), SerializeField] float _airDrag = 8;

    [Header("Ather")]
    [SerializeField] Color[] _playerColorList;
    [SerializeField] bool _isOnline = true;


    void Start()
    {
        SetUp();
    }

    void Update()
    {
        if (!GameManager.Instance.IsDuringGame)
            return;
        if (_isOnline)
            if (!_view.IsMine) return;
        PlayerInput();
        VelocityJump();
    }

    void FixedUpdate()
    {
        AddForceMove();
        ControlGravity();
    }

    /// <summary>
    /// スタートで呼ばれるセットアップ
    /// </summary>
    void SetUp()
    {
        _rb = GetComponent<Rigidbody2D>();

        if (!_isOnline) return;
        _view = gameObject.GetPhotonView();
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

    /// <summary>
    /// プレイヤーの入力
    /// </summary>
    void PlayerInput()
    {
        _inputHorizontal = Input.GetAxisRaw("Horizontal");
        _isJumpInput = Input.GetButtonDown("Jump");
    }

    /// <summary>
    /// AddForceの移動
    /// </summary>
    void AddForceMove()
    {
        _addForceMoveMultiplier = (_inputHorizontal == 0) ? decelerationMultiplication : _accelerationMultiplication;

        if (_rb)
        {
            Vector2 vec = new Vector2(_inputHorizontal * _playerSpeed, 0);
            _rb.AddForce(_addForceMoveMultiplier * (vec - new Vector2(_rb.velocity.x,0)));
        }
        else
        {
            Debug.LogError("Rbないよ～");
        }
    }

    /// <summary>
    /// velocityのジャンプ
    /// </summary>
    void VelocityJump()
    {
        if (_isJumpInput && _jumpCount < _maxJumpCount)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, _jumpPower * 10);
            _jumpCount++;
        }
    }

    /// <summary>
    /// 重力操作
    /// </summary>
    void ControlGravity()
    {
        if (!_isControlDrag)
            return;

        if (!_isGrounded)
        {
            _rb.gravityScale = _airDrag;
        }
        else
        {
            _rb.gravityScale = _groundDrag;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            _jumpCount = 0;
            _isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            _isGrounded = false;
        }
    }
}

