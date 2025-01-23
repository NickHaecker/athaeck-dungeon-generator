using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DungeonGenerator
{
    [CreateAssetMenu(fileName = "End", menuName = "Data/Generator/End")]
    [Serializable]
    public class End : Prefab
    {
        public List<Prefab> Prepend = new List<Prefab>();
    }
}