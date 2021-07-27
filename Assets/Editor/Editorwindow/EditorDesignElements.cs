using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
namespace Editor
{
    public static class EditorDesignElements
    {
        public static void DrawTitle(string title)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            DrawSeparator();
        }
    
        public static void DrawSeparator()
        {
            Color color = EditorGUIUtility.isProSkin ? Color.gray : Color.black;
            color.a = 0.2f;
            EditorGUILayoutUtility.HorizontalLine(color, new Vector2(0f, 5f));
        }
    }
}
#endif