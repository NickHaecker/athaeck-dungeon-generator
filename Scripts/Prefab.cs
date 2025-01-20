using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Prefab", menuName = "Data/Generator/Prefab")]
[Serializable]
public class Prefab : ScriptableObject
{
    public GameObject Model;
    public int Weight;
    public List<Prefab> Append = new List<Prefab>();
}
