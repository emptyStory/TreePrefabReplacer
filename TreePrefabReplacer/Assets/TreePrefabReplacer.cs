using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
// Replaces Unity terrain trees with prefab.
[ExecuteInEditMode]
public class TreePrefabReplacer : EditorWindow
{
    [Header("References")]
    public Terrain _terrain;
    //============================================
    [MenuItem("Tools/Terrain/TreePrefabReplacer")]
    static void Init()
    {
        TreePrefabReplacer window = (TreePrefabReplacer)GetWindow(typeof(TreePrefabReplacer));
    }
    void OnGUI()
    {
        _terrain = (Terrain)EditorGUILayout.ObjectField(_terrain, typeof(Terrain), true);
        if (GUILayout.Button("Convert to prefabs"))
        {
            Convert();
        }
        if (GUILayout.Button("Clear generated trees"))
        {
            Clear();
        }
    }
    //============================================
    public void Convert()
    {
        TerrainData data = _terrain.terrainData;
        float width = data.size.x;
        float height = data.size.z;
        float y = data.size.y;
        // Create parent
        GameObject parent = GameObject.Find("PREFABS_TREES_GENERATED");
        if (parent == null)
        {
            parent = new GameObject("PREFABS_TREES_GENERATED");
        }
        // Create trees
        foreach (TreeInstance tree in data.treeInstances)
        {
            if (tree.prototypeIndex >= data.treePrototypes.Length)
                continue;
            var _tree = data.treePrototypes[tree.prototypeIndex].prefab;
            Vector3 position = new Vector3(
                tree.position.x * width,
                tree.position.y * y,
                tree.position.z * height) + _terrain.transform.position;
            Vector3 scale = new Vector3(tree.widthScale, tree.heightScale, tree.widthScale);
            GameObject go = PrefabUtility.InstantiatePrefab((_tree), parent.transform) as GameObject;
            go.transform.position = position;
            go.transform.rotation = Quaternion.Euler(0f, Mathf.Rad2Deg * tree.rotation, 0f);
            go.transform.localScale = scale;
        }
    }
    public void Clear()
    {
        DestroyImmediate(GameObject.Find("PREFABS_TREES_GENERATED"));
    }
}