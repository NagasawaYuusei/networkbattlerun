using System.Linq;
using UnityEngine;

/// <summary>
/// 先頭のプレイヤーをカメラが追うためのコンポーネント
/// </summary>
public class CameraSwitcher : MonoBehaviour
{
    /// <summary>カメラの Follow Target となる Transform</summary>
	[SerializeField] Transform _target;
    GameObject[] _players;

    private void Update()
    {
        _players = GameObject.FindGameObjectsWithTag("Player");
        GameObject firstPlayer = _players.OrderByDescending(x => x.transform.position.x).FirstOrDefault();

        // カメラは先頭のプレイヤーを追う
        if (firstPlayer)
        {
            Vector3 position = _target.position;
            // カメラは戻さない
            position.x = Mathf.Max(firstPlayer.transform.position.x, _target.position.x);
            _target.position = position;
        }
    }
}
