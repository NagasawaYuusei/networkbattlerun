using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    [SerializeField] bool _isDebug;
    [SerializeField] float _range;
    [SerializeField] float _errorRange;
    [SerializeField] LayerMask _roofLayer;
    Rigidbody2D _rb;
    Vector3 _hitPoint;

    LineRenderer _lr;
    DistanceJoint2D _dj;
    [SerializeField] GameObject _grappleTip;
    GameObject _currentGrappleTip;

    bool _currentGrapple;

    void Start()
    {
        _lr = GetComponent<LineRenderer>();
        _dj = GetComponent<DistanceJoint2D>();
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))//GetButtonに変えたい
        {
            StartGrapple();
        }

        if (_currentGrapple && Input.GetKey(KeyCode.LeftControl))
        {
            DrawLine();
        }

        if(_currentGrapple && Input.GetKeyUp(KeyCode.LeftControl))
        {
            _currentGrapple = false;
            FinishGrapple();
        }

        if(_isDebug)
        {
            if(_rb.velocity.x >= 0)
            {
                Debug.DrawRay(transform.position, (Vector2.up + Vector2.right) * _range);
            }
            else
            {
                Debug.DrawRay(transform.position, (Vector2.up - Vector2.right) * _range);
            }
        }

    }

    void StartGrapple()
    {
        RaycastHit2D hit;
        if (_rb.velocity.x >= 0)
        {
            hit = Physics2D.Raycast(transform.position, transform.up + transform.right, _range, _roofLayer);
        }
        else
        {
            hit = Physics2D.Raycast(transform.position, transform.up - transform.right, _range, _roofLayer);
        }
        _hitPoint = hit.point;
        float distance =  Vector2.Distance(_hitPoint, transform.position) - _errorRange; 

        if(hit)
        {
            _currentGrappleTip = Instantiate(_grappleTip, _hitPoint, Quaternion.identity);
            _dj.enabled = true;
            _lr.enabled = true;

            _dj.connectedBody = _currentGrappleTip.GetComponent<Rigidbody2D>();
            _dj.distance = distance;

            _lr.positionCount = 2;

            _currentGrapple = true;
        }
    }

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
}