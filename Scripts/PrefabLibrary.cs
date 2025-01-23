using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DungeonGenerator
{
    [CreateAssetMenu(fileName = "PrefabLibrary", menuName = "Data/Generator/Library")]
    [Serializable]
    public class PrefabLibrary : ScriptableObject
    {
        public List<Prefab> Straights = new List<Prefab>();
        public List<Prefab> Lefts = new List<Prefab>();
        public List<Prefab> Rights = new List<Prefab>();
        public List<Prefab> BiSplits = new List<Prefab>();
        public List<Prefab> TriSplits = new List<Prefab>();
        public List<End> Ends = new List<End>();
        public List<Prefab> Stairs = new List<Prefab>();

        public List<Room> Rooms = new List<Room>();
    }
}