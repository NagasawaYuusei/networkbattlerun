using UnityEngine;
using Photon.Pun;

public class PlayerMove2D : MonoBehaviour
{

    [Tooltip("���ړ�����")] float _inputHorizontal;
    [Tooltip("�W�����v����")] bool _isJumpInput;
    [Tooltip("�W�����v�̃J�E���g")] int _jumpCount;
    [Tooltip("��������")] bool _isAccelerationInput;
    Rigidbody2D _rb;
    bool _isGrounded;
    public bool IsGrounded { get => _isGrounded; }

    PhotonView _view;
    SpriteRenderer _sprite;
    Nitsuma.WallKick _wallkick;
    Grapple _grapple;
    Sliding _sliding;

    [Header("MoveSettings")]
    [Tooltip("�ʏ펞�̃X�s�[�h"), SerializeField] float _normalSpeed = 3;
    [Tooltip("�������̃X�s�[�h"), SerializeField] float _acceleratorSpeed = 5;
    float _currentSpeed = 0;
    [Tooltip("AddForce���̑��x�搔")] float _addForceMoveMultiplier;
    [Tooltip("AddForce���̑��x�搔(����)"), SerializeField] float _accelerationMultiplication = 0.1f;
    [Tooltip("AddForce���̑��x�搔(����)"), SerializeField] float _decelerationMultiplication = 10f;
    float _accelerationValue = 0;
    [SerializeField] float _maxAccelerationValue = 100f;
    [SerializeField] float _changeSpeedMultiplication = 10f;

    [Header("JumpSettings")]
    [Tooltip("�v���C���[�̃W�����v�p���["), SerializeField] float _jumpPower = 2.5f;
    [Tooltip("�ő�W�����v��"), SerializeField] int _maxJumpCount = 1;

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
        PlayerInput();
        VelocityJump();
        ChangeSpeed();
    }

    void FixedUpdate()
    {
        AddForceMove();
        ControlGravity();
    }

    /// <summary>
    /// �X�^�[�g�ŌĂ΂��Z�b�g�A�b�v
    /// </summary>
    void SetUp()
    {
        _rb = GetComponent<Rigidbody2D>();
        _currentSpeed = _normalSpeed;

        if (!_isOnline) return;
        _view = gameObject.GetPhotonView();
        _sprite = GetComponent<SpriteRenderer>();
        _wallkick = GetComponent<Nitsuma.WallKick>();
        _grapple = GetComponent<Grapple>();
        _sliding = GetComponent<Sliding>();

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
    /// �v���C���[�̓���
    /// </summary>
    void PlayerInput()
    {
        _inputHorizontal = Input.GetAxisRaw("Horizontal");
        _isJumpInput = Input.GetButtonDown("Jump");
        _isAccelerationInput = Input.GetKey(KeyCode.W);
    }

    /// <summary>
    /// AddForce�̈ړ�
    /// </summary>
    void AddForceMove()
    {
        if(_wallkick.IswallKick || _sliding.IsSliding || _grapple.CurrentGrapple) { return; }

        _addForceMoveMultiplier = (_inputHorizontal == 0) ? _decelerationMultiplication : _accelerationMultiplication;

        if (_rb)
        {
            Vector2 vec = new Vector2(_inputHorizontal * _currentSpeed, 0);
            _rb.AddForce(_addForceMoveMultiplier * (vec - new Vector2(_rb.velocity.x,0)));
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
            _rb.velocity = new Vector3(_rb.velocity.x, _jumpPower * 10);
            _jumpCount++;
        }
    }

    /// <summary>
    /// �����Q�[�W�𑝉�������
    /// </summary>
    /// <param name="value"></param>
    public void AddAccelerationValue(float value)
    {
        if (value < 0) return; //���̒l�������ė�����return

        if(_accelerationValue >= _maxAccelerationValue) //�Q�[�W������ɒB������
        {
            _accelerationValue = _maxAccelerationValue;
            return;
        }

        _accelerationValue += value;
    }

    /// <summary>
    /// CurrentSpeed����͂ɉ����ĕω�������
    /// </summary>
    void ChangeSpeed()
    {
        if (_accelerationValue <= 0)
        {
            _accelerationValue = 0;
        }
        else if(_isAccelerationInput)
        {
            if (_currentSpeed < _acceleratorSpeed)
            {
                _currentSpeed += Time.deltaTime * _changeSpeedMultiplication;

                if(_currentSpeed >= _acceleratorSpeed)
                {
                    _currentSpeed = _acceleratorSpeed;
                }
            }

            _accelerationValue -= Time.deltaTime * _changeSpeedMultiplication;

            return;
        }

        if(_currentSpeed > _normalSpeed)
        {
            _currentSpeed -= Time.deltaTime * _changeSpeedMultiplication;

            if(_currentSpeed <= _normalSpeed)
            {
                _currentSpeed = _normalSpeed;
            }
        }
    }

    /// <summary>
    /// �d�͑���
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

