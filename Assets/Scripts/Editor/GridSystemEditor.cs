using MyGridSystem;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(GridSystem))]
    public class GridSystemEditor : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GridSystem gridSystem = (GridSystem)target;
            if (GUILayout.Button("Save Map Data"))
            {
                gridSystem.SaveMapData();
            }
            else if (GUILayout.Button("Load Map Data"))
            {
                gridSystem.LoadMapData();
            }
        }
    }
}