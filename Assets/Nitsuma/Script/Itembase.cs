using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Itembase : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GetItem();
        Destroy(gameObject);
    }

    protected abstract void GetItem();
}
