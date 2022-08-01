using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AccelerationArea : MonoBehaviour
{
    [SerializeField] Vector2Int _area;
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
            //var p = go.GetComponent<PlayerMove2D>();
            var view = go.gameObject.GetPhotonView();

            if(view && view.IsMine)
            {
                //p.AddAccelerationValue();
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
