using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Room", menuName = "Data/Generator/Room")]
[Serializable]
public class Room : Prefab
{
    public Vector2 Size;
}
