using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sliding : MonoBehaviour
{
    [SerializeField] float _slidingTime = 1f;
    [SerializeField] float _speed = 3f;
    bool _isSliding;
    float _lastX = 0f;
    Rigidbody2D _rb2d;
    PlayerMove2D _playerMove;
    Animator _anim;
    /// <summary>
    /// スライディングしているかどうか
    /// </summary>
    public bool IsSliding { get => _isSliding; }

    // Start is called before the first frame update
    void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _playerMove = GetComponent<PlayerMove2D>();
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        var h = Input.GetAxisRaw("Horizontal");
        var dir = new Vector2(h, 0);
        if (h != 0 && !_isSliding)
        {
            _lastX = h;
        }

        if (Input.GetKeyDown(KeyCode.S) && dir != Vector2.zero && !_isSliding)
        {
            if (!_playerMove.IsGrounded) { return; }
            _anim.Play("Sliding");
            _isSliding = true;
            StartCoroutine(DelayMethod(_slidingTime, () =>
            {
                _anim.Play("SlidingEnd");
                _isSliding = false;
            }));
        }
    }

    void FixedUpdate()
    {
        if (_isSliding)
        {
            _rb2d.AddForce(new Vector2(_lastX * _speed, 0));
        }
    }
    IEnumerator DelayMethod(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action?.Invoke();
    }
}
