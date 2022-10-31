using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Itembase : MonoBehaviour
{
    [SerializeField]
    Sprite _sprite;

    public Sprite Sprite { get => _sprite; set => _sprite = value; }

    public abstract void Use(Vector3 pos);
}
