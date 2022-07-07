using UnityEngine;
using Photon.Pun;

public class PlayerMove2D : MonoBehaviour
{
    /// <summary>
    /// Playerの移動タイプ
    /// </summary>
    enum MoveType
    {
        None,
        Translate,
        Position,
        Velocity,
        AddForce,
    };

    enum JumpType
    {
        None,
        Velocity,
        AddForce,
    };

    [Tooltip("横移動入力")] float _inputHorizontal;
    [Tooltip("縦移動入力")] float _inputVertical;
    [Tooltip("ジャンプ入力")] bool _isJumpInput;
    [Tooltip("現在の移動タイプ")] MoveType _nowMoveType;
    [Tooltip("現在のジャンプタイプ")] JumpType _nowJumpType;
    [Tooltip("現在上下移動に対応しているかどうか")] bool _nowIsVertical;
    [Tooltip("プレイヤーを中心としたGizmo")] Vector2 _centerPlayer;
    [Tooltip("ジャンプのカウント")] int _jumpCount;
    [Tooltip("ジャンプ後のタイマー")] float _jumptimer;
    Rigidbody2D _rb;
    bool _isGrounded;

    PhotonView _view;
    SpriteRenderer _sprite;

    [Header("MoveSettings")]
    [Tooltip("移動のタイプ"), SerializeField] MoveType _moveType = MoveType.AddForce;
    [Tooltip("プレイヤーのスピード"), SerializeField] float _playerSpeed = 3;
    [Tooltip("AddForce時の速度乗数"), SerializeField] float _addForceMoveMultiplier = 5f;
    [Tooltip("上下移動に対応するかどうか"), SerializeField] bool _isVertical = false;

    [Header("JumpSettings")]
    [Tooltip("ジャンプのタイプ"), SerializeField] JumpType _jumpType = JumpType.AddForce;
    [Tooltip("プレイヤーのジャンプパワー"), SerializeField] float _jumpPower = 2.5f;
    [Tooltip("最大ジャンプ回数"), SerializeField] int _maxJumpCount = 1;
    [Tooltip("中心差分"), SerializeField] Vector2 _point = new Vector2(0, 0);
    [Tooltip("レイヤーのサイズ"), SerializeField] Vector2 _size = new Vector2(0.98f, 1);
    [Tooltip("地面のレイヤー"), SerializeField] LayerMask _groundLayer;
    [Tooltip("ジャンプレイヤーのデバッグ"), SerializeField] bool _isJumpDebug = true;

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
        State();
        PlayerInput();
        PlayerMoveSwitch();
        PlayerJumpSwitch();
        ControlGravity();
    }

    /// <summary>
    /// スタートで呼ばれるセットアップ
    /// </summary>
    void SetUp()
    {
        if (TryGetComponent(out _rb) && _isVertical)
        {
            _rb.gravityScale = 0;
        }
        _nowIsVertical = !_isVertical;

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
    /// アップデートで更新するプレイヤーの状態
    /// </summary>
    void State()
    {
        _centerPlayer = (Vector2)transform.position + _point;
        if (_isJumpDebug)
        {
            Debug.Log(_isGrounded);
        }
    }

    /// <summary>
    /// プレイヤーの入力
    /// </summary>
    void PlayerInput()
    {
        _inputHorizontal = Input.GetAxisRaw("Horizontal");
        _inputVertical = Input.GetAxisRaw("Vertical");

        if (!_isVertical)
        {
            _isJumpInput = Input.GetButtonDown("Jump");
        }
    }

    /// <summary>
    /// プレイヤーの移動処理の選別
    /// </summary>
    void PlayerMoveSwitch()
    {
        switch (_moveType)
        {
            case MoveType.Translate:
                TranslateMove();
                break;
            case MoveType.Position:
                PositionMove();
                break;
            case MoveType.Velocity:
                VelocityMove();
                break;
            case MoveType.AddForce:
                AddForceMove();
                break;
            default:
                Debug.LogError("NoneMoveType");
                break;
        }

        if (_nowMoveType != _moveType)
        {
            _nowMoveType = _moveType;
            Debug.Log(_nowMoveType);
        }

        if (_nowIsVertical != _isVertical)
        {
            _nowIsVertical = _isVertical;
            Debug.Log("VerticalInput : " + _nowIsVertical);
        }
    }

    /// <summary>
    /// プレイヤーのジャンプ処理の選別
    /// </summary>
    void PlayerJumpSwitch()
    {
        switch (_jumpType)
        {
            case JumpType.Velocity:
                VelocityJump();
                break;
            case JumpType.AddForce:
                AddForceJump();
                break;
            default:
                Debug.LogError("NoneJumpType");
                break;
        }

        if (_nowJumpType != _jumpType)
        {
            _nowJumpType = _jumpType;
            Debug.Log(_nowJumpType);
        }
    }

    /// <summary>
    /// Translateの移動
    /// </summary>
    void TranslateMove()
    {
        if (!_isVertical)
        {
            transform.Translate(new Vector3(_inputHorizontal * _playerSpeed * 0.1f * Time.deltaTime * 250, 0));
        }
        else
        {
            Vector3 vec = new Vector3(_inputHorizontal * _playerSpeed * 0.1f * Time.deltaTime * 250,
                _inputVertical * _playerSpeed * Time.deltaTime * 250 * 0.1f);
            transform.Translate(vec.normalized);
        }
    }

    /// <summary>
    /// Positionの移動
    /// </summary>
    void PositionMove()
    {
        if (!_isVertical)
        {
            transform.position = transform.position + new Vector3(_inputHorizontal * _playerSpeed * 0.1f * Time.deltaTime * 250, 0);
        }
        else
        {
            Vector3 vec = new Vector3(_inputHorizontal * _playerSpeed * 0.1f * Time.deltaTime * 250,
                _inputVertical * _playerSpeed * 0.1f * Time.deltaTime * 250);
            transform.position = transform.position + vec.normalized;
        }
    }

    /// <summary>
    /// velocityの移動
    /// </summary>
    void VelocityMove()
    {
        if (_rb && !_isVertical)
        {
            _rb.velocity = new Vector3(_inputHorizontal * _playerSpeed * Time.deltaTime * 250, _rb.velocity.y);
        }
        else if (_rb && _isVertical)
        {
            Vector3 vec = new Vector3(_inputHorizontal * _playerSpeed * Time.deltaTime * 250, _inputVertical * _playerSpeed * Time.deltaTime * 250);
            _rb.velocity = vec.normalized;
        }
        else
        {
            Debug.LogError("Rbないよ～");
        }
    }

    /// <summary>
    /// AddForceの移動
    /// </summary>
    void AddForceMove()
    {
        if (_rb && !_isVertical)
        {
            Vector2 vec = new Vector2(_inputHorizontal * _playerSpeed * Time.deltaTime * 250, 0);
            _rb.AddForce(_addForceMoveMultiplier * (vec - _rb.velocity));
        }
        else if (_rb && _isVertical)
        {
            Vector2 vec = new Vector2(_inputHorizontal * _playerSpeed * Time.deltaTime * 250, _inputVertical * _playerSpeed * Time.deltaTime * 250);
            _rb.AddForce(_addForceMoveMultiplier * (vec.normalized - _rb.velocity));
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
            _jumptimer = 0;
            _rb.velocity = new Vector3(_rb.velocity.x, _jumpPower * 10);
            _jumpCount++;
        }
    }

    /// <summary>
    /// AddForceのジャンプ
    /// </summary>
    void AddForceJump()
    {
        if (_isJumpInput && _jumpCount < _maxJumpCount)
        {
            _jumptimer = 0;
            _rb.AddForce(Vector2.up * _jumpPower * 10, ForceMode2D.Impulse);
            _jumpCount++;
        }
    }

    /// <summary>
    /// 重力操作
    /// </summary>
    void ControlGravity()
    {
        if (_isVertical || !_isControlDrag)
            return;

        if (!_isGrounded && _rb.velocity.y < 0)
        {
            _rb.gravityScale = _airDrag;
        }
        else
        {
            _rb.gravityScale = _groundDrag;
        }
    }

    /// <summary>
    /// プレイヤーの設置判定
    /// </summary>
    /// <returns>地面に触れているかどうか</returns>
    //bool IsGrounded()
    //{
    //    var collision = Physics2D.OverlapBox(_centerPlayer, _size, 0, _groundLayer);
    //    if (collision && _jumptimer > 0.1f)
    //    {
    //        _jumpCount = 0;
    //        return true;
    //    }
    //    else
    //    {
    //        _jumptimer += Time.deltaTime;
    //        return false;
    //    }
    //}

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.layer);
        Debug.Log(_groundLayer);
        if (collision.gameObject.layer == _groundLayer.value)
        {
            _jumpCount = 0;
            _isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.layer);
        Debug.Log(_groundLayer);
        if (collision.gameObject.layer == _groundLayer)
        {
            _isGrounded = false;
        }
    }

    /// <summary>
    /// Gizmo表示
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (_isJumpDebug)
        {
            Gizmos.DrawCube(_centerPlayer, _size);
        }
    }
}

