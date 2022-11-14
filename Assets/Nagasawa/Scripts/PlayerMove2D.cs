using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections;
using System;

public class PlayerMove2D : MonoBehaviour
{

    [Tooltip("横移動入力")] float _inputHorizontal;
    [Tooltip("ジャンプ入力")] bool _isJumpInput;
    [Tooltip("ジャンプのカウント")] int _jumpCount;
    [Tooltip("加速入力")] bool _isAccelerationInput;
    Rigidbody2D _rb;
    bool _isGrounded;
    public bool IsGrounded { get => _isGrounded; }

    PhotonView _view;
    SpriteRenderer _sprite;
    Nitsuma.WallKick _wallkick;
    Grapple _grapple;
    Sliding _sliding;
    bool _canMove = true;

    [Header("MoveSettings")]
    [Tooltip("通常時のスピード"), SerializeField] float _normalSpeed = 3;
    [Tooltip("加速時のスピード"), SerializeField] float _acceleratorSpeed = 5;
    float _currentSpeed = 0;
    [Tooltip("AddForce時の速度乗数")] float _addForceMoveMultiplier;
    [Tooltip("AddForce時の速度乗数(加速)"), SerializeField] float _accelerationMultiplication = 0.1f;
    [Tooltip("AddForce時の速度乗数(減速)"), SerializeField] float _decelerationMultiplication = 10f;
    float _accelerationValue = 0;
    [SerializeField] float _maxAccelerationValue = 100f;
    [SerializeField, Tooltip("加速仕切るまでの速度")] float _changeSpeed = 10f;
    [SerializeField, Tooltip("accelerationValueを減らす速度")] float _changeSpeedValue = 10f;
    [SerializeField] Slider _slider;
    [SerializeField, Tooltip("changeSpeedを増加させる値"), Range(0, 100)] float _addValue = 3f;

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

    [Header("Debug")]
    [SerializeField] bool _isAccelerator;



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
        ChangeSpeed();
    }

    public void DontMove(float time)
    {
        _canMove = false;
        StartCoroutine(DelayMethod(time, () => _canMove = true));
    }
    IEnumerator DelayMethod(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action?.Invoke();
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
        _wallkick = GetComponent<Nitsuma.WallKick>();
        _grapple = GetComponent<Grapple>();
        _sliding = GetComponent<Sliding>();
        //_slider.value = _accelerationValue;
        _currentSpeed = _normalSpeed;

        if (_isAccelerator)
            _accelerationValue = _maxAccelerationValue;

        if (PhotonNetwork.IsConnected)
        {
            _sprite.color = _playerColorList[_view.OwnerActorNr % _playerColorList.Length];
        }
        else
        {
            _sprite.color = _playerColorList[UnityEngine.Random.Range(0, _playerColorList.Length)];
        }

        ////Sliderをセットするのイベントを呼ぶ
        //RaiseEventOptions target = new RaiseEventOptions();
        //target.Receivers = ReceiverGroup.All;
        //SendOptions sendOptions = new SendOptions();
        //PhotonNetwork.RaiseEvent(5, null, target, sendOptions);
    }

    /// <summary>
    /// プレイヤーの入力
    /// </summary>
    void PlayerInput()
    {
        _inputHorizontal = Input.GetAxisRaw("Horizontal");
        _isJumpInput = Input.GetButtonDown("Jump");
        _isAccelerationInput = Input.GetKey(KeyCode.UpArrow);
    }

    /// <summary>
    /// AddForceの移動
    /// </summary>
    void AddForceMove()
    {
        if (_wallkick.IswallKick || _sliding.IsSliding || _grapple.CurrentGrapple || !_canMove) { return; }

        _addForceMoveMultiplier = (_inputHorizontal == 0) ? _decelerationMultiplication : _accelerationMultiplication;

        if (_rb)
        {
            Vector2 vec = new Vector2(_inputHorizontal * _currentSpeed, 0);
            _rb.AddForce(_addForceMoveMultiplier * (vec - new Vector2(_rb.velocity.x, 0)));
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
        if (_isJumpInput && _jumpCount < _maxJumpCount && _canMove)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, _jumpPower * 10);
            _jumpCount++;
        }
    }

    /// <summary>
    /// 加速ゲージを増加させる
    /// </summary>
    /// <param name="value"></param>
    [PunRPC]
    public void IncreaseAccelerationValue()
    {
        _accelerationValue += _addValue * Time.deltaTime;

        if (_accelerationValue >= _maxAccelerationValue) //ゲージが上限に達した時
        {
            _accelerationValue = _maxAccelerationValue;
        }

        _view.RPC("ChangeSlider", RpcTarget.All); //全員
    }
    [PunRPC]
    void DecreaseAccelerationValue()
    {
        _accelerationValue -= Time.deltaTime * _changeSpeedValue;

        _view.RPC("ChangeSlider", RpcTarget.All); //全員
    }

    [PunRPC]
    void ChangeSlider()
    {
        if (_slider)
        {
            _slider.value = _accelerationValue / _maxAccelerationValue;
        }
    }

    /// <summary>
    /// CurrentSpeedを入力に応じて変化させる
    /// </summary>
    void ChangeSpeed()
    {
        if(!_canMove) { return; }

        if (_accelerationValue <= 0)
        {
            _accelerationValue = 0;
        }
        else if (_isAccelerationInput)
        {
            if (_currentSpeed < _acceleratorSpeed)
            {
                _currentSpeed += Time.deltaTime * _changeSpeed;

                if (_currentSpeed >= _acceleratorSpeed)
                {
                    _currentSpeed = _acceleratorSpeed;
                }
            }

            if (!_isAccelerator)
                _view.RPC("DecreaseAccelerationValue", RpcTarget.All); //全員に知らせる

            return;
        }

        if (_currentSpeed > _normalSpeed)
        {
            _currentSpeed -= Time.deltaTime * _changeSpeed;

            if (_currentSpeed <= _normalSpeed)
            {
                _currentSpeed = _normalSpeed;
            }
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

    [PunRPC]
    void SetSlider()
    {
        var sliders = FindObjectsOfType<Slider>();

        foreach (var slider in sliders)
        {
            var view = slider.gameObject.GetPhotonView();

            if (view && view.IsMine)
            {
                _slider = slider;
                _slider.value = _accelerationValue;

                var text = _slider.GetComponentInChildren<Text>();
                var n = PhotonNetwork.LocalPlayer.ActorNumber;
                text.text = $"Plsyer{n}";
                break;
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
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

