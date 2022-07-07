using UnityEngine;
using Photon.Pun;

public class PlayerMove2D : MonoBehaviour
{
    /// <summary>
    /// Player�̈ړ��^�C�v
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

    [Tooltip("���ړ�����")] float _inputHorizontal;
    [Tooltip("�c�ړ�����")] float _inputVertical;
    [Tooltip("�W�����v����")] bool _isJumpInput;
    [Tooltip("���݂̈ړ��^�C�v")] MoveType _nowMoveType;
    [Tooltip("���݂̃W�����v�^�C�v")] JumpType _nowJumpType;
    [Tooltip("���ݏ㉺�ړ��ɑΉ����Ă��邩�ǂ���")] bool _nowIsVertical;
    [Tooltip("�v���C���[�𒆐S�Ƃ���Gizmo")] Vector2 _centerPlayer;
    [Tooltip("�W�����v�̃J�E���g")] int _jumpCount;
    [Tooltip("�W�����v��̃^�C�}�[")] float _jumptimer;
    Rigidbody2D _rb;
    bool _isGrounded;

    PhotonView _view;
    SpriteRenderer _sprite;

    [Header("MoveSettings")]
    [Tooltip("�ړ��̃^�C�v"), SerializeField] MoveType _moveType = MoveType.AddForce;
    [Tooltip("�v���C���[�̃X�s�[�h"), SerializeField] float _playerSpeed = 3;
    [Tooltip("AddForce���̑��x�搔"), SerializeField] float _addForceMoveMultiplier = 5f;
    [Tooltip("�㉺�ړ��ɑΉ����邩�ǂ���"), SerializeField] bool _isVertical = false;

    [Header("JumpSettings")]
    [Tooltip("�W�����v�̃^�C�v"), SerializeField] JumpType _jumpType = JumpType.AddForce;
    [Tooltip("�v���C���[�̃W�����v�p���["), SerializeField] float _jumpPower = 2.5f;
    [Tooltip("�ő�W�����v��"), SerializeField] int _maxJumpCount = 1;
    [Tooltip("���S����"), SerializeField] Vector2 _point = new Vector2(0, 0);
    [Tooltip("���C���[�̃T�C�Y"), SerializeField] Vector2 _size = new Vector2(0.98f, 1);
    [Tooltip("�n�ʂ̃��C���["), SerializeField] LayerMask _groundLayer;
    [Tooltip("�W�����v���C���[�̃f�o�b�O"), SerializeField] bool _isJumpDebug = true;

    [Header("Drag")]
    [Tooltip("�d�͂��R���g���[�����邩�ǂ���"), SerializeField] bool _isControlDrag = true;
    [Tooltip("�n�ʂɂ���Ƃ��̏d��"), SerializeField] float _groundDrag = 1;
    [Tooltip("�󒆂ɂ���Ƃ��̏d��"), SerializeField] float _airDrag = 8;

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
    /// �X�^�[�g�ŌĂ΂��Z�b�g�A�b�v
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
    /// �A�b�v�f�[�g�ōX�V����v���C���[�̏��
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
    /// �v���C���[�̓���
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
    /// �v���C���[�̈ړ������̑I��
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
    /// �v���C���[�̃W�����v�����̑I��
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
    /// Translate�̈ړ�
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
    /// Position�̈ړ�
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
    /// velocity�̈ړ�
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
            Debug.LogError("Rb�Ȃ���`");
        }
    }

    /// <summary>
    /// AddForce�̈ړ�
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
            Debug.LogError("Rb�Ȃ���`");
        }
    }

    /// <summary>
    /// velocity�̃W�����v
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
    /// AddForce�̃W�����v
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
    /// �d�͑���
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
    /// �v���C���[�̐ݒu����
    /// </summary>
    /// <returns>�n�ʂɐG��Ă��邩�ǂ���</returns>
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
    /// Gizmo�\��
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

