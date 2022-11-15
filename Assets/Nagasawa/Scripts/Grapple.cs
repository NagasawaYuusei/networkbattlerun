using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    [SerializeField] bool _isDebug;
    [SerializeField] float _range;
    [SerializeField] float _errorRange;
    [SerializeField] float _grappleSpeed = 20;
    [SerializeField] LayerMask _roofLayer;
    Rigidbody2D _rb;
    Vector3 _hitPoint;
    Vector3 _playerPos;

    LineRenderer _lr;
    DistanceJoint2D _dj;
    [SerializeField] GameObject _grappleTip;
    GameObject _currentGrappleTip;
    Vector2 _grappleVec;
    PhotonView _photonView;

    bool _currentGrapple;

    float? _lastSpeedX;
    float? _lastSpeedY;

    public bool CurrentGrapple => _currentGrapple;

    void Start()
    {
        _lr = GetComponent<LineRenderer>();
        _dj = GetComponent<DistanceJoint2D>();
        _rb = GetComponent<Rigidbody2D>();
        _photonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))//GetButtonに変えたい
        {
            Debug.Log("押された");
            _photonView.RPC("StartGrapple", RpcTarget.All);
        }

        if (_currentGrapple)
        {
            DrawLine();
        }

        if (_currentGrapple && OverGrapple(_grappleVec))
        {
            _currentGrapple = false;
            FinishGrapple();
        }

        if (_isDebug)
        {
            if (_rb.velocity.x >= 0)
            {
                Debug.DrawRay(transform.position, (Vector2.up + Vector2.right) * _range);
            }
            else
            {
                Debug.DrawRay(transform.position, (Vector2.up - Vector2.right) * _range);
            }
        }

    }

    void FixedUpdate()
    {
        if (_currentGrapple)
        {
            DrawLine();
            GrappleMove();
        }
    }

    [PunRPC]
    void StartGrapple()
    {
        RaycastHit2D hit;
        if (_rb.velocity.x >= 0)
        {
            hit = Physics2D.Raycast(transform.position, transform.up + transform.right, _range, _roofLayer);
            _grappleVec = Vector2.right;
        }
        else
        {
            hit = Physics2D.Raycast(transform.position, transform.up - transform.right, _range, _roofLayer);
            _grappleVec = Vector2.left;
        }
        _hitPoint = hit.point;
        float distance = Vector2.Distance(_hitPoint, transform.position) - _errorRange;

        if (hit && distance > 0.5f)
        {
            _playerPos = transform.position;
            _currentGrappleTip = Instantiate(_grappleTip, _hitPoint, Quaternion.identity);
            _dj.enabled = true;
            _lr.enabled = true;

            _dj.connectedBody = _currentGrappleTip.GetComponent<Rigidbody2D>();
            _dj.distance = distance;

            _lr.positionCount = 2;

            _currentGrapple = true;
        }
    }

    void GrappleMove()
    {
        Vector2 vec = new Vector2(_grappleVec.x * _grappleSpeed, _rb.velocity.y);
        _rb.velocity = vec;
    }

    [PunRPC]
    void FinishGrapple()
    {
        _dj.enabled = false;
        _lr.enabled = false;
        Destroy(_currentGrappleTip);
    }

    void DrawLine()
    {
        _lr.SetPosition(0, _hitPoint);
        _lr.SetPosition(1, transform.position);
    }

    bool OverGrapple(Vector2 vec)
    {
        bool isRight = vec.x > 0;
        if (isRight)
        {
            if (transform.position.x > _hitPoint.x + ((_hitPoint.x - _playerPos.x) / 2))
            {
                return true;
            }
        }
        else
        {
            if (transform.position.x < _hitPoint.x - ((_playerPos.x - _hitPoint.x) / 2))
            {
                return true;
            }
        }

        if(transform.position.y + 0.51f > _hitPoint.y)
        {
            return true;
        }
        return false;
    }
}