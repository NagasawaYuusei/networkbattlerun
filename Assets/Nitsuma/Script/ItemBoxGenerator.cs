using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBoxGenerator : MonoBehaviour
{
    [SerializeField,Tooltip("��������܂ł̎���")]
    float _interval = 3f;
    [SerializeField, Tooltip("��������Prefab")]
    GameObject _prefab;

    float _time;
    // Update is called once per frame
    void Update()
    {
        if(transform.childCount >= 1) { return; }
        _time += Time.deltaTime;
        if(_time >= _interval)
        {
            var obj = Instantiate(_prefab,this.transform.position ,Quaternion.identity);
            obj.transform.parent = this.transform;
        }
    }
}
