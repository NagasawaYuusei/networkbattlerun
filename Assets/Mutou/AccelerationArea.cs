using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationArea : MonoBehaviour
{
    [SerializeField] Vector2Int _area;
    [SerializeField] float _multiplication = 0.1f;
    [SerializeField] float _addValue = 3f;
    bool _isCheck;

    private void Update()
    {
        _isCheck = CheckArea();
    }
    bool CheckArea()
    {
        var check = Physics2D.OverlapBoxAll(this.transform.position, new Vector2(_area.x, _area.y), 0);

        foreach(var go in check)
        {
            var p = go.GetComponent<PlayerMove2D>();

            if(p)
            {
                p.AddAccelerationValue(_addValue * _multiplication * Time.deltaTime);
                return true;
            }
        }

        return false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = _isCheck? Color.green : Color.red;

        Gizmos.DrawWireCube(this.transform.position, new Vector2(_area.x, _area.y));
    }
}
