using System.Linq;
using Editor;
using Field;
using Turrets;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Editorwindow
{
    public class TurretOverviewWindow : EditorWindow
    {
        private int selectedTabIndex = 0;
        private string[] tabNames = {"Türme", "Description", "Effekte","Gegner"};
        private TurretScriptableObject[] turrets = new TurretScriptableObject[0];
        private Vector2 scrollPos;
        
        [MenuItem("Custom/Turret Overview")]
        public static void CreateWindow()
        {
            var window = GetWindow<TurretOverviewWindow>();
            window.titleContent = new GUIContent("Turm Übersicht");
            window.Show();
        }
        
        private void OnGUI()
        {
            if (GUILayout.Button("Update"))
            {
                FindValidObjects();
            }
		
            if (turrets.Length == 0)
                return;
            selectedTabIndex = GUILayout.Toolbar(selectedTabIndex, tabNames);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            switch (selectedTabIndex)
            {
                case 0:
                    DrawTurrets();
                    break;
                case 1:
                    DrawDescriptionComparision();
                    break;
                case 2:
                    EditorGUILayout.LabelField("TODO: Resourcen ");
                    break;
                case 3:
                    EditorGUILayout.LabelField("TODO: Typ");
                    break;
            }
            EditorGUILayout.EndScrollView();

        }
        
        private void DrawTurrets()
        {
            var sortedTurrets = turrets.OrderBy(x => x.ressourceType).ToArray();
            Ressource.RessourceType ressource = sortedTurrets[0].ressourceType;
            DrawTitle(ressource);
            
            foreach (TurretScriptableObject turret in sortedTurrets)
            {
                if (ressource != turret.ressourceType)
                {
                    ressource = turret.ressourceType;
                    DrawTitle(turret.ressourceType);
                }
                DrawTurretSelectionButton(turret);
            }
        }

        private void DrawDescriptionComparision()
        {
            foreach (var turret in turrets)
            {
                DrawTurretSelectionButton(turret);
                turret.beschreibung = EditorGUILayout.TextArea(turret.beschreibung, EditorStyles.textArea);
                EditorDesignElements.DrawSeparator();
            }
        }
        
        private void DrawStrengthsComparision()
        {
            foreach (var turret in turrets)
            {
                //TODO: vergleiche DrawDescriptionComparision()
            }
        }

        private void DrawWeaknessesComparision()
        {
            foreach (var turret in turrets)
            {
                //TODO: vergleiche DrawDescriptionComparision()
            }
        }
        
        private void FindValidObjects()
        {
            turrets = ScriptableObjectHelper.GetAllInstances<TurretScriptableObject>();
        }
        
        private void DrawTurretSelectionButton(TurretScriptableObject turret)
        {
            if (GUILayout.Button($"{turret.name} ({turret.ressourceType},{turret.turretType})"))
            {
                Selection.objects = new Object[]{turret};
            }
        }
        

        private void DrawTitle(Ressource.RessourceType ressource)
        {
            EditorDesignElements.DrawTitle(ressource.ToString());
        }
    }
}
#endif