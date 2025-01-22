using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;


namespace ConnectingMinds
{
    public class DungeonGenerator : EditorWindow
    {
        private PrefabLibrary _library;

        public int straightChance = 15;
        public int leftChance = 15;
        public int rightChance = 15;
        public int _biSplitChance = 15;
        public int _triSplitChance = 10;
        public int roomChance = 25;
        public int stairChance = 1;


        public int dungeonSize = 200;
        private int remainingModules;
        public float currentHeight = 0f;
        private int localHeight;

        private Dictionary<Vector2, bool> occupiedCells = new Dictionary<Vector2, bool>();
        private List<Prefab> pathPrefabs;

        private Dictionary<GameObject, Vector2> roomSizes;

        private Transform dungeonParent;
        private int maxPathLength;
        private static bool isGeneratorOpenedFromToolbar = false;

        private string _defaultFolder = "";

        private string _name = "";

        private float _padding = 0.125f;


        private Vector2 scrollPosition;

        [MenuItem("Tools/Dungeon Generator")]
        public static void ShowWindow()
        {
            isGeneratorOpenedFromToolbar = true;
            GetWindow<DungeonGenerator>("Dungeon Generator");
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height));

            GUILayout.Label("Dungeon Generator Settings", EditorStyles.boldLabel);

            GUILayout.Label("Drag and Drop Library", EditorStyles.boldLabel);

            _library = (PrefabLibrary)EditorGUILayout.ObjectField("Library:", _library, typeof(PrefabLibrary), false);


            dungeonSize = EditorGUILayout.IntField("Dungeon Size", dungeonSize);

            currentHeight = EditorGUILayout.FloatField("Starting Height", currentHeight);

            GUILayout.Label("Straight Prefabs", EditorStyles.boldLabel);

            if (GUILayout.Button("Add Prefab"))
            {
                AddPrefabSlot(ref _library.Straights);
            }

            if (_library != null && _library.Straights != null)
            {
                for (int i = 0; i < _library.Straights.Count; i++)
                {
                    _library.Straights[i].Weight = straightChance;

                    EditorGUILayout.BeginHorizontal();

                    _library.Straights[i].Model = (GameObject)EditorGUILayout.ObjectField($"Straight Prefab {i + 1}", _library.Straights[i].Model, typeof(GameObject), false);

                    _library.Straights[i].Weight = EditorGUILayout.IntField("Straight Spawn Chance", _library.Straights[i].Weight);

                    if (GUILayout.Button("Remove", GUILayout.Width(70)))
                    {
                        RemovePrefabSlot(ref _library.Straights, i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            GUILayout.Label("Left Prefabs", EditorStyles.boldLabel);
            if (GUILayout.Button("Add Left Prefab"))
            {
                AddPrefabSlot(ref _library.Lefts);
            }

            if (_library != null && _library.Lefts != null)
            {
                for (int i = 0; i < _library.Lefts.Count; i++)
                {
                    _library.Lefts[i].Weight = leftChance;

                    EditorGUILayout.BeginHorizontal();

                    _library.Lefts[i].Model = (GameObject)EditorGUILayout.ObjectField($"Left Prefab {i + 1}", _library.Lefts[i].Model, typeof(GameObject), false);

                    _library.Lefts[i].Weight = EditorGUILayout.IntField("Left Spawn Chance", _library.Lefts[i].Weight);

                    if (GUILayout.Button("Remove", GUILayout.Width(70)))
                    {
                        RemovePrefabSlot(ref _library.Lefts, i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            GUILayout.Label("Right Prefabs", EditorStyles.boldLabel);
            if (GUILayout.Button("Add Right Prefab"))
            {
                AddPrefabSlot(ref _library.Rights);
            }

            if (_library != null && _library.Rights != null)
            {
                for (int i = 0; i < _library.Rights.Count; i++)
                {
                    _library.Rights[i].Weight = rightChance;

                    EditorGUILayout.BeginHorizontal();

                    _library.Rights[i].Model = (GameObject)EditorGUILayout.ObjectField($"Right Prefab {i + 1}", _library.Rights[i].Model, typeof(GameObject), false);

                    _library.Rights[i].Weight = EditorGUILayout.IntField("Right Spawn Chance", _library.Rights[i].Weight);

                    if (GUILayout.Button("Remove", GUILayout.Width(70)))
                    {
                        RemovePrefabSlot(ref _library.Rights, i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            GUILayout.Label("Double Split Prefabs", EditorStyles.boldLabel);
            if (GUILayout.Button("Add Double Split Prefab"))
            {
                AddPrefabSlot(ref _library.BiSplits);
            }

            if (_library != null && _library.BiSplits != null)
            {
                for (int i = 0; i < _library.BiSplits.Count; i++)
                {
                    _library.BiSplits[i].Weight = _biSplitChance;

                    EditorGUILayout.BeginHorizontal();

                    _library.BiSplits[i].Model = (GameObject)EditorGUILayout.ObjectField($"Double Split Prefab {i + 1}", _library.BiSplits[i].Model, typeof(GameObject), false);

                    _library.BiSplits[i].Weight = EditorGUILayout.IntField("Double Split Spawn Chance", _library.BiSplits[i].Weight);

                    if (GUILayout.Button("Remove", GUILayout.Width(70)))
                    {
                        RemovePrefabSlot(ref _library.BiSplits, i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            GUILayout.Label("Triple Split Prefabs", EditorStyles.boldLabel);
            if (GUILayout.Button("Add Triple Split Prefab"))
            {
                AddPrefabSlot(ref _library.TriSplits);
            }

            if (_library != null && _library.TriSplits != null)
            {
                for (int i = 0; i < _library.TriSplits.Count; i++)
                {
                    _library.TriSplits[i].Weight = _triSplitChance;

                    EditorGUILayout.BeginHorizontal();

                    _library.TriSplits[i].Model = (GameObject)EditorGUILayout.ObjectField($"Triple Split Prefab {i + 1}", _library.TriSplits[i].Model, typeof(GameObject), false);

                    _library.TriSplits[i].Weight = EditorGUILayout.IntField("Tripple Split Spawn Chance", _library.TriSplits[i].Weight);

                    if (GUILayout.Button("Remove", GUILayout.Width(70)))
                    {
                        RemovePrefabSlot(ref _library.TriSplits, i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            GUILayout.Label("Room Prefabs", EditorStyles.boldLabel);

            if (GUILayout.Button("Add Room Prefabs"))
            {
                AddPrefabSlot(ref _library.Rooms);
            }

            if (_library != null && _library.Rooms != null)
            {
                for (int i = 0; i < _library.Rooms.Count; i++)
                {
                    _library.Rooms[i].Weight = roomChance;

                    EditorGUILayout.BeginHorizontal();

                    _library.Rooms[i].Model = (GameObject)EditorGUILayout.ObjectField($"Room Prefab {i + 1}", _library.Rooms[i].Model, typeof(GameObject), false);

                    EditorGUILayout.BeginHorizontal();
                    _library.Rooms[i].Size.x = Mathf.Max(1, EditorGUILayout.FloatField(_library.Rooms[i].Size.x, GUILayout.Width(50)));
                    _library.Rooms[i].Size.y = Mathf.Max(1, EditorGUILayout.FloatField(_library.Rooms[i].Size.y, GUILayout.Width(50)));

                    _library.Rooms[i].Weight = EditorGUILayout.IntField("Room Spawn Chance", _library.Rooms[i].Weight);

                    EditorGUILayout.EndHorizontal();

                    if (GUILayout.Button("Remove", GUILayout.Width(70)))
                    {
                        RemovePrefabSlot(ref _library.Rooms, i);
                        continue;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            GUILayout.Label("Stair Prefabs", EditorStyles.boldLabel);
            if (GUILayout.Button("Add Stair Prefabs"))
            {
                AddPrefabSlot(ref _library.Stairs);
            }

            if (_library != null && _library.Stairs != null)
            {
                for (int i = 0; i < _library.Stairs.Count; i++)
                {
                    _library.Stairs[i].Weight = stairChance;

                    EditorGUILayout.BeginHorizontal();

                    _library.Stairs[i].Model = (GameObject)EditorGUILayout.ObjectField($"Stair Prefab {i + 1}", _library.Stairs[i].Model, typeof(GameObject), false);

                    _library.Stairs[i].Weight = EditorGUILayout.IntField("Stairs Spawn Chance", _library.Stairs[i].Weight);

                    if (GUILayout.Button("Remove", GUILayout.Width(70)))
                    {
                        RemovePrefabSlot(ref _library.Stairs, i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }


            GUILayout.Label("End Prefabs", EditorStyles.boldLabel);
            if (GUILayout.Button("Add End Prefabs"))
            {
                AddPrefabSlot(ref _library.Ends);
            }

            if (_library != null && _library.Ends != null)
            {
                for (int i = 0; i < _library.Ends.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    _library.Ends[i].Model = (GameObject)EditorGUILayout.ObjectField($"End Prefab {i + 1}", _library.Ends[i].Model, typeof(GameObject), false);

                    _library.Ends[i].Weight = EditorGUILayout.IntField("Ends Spawn Chance", _library.Ends[i].Weight);

                    if (GUILayout.Button("Remove", GUILayout.Width(70)))
                    {
                        RemovePrefabSlot(ref _library.Ends, i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }


            _name = EditorGUILayout.TextField("Dungeon Name:", _name);

            if (GUILayout.Button("Generate Dungeon"))
            {
                GenerateDungeon();
            }

            if (GUILayout.Button("Delete Dungeon"))
            {
                ClearDungeon();
            }

            EditorGUILayout.EndScrollView();
        }

        private void OnEnable()
        {
            if (!isGeneratorOpenedFromToolbar)
            {
                return;
            }

            isGeneratorOpenedFromToolbar = false;

            if (!TagExists("DungeonPath"))
            {
                Debug.Log("Tag 'DungeonPath' does not exist. Creating it now.");
                AddTag("DungeonPath");
            }
            if (_library == null || _library.Straights.Count == 0)
            {
                AddPrefabSlot(ref _library.Straights);
            }

            if (_library == null || _library.Lefts.Count == 0)
            {
                AddPrefabSlot(ref _library.Lefts);
            }

            if (_library == null || _library.Rights.Count == 0)
            {
                AddPrefabSlot(ref _library.Rights);
            }

            if (_library == null || _library.BiSplits.Count == 0)
            {
                AddPrefabSlot(ref _library.BiSplits);
            }

            if (_library == null || _library.TriSplits.Count == 0)
            {
                AddPrefabSlot(ref _library.TriSplits);
            }

            if (_library == null || _library.Rooms.Count == 0)
            {
                AddPrefabSlot(ref _library.Rooms);
            }

        }

        private bool TagExists(string tagName)
        {
            return UnityEditorInternal.InternalEditorUtility.tags.Contains(tagName);
        }

        private void AddTag(string tagName)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                SerializedProperty tag = tagsProp.GetArrayElementAtIndex(i);
                if (tag.stringValue == tagName)
                {
                    return;
                }
            }

            tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
            SerializedProperty newTag = tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1);
            newTag.stringValue = tagName;

            tagManager.ApplyModifiedProperties();

            Debug.Log($"Tag '{tagName}' successfully created.");
        }

        private void AddPrefabSlot<T>(ref List<T> prefabArray) where T : Prefab
        {
            if (prefabArray == null)
            {
                prefabArray = new List<T>();
            }
            prefabArray.Add(null);
        }
        private void RemovePrefabSlot<T>(ref List<T> prefabArray, int index) where T: Prefab
        {
            prefabArray.RemoveAt(index);
        }

        private void GenerateDungeon()
        {
            if (_library == null || _library.Straights == null || _library.Lefts == null || _library.Rights == null || _library.BiSplits == null || _library.TriSplits == null)
            {
                Debug.LogError("Please assign all pathbuilding prefabs.");
                return;
            }

            InitializeRoomSizes();
            InitializePathPrefabs();
            occupiedCells.Clear();

            remainingModules = dungeonSize;

            Vector2 startPosition = Vector2.zero;
            int startRotation = 0;

            GeneratePath(startPosition, startRotation, 10, localHeight, pathPrefabs);

            Debug.Log($"Dungeon generation complete. Total modules placed: {occupiedCells.Count} / {dungeonSize}");

            Vector2 dimensions = CalculateDungeonDimensions();
            Debug.Log($"Generated dungeon with dimensions: {dimensions.x}x{dimensions.y}");
        }

        private void GeneratePath(Vector2 startPosition, int startRotation, int maxPathLength, int localHeight, List<Prefab> weights)
        {
            Vector2 currentPosition = startPosition;
            int currentRotation = startRotation;
            int yOffset = 0;

            Prefab nextPrefab = SelectPrefabWeighted(weights);

            for (int i = 0; i < maxPathLength; i++)
            {
                Vector2 nextPosition = currentPosition + GetDirectionFromRotation(currentRotation);

                Prefab end = _library.Ends.Find(e => e.Prepend.Contains(nextPrefab));

                if (CanPlacePrefab(nextPrefab.Model, nextPosition, currentRotation))
                {
                    if (yOffset > 0)
                    {
                        nextPosition.y += yOffset;
                        yOffset = 0;
                    }

                    else if (_library.BiSplits.Contains(nextPrefab))
                    {
                        PlacePrefab(nextPrefab, ref currentPosition, ref currentRotation, ref localHeight);

                        GenerateBiSplit(currentPosition, currentRotation, 10, localHeight,nextPrefab.Append);
                        break;
                    }
                    else if (_library.TriSplits.Contains(nextPrefab))
                    {
                        PlacePrefab(nextPrefab, ref currentPosition, ref currentRotation, ref localHeight);

                        GenerateTriSplit(currentPosition, currentRotation, 10, localHeight,nextPrefab.Append);
                        break;
                    }
                    else if (_library.Rooms.Any(room => room == nextPrefab))
                    {

                        Room matchingRoom = _library.Rooms.First(room => room == nextPrefab);

                        float skipDistance = matchingRoom.Size.y - 1;

                        PlacePrefab(nextPrefab, ref currentPosition, ref currentRotation, ref localHeight);


                        currentPosition += GetDirectionFromRotation(currentRotation) * skipDistance;
                    }

                    else if(_library.Stairs.Contains(nextPrefab))
                    {
                        int skipDistance = 1;
                        PlacePrefab(nextPrefab, ref currentPosition, ref currentRotation, ref localHeight);


                        currentPosition += GetDirectionFromRotation(currentRotation) * skipDistance;
                    }
                    else
                    {
                        PlacePrefab(nextPrefab, ref currentPosition, ref currentRotation, ref localHeight);

                    }
                }
                else
                {
                    
                    if (remainingModules <= 0)
                    {
                       
                        PlacePrefab(end, ref currentPosition, ref currentRotation, ref localHeight);
                        break; // End the path generation
                    }

                    else if (maxPathLength <= 0)
                    {
                        PlacePrefab(end, ref currentPosition, ref currentRotation, ref localHeight);
                        break; // End the path generation
                    }
                    else
                    {
                        continue;
                    }

                }

                if (remainingModules <= 0)
                {
                    Debug.Log($"Path generation stopped at {currentPosition} (Last module placed: {nextPrefab.Model.name})");
                    PlacePrefab(end, ref currentPosition, ref currentRotation, ref localHeight);
                    break;
                }

                if (maxPathLength <= 0)
                {
                    Debug.Log($"Path generation stopped at {currentPosition} (Last module placed: {nextPrefab.Model.name})");
                    PlacePrefab(end, ref currentPosition, ref currentRotation, ref localHeight);
                    break;
                }

                //currentPosition += new Vector2(_padding,_padding);

                nextPrefab = InitPathPrefabs(nextPrefab.Append);
            }
            maxPathLength = 10;
        }
        private void RefreshRoomPrefabs()
        {
            for (int i = 0; i < pathPrefabs.Count; i++)
            {
                if (pathPrefabs[i].Weight == roomChance)
                {
                    pathPrefabs[i] = GetRandomRoomPrefab();
                }
            }
        }

        private Prefab GetRandomRoomPrefab()
        {
            if (_library == null || _library.Rooms.Count == 0)
            {
                Debug.LogError("No room prefabs assigned!");
                return null;
            }
            return _library.Rooms[Random.Range(0, _library.Rooms.Count)];
        }



        private void GenerateBiSplit(Vector2 splitPosition, int splitRotation, int remainingModules, int localHeight,List<Prefab> appends)
        {
            Debug.Log($"[Split1] Generating Split1 at position {splitPosition} with rotation {splitRotation}");

            List<Prefab> weights = appends;

            int branchLength = 10;

            int leftRotation = (splitRotation - 90 + 360) % 360;
            Vector2 leftStart = splitPosition + GetDirectionFromRotation(leftRotation);
            Debug.Log($"[Split1] Left branch starts at {leftStart} with rotation {leftRotation}");

            foreach (Prefab prefab in appends)
            {
                if (CanPlacePrefab(prefab.Model, leftStart, leftRotation))
                {
                    Debug.Log($"[Split1] Placing left branch module at {leftStart}");
                    PlacePrefab(prefab, ref splitPosition, ref leftRotation, ref localHeight, leftStart);

                    Vector2 nextLeftPosition = splitPosition + GetDirectionFromRotation(leftRotation);


                    GeneratePath(nextLeftPosition, leftRotation, branchLength, localHeight, weights);
                    break;
                }
                else
                {
                    Debug.Log($"[Split1] Left branch failed to generate at {leftStart}. Skipping.");
                }
            }

            int rightRotation = (splitRotation + 90) % 360;
            Vector2 rightStart = splitPosition + GetDirectionFromRotation(rightRotation);
            Debug.Log($"[Split1] Right branch starts at {rightStart} with rotation {rightRotation}");

            foreach (Prefab prefab in appends)
            {
                if (CanPlacePrefab(prefab.Model, rightStart, rightRotation))
                {
                    PlacePrefab(prefab, ref splitPosition, ref rightRotation, ref localHeight, rightStart);

                    Vector2 nextRightPosition = splitPosition + GetDirectionFromRotation(rightRotation);

                    GeneratePath(nextRightPosition, rightRotation, branchLength, localHeight, weights);
                    break;
                }
                else
                {
                    Debug.Log($"[Split1] Right branch failed to generate at {rightStart}. Skipping.");
                }
            }
        }

        private void GenerateTriSplit(Vector2 splitPosition, int splitRotation, int remainingModules, int localHeight, List<Prefab> appends)
        {
            Debug.Log($"[Split2] Generating Split2 at position {splitPosition} with rotation {splitRotation}");

            List<Prefab> weights = appends;

            int branchLength = 10;

            int leftRotation = (splitRotation - 90 + 360) % 360;
            Vector2 leftStart = splitPosition + GetDirectionFromRotation(leftRotation);
            Debug.Log($"[Split2] Left branch starts at {leftStart} with rotation {leftRotation}");

            foreach (Prefab prefab in appends)
            {
                if (CanPlacePrefab(prefab.Model, leftStart, leftRotation))
                {
                    Debug.Log($"[Split2] Placing left branch module at {leftStart}");
                    PlacePrefab(prefab, ref splitPosition, ref leftRotation, ref localHeight, leftStart);

                    Vector2 nextLeftPosition = splitPosition + GetDirectionFromRotation(leftRotation);

                    GeneratePath(nextLeftPosition, leftRotation, branchLength, localHeight, weights);
                    break;
                }
                else
                {
                    Debug.Log($"[Split2] Left branch failed to generate at {leftStart}. Skipping.");
                }
            }
            Vector2 straightStart = splitPosition + GetDirectionFromRotation(splitRotation);
            Debug.Log($"[Split2] Straight branch starts at {straightStart} with rotation {splitRotation}");

            foreach (Prefab prefab in appends)
            {
                if (CanPlacePrefab(prefab.Model, straightStart, splitRotation))
                {
                    Debug.Log($"[Split2] Placing straight branch module at {straightStart}");
                    PlacePrefab(prefab, ref splitPosition, ref splitRotation, ref localHeight, straightStart);

                    Vector2 nextStraightPosition = splitPosition + GetDirectionFromRotation(splitRotation);

                    GeneratePath(nextStraightPosition, splitRotation, branchLength, localHeight, weights);
                    break;
                }
                else
                {
                    Debug.Log($"[Split2] Straight branch failed to generate at {straightStart}. Skipping.");
                }
            }

            int rightRotation = (splitRotation + 90) % 360;
            Vector2 rightStart = splitPosition + GetDirectionFromRotation(rightRotation);
            Debug.Log($"[Split2] Right branch starts at {rightStart} with rotation {rightRotation}");

            foreach (Prefab prefab in appends)
            {
                if (CanPlacePrefab(prefab.Model, rightStart, rightRotation))
                {
                    Debug.Log($"[Split2] Placing right branch module at {rightStart}");
                    PlacePrefab(prefab, ref splitPosition, ref rightRotation, ref localHeight, rightStart); // Fixed position

                    Vector2 nextRightPosition = splitPosition + GetDirectionFromRotation(rightRotation);

                    GeneratePath(nextRightPosition, rightRotation, branchLength, localHeight, weights);
                    break;
                }
                else
                {
                    Debug.Log($"[Split2] Right branch failed to generate at {rightStart}. Skipping.");
                }
            }
        }

        private void ClearDungeon()
        {
            foreach (var obj in GameObject.FindGameObjectsWithTag("DungeonPath"))
            {
                DestroyImmediate(obj);
            }
            occupiedCells.Clear();
            Debug.Log("Dungeon cleared.");
        }

        private Prefab InitPathPrefabs(List<Prefab> prefabs)
        {
            return SelectPrefabWeighted(prefabs);
        }

        private void InitializePathPrefabs()
        {
            List<Prefab> weightesPaths = new List<Prefab>();


            weightesPaths.AddRange(_library.Rooms);
            weightesPaths.AddRange(_library.Lefts);
            weightesPaths.AddRange(_library.Rights);
            weightesPaths.AddRange(_library.BiSplits);
            weightesPaths.AddRange(_library.TriSplits);
            weightesPaths.AddRange(_library.Stairs);
            weightesPaths.AddRange(_library.Straights);

            pathPrefabs = weightesPaths;
        }

        private Prefab SelectPrefabWeighted(List<Prefab> weights)
        {
            int totalWeight = 0;
            foreach (var pathPrefab in weights)
            {
                totalWeight += pathPrefab.Weight;
            }

            int randomValue = Random.Range(0, totalWeight);
            foreach (var pathPrefab in weights)
            {
                if (randomValue < pathPrefab.Weight)
                {
                    return pathPrefab;
                }
                randomValue -= pathPrefab.Weight;
            }
            return null;
        }

        private bool CanPlacePrefab(GameObject prefab, Vector2 position, int currentRotation)
        {
            if (roomSizes.ContainsKey(prefab))
            {
                Vector2 roomSize = roomSizes[prefab];
                List<Vector2> occupiedRoomCells = GetOccupiedCells(position, roomSize, currentRotation);

                Debug.Log($"[CanPlacePrefab] Checking room {prefab.name} at position {position} with size {roomSize} and rotation {currentRotation}");

                foreach (var cell in occupiedRoomCells)
                {
                    if (occupiedCells.ContainsKey(cell))
                    {
                        Debug.LogWarning($"[CanPlacePrefab] Cell {cell} is occupied.");
                        return false;
                    }
                    else
                    {
                        Debug.Log($"[CanPlacePrefab] Cell {cell} is free.");
                    }
                }
            }

            if (occupiedCells.ContainsKey(position))
            {
                Debug.LogWarning($"[CanPlacePrefab] Position {position} is occupied by another module.");
                return false;
            }
            else
            {
                Debug.Log($"[CanPlacePrefab] Position {position} is free.");
            }
            return true;
        }

        private void PlacePrefab(Prefab prefab, ref Vector2 position, ref int currentRotation, ref int localHeight, Vector2? fixedPosition = null)
        {
            Debug.Log($"remaining modules  {remainingModules}");
            remainingModules--; // Decrement for the placed module
            maxPathLength--;

            if (dungeonParent == null)
            {
                GameObject parentObject = GameObject.Find(_name);
                if (parentObject == null)
                {
                    parentObject = new GameObject(_name);
                }
                dungeonParent = parentObject.transform;
            }

            Vector2 placementPosition = fixedPosition ?? (position + GetDirectionFromRotation(currentRotation));

            int prefabRotation = currentRotation;
            if (_library.Lefts.Contains(prefab))
            {
                currentRotation = (currentRotation - 90 + 360) % 360;
            }
            else if (_library.Rights.Contains(prefab))
            {
                currentRotation = (currentRotation + 90) % 360;
            }
            else if (_library.Straights.Contains(prefab))
            {


            }

            Vector3 worldPosition = new Vector3(placementPosition.x * 2, localHeight, placementPosition.y * 2);
            if (_library.Stairs.Contains(prefab))
            {
                localHeight += 2;


                List<Vector2> stairsOccupiedCells = GetOccupiedCells(placementPosition, new Vector2(1, 2), prefabRotation);
                foreach (var cell in stairsOccupiedCells)
                {
                    occupiedCells[cell] = true;
                }
            }
            Quaternion rotation = Quaternion.Euler(0, prefabRotation, 0);
            GameObject instance = Instantiate(prefab.Model, worldPosition, rotation);
            instance.transform.SetParent(dungeonParent);
            instance.tag = "DungeonPath";

            occupiedCells[placementPosition] = true;

            if (roomSizes.ContainsKey(prefab.Model))
            {
                Vector2 roomSize = roomSizes[prefab.Model];
                List<Vector2> occupiedRoomCells = GetOccupiedCells(placementPosition, roomSize, prefabRotation);

                foreach (var cell in occupiedRoomCells)
                {
                    occupiedCells[cell] = true; // Mark the cell as occupied
                }

                Debug.Log($"Placed room {prefab.name} at {placementPosition} with rotation {prefabRotation}. Occupied cells: {string.Join(", ", occupiedRoomCells)}");

                RefreshRoomPrefabs();
            }
            else
            {
                Debug.Log($"Placed {prefab.name} at {placementPosition} with rotation {prefabRotation}");
            }

            if (!fixedPosition.HasValue)
            {
                position = placementPosition; // Update the original position reference only when no fixed position is provided
            }

            Debug.Log($"Placed {prefab.name} at {placementPosition} with rotation {prefabRotation}. New direction: {GetDirectionFromRotation(currentRotation)}");
        }


        private List<Vector2> GetOccupiedCells(Vector2 center, Vector2 roomSize, int rotation)
        {
            List<Vector2> occupiedCells = new List<Vector2>();
            float width = roomSize.x;
            float height = roomSize.y;

            for (float y = 0; y < height; y++) 
            {
                for (float x = -width / 2; x <= width / 2; x++) 
                {

                    Vector2 offset = new Vector2(-x, height - y - 1);
                    offset = RotateOffset(offset, rotation); 
                    occupiedCells.Add(center + offset); 
                }
            }

            return occupiedCells;
        }

        private Vector2 GetDirectionFromRotation(int rotation)
        {
            switch (rotation % 360)
            {
                case 0: return Vector2.up + new Vector2(0,_padding);
                case 90: return Vector2.right + new Vector2(_padding,0);
                case 180: return Vector2.down - new Vector2(0,_padding);
                case 270: return Vector2.left - new Vector2(_padding,0);
                default: return Vector2.zero + new Vector2(_padding,_padding);
            }
        }

        private void InitializeRoomSizes()
        {
            roomSizes = new Dictionary<GameObject, Vector2>();

            foreach (var roomInfo in _library.Rooms)
            {
                if (roomInfo.Model != null)
                {
                    roomSizes[roomInfo.Model] = roomInfo.Size;
                }
            }
        }

        private Vector2 RotateOffset(Vector2 offset, int rotation)
        {
            switch (rotation)
            {
                case 90:
                    return new Vector2(-offset.y, -offset.x); // Clockwise 90°
                case 180:
                    return new Vector2(-offset.x, -offset.y); // Upside down
                case 270:
                    return new Vector2(-offset.y, -offset.x); // Counterclockwise 90°
                default: // 0 degrees
                    return offset;
            }
        }

        private Vector2 CalculateDungeonDimensions()
        {
            if (occupiedCells.Count == 0)
            {
                Debug.LogWarning("No modules have been placed. Dungeon dimensions cannot be calculated.");
                return Vector2.zero;
            }


            float minX = int.MaxValue, maxX = int.MinValue;
            float minY = int.MaxValue, maxY = int.MinValue;

            foreach (Vector2 cell in occupiedCells.Keys)
            {
                if (cell.x < minX) minX = cell.x;
                if (cell.x > maxX) maxX = cell.x;
                if (cell.y < minY) minY = cell.y;
                if (cell.y > maxY) maxY = cell.y;
            }

            // Calculate width and length
            float width = maxX - minX + 1;
            float length = maxY - minY + 1;

            //Debug.Log($"Dungeon dimensions - Width: {width}, Length: {length}");
            return new Vector2(width, length);
        }

    }
}