using System.Linq;
using UnityEngine;

/// <summary>
/// 先頭のプレイヤーをカメラが追うためのコンポーネント
/// </summary>
public class CameraSwitcher : MonoBehaviour
{
    /// <summary>カメラの Follow Target となる Transform</summary>
	[SerializeField] Transform _target;
    MoveDirection _moveDirection = MoveDirection.Right;

    public MoveDirection MoveDirection { get => _moveDirection;}

    private void Update()
    {
        var players = FindObjectsOfType<PlayerMoveDirection>();

        //プレイヤーを探して、誰もいなければreturn
        if(players.Length <= 0) return;

        //CameraSwicherに保存された移動方向と同じ方向に進むプレイヤーの条件で再検索
        players = players.Where(x => x.MoveDirection == _moveDirection).ToArray();

        if (players.Length <= 0) return;

        if (_moveDirection == MoveDirection.Right)
        {
            _target.position = players.OrderByDescending(x => x.transform.position.x).FirstOrDefault().transform.position;
        }
        else
        {
            _target.position = players.OrderBy(x => x.transform.position.x).FirstOrDefault().transform.position;
        }

        // カメラは先頭のプレイヤーを追う
        if (_target)
        {
            _target.position = _target.position;
        }
    }
    
    /// <summary>
    /// 移動方向を変更する
    /// </summary>
    public void ChangeDirection()
    {
        if(_moveDirection == MoveDirection.Left)
        {
            _moveDirection = MoveDirection.Right;
        }
        else
        {
            _moveDirection = MoveDirection.Left;
        }
    }
}
