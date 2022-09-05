using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AccelerationArea : MonoBehaviour
{
    [SerializeField] Vector2Int _area;
    bool _isCheck;

    private void FixedUpdate()
    {
        _isCheck = CheckArea();
    }
    private void OnValidate()
    {
        this.transform.localScale = new Vector3(_area.x, _area.y, 1);
    }
    bool CheckArea()
    {
        var check = Physics2D.OverlapBoxAll(this.transform.position, new Vector2(_area.x, _area.y), 0);

        foreach(var go in check)
        {
            var view = go.gameObject.GetPhotonView();

            if(view && view.IsMine)
            {
                view.RPC("IncreaseAccelerationValue", RpcTarget.All); //‘Sˆõ‚É’m‚ç‚¹‚é
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
