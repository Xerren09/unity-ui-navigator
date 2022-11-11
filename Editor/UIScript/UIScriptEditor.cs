using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace UIViews
{
#if UNITY_EDITOR
    [CustomEditor(typeof(UIScript), true, isFallback = true)]
    public class UIScriptEditor : Editor
    {
        /// <summary>
        /// The array of possible containerIDs on the dependency.
        /// </summary>
        private string[] _dependencyContainerIDs = new string[0];
        /// <summary>
        /// The index of the selected dependency containerID.
        /// </summary>
        private int _selectedContainerIDIndex = 0;

        private bool _isFoldoutOpen = true;

        private SerializedProperty _viewNavigator;
        private SerializedProperty _viewID;
        private SerializedProperty _viewUIDoc;
        private SerializedProperty _viewStatic;
        private SerializedProperty _viewDependency;
        private SerializedProperty _viewContainerID;

        void OnEnable()
        {
            _viewNavigator = serializedObject.FindProperty("<Navigator>k__BackingField");
            _viewID = serializedObject.FindProperty("<ID>k__BackingField");
            _viewUIDoc = serializedObject.FindProperty("<UXMLDocument>k__BackingField");
            _viewStatic = serializedObject.FindProperty("<IsStatic>k__BackingField");
            _viewDependency = serializedObject.FindProperty("<Dependency>k__BackingField");
            _viewContainerID = serializedObject.FindProperty("<ContainerID>k__BackingField");
        }

        public override void OnInspectorGUI()
        {
            // Get the view data
            var view = target as UIScript;
            serializedObject.Update();

            // View setup is in its own foldout so it doesn't take up too much space on derived scripts
            _isFoldoutOpen = EditorGUILayout.BeginFoldoutHeaderGroup(_isFoldoutOpen, "View setup");
            if (_isFoldoutOpen)
            {
                // Increase ident by 1 so the foldout is more visually separated
                EditorGUI.indentLevel++;
                // Set the UI Navigator instance
                _viewNavigator.objectReferenceValue = (ViewNavigator)EditorGUILayout.ObjectField("UI Navigator", view.Navigator, typeof(ViewNavigator), true);

                EditorGUILayout.Space(10);

                _viewID.stringValue = EditorGUILayout.TextField("ID", view.ID);
                _viewUIDoc.objectReferenceValue = (VisualTreeAsset)EditorGUILayout.ObjectField("UI Document", view.UXMLDocument, typeof(VisualTreeAsset), true);
                _viewStatic.boolValue = EditorGUILayout.Toggle("Is Static", view.IsStatic);

                // This is purely for visual debug, so the toggle is disabled
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Toggle("Is View Active", view.IsViewActive);
                EditorGUI.EndDisabledGroup();

                // Dependency handling
                EditorGUILayout.Space(10);

                EditorGUILayout.LabelField("Dependency setup", EditorStyles.boldLabel);
                _viewDependency.objectReferenceValue = (UIScript)EditorGUILayout.ObjectField("Dependency", view.Dependency, typeof(UIScript), true);

                if (view.Navigator != null)
                {
                    // FIXME: Potential performance and memory impact, as this runs multiple times, not just when the dependency changes
                    // Get the container IDs from the dependency, if there is one; otherwise get the Navigator target's root
                    if (view.Dependency != null)
                    {
                        UpdateDependencyContainerIDs(view.Dependency.UXMLDocument);
                    }
                    else
                    {
                        UpdateDependencyContainerIDs(view.Navigator.Target.visualTreeAsset);
                    }

                    _selectedContainerIDIndex = Array.IndexOf<string>(_dependencyContainerIDs, view.ContainerID);
                    _selectedContainerIDIndex = EditorGUILayout.Popup("Target container ID: ", _selectedContainerIDIndex, _dependencyContainerIDs);
                    if (_selectedContainerIDIndex != -1)
                    {
                        _viewContainerID.stringValue = _dependencyContainerIDs[_selectedContainerIDIndex];
                    }
                }

                // Display informational warning on the bottom of the foldout
                ShowInformationWarnings(view);

                // Reset ident to normal
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space(10);
            serializedObject.ApplyModifiedProperties();
            // Draw the default inspector last so the script's UI can be separated. This will draw the auto UI as normal for derived scripts
            DrawDefaultInspector();
        }

        /// <summary>
        /// Checks the view setup and displays informational warnings.
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        private void ShowInformationWarnings(UIScript view)
        {
            if (view.UXMLDocument == null)
            {
                EditorGUILayout.HelpBox("No UXML document has been added, so this view will not be drawn.", MessageType.Warning);
            }
            if (view.Dependency == null)
            {
                EditorGUILayout.HelpBox("No dependency has been specified, so this view will be attached to the selected container on the root visual element.", MessageType.Info);
            }
            if (view.ContainerID == null || view.ContainerID.Length == 0)
            {
                EditorGUILayout.HelpBox("No container have been selected, so this view will not be drawn. A container must be selected.", MessageType.Error);
            }
        }


        /// <summary>
        /// Compiles and sets the list of container IDs in the dependency's VisualTreeAsset
        /// </summary>
        /// <param name="dependencyUXML"></param>
        private void UpdateDependencyContainerIDs(VisualTreeAsset dependencyUXML)
        {
            // Get every VisualElement in the dependency's hierarchy
            VisualElement layout = dependencyUXML.Instantiate();
            List<VisualElement> childList = layout.Query<VisualElement>().ToList();
            // We use instantiate, so remove the TemplateContainer's ID.
            childList.RemoveAt(0);
            var selectableContainerIDs = new List<string>();
            // Get the names of every VisualElement that has one, so they can be targeted
            foreach (var element in childList)
            {
                if (element.name != null)
                {
                    selectableContainerIDs.Add(element.name);
                }
            }
            _dependencyContainerIDs = selectableContainerIDs.ToArray();
        }
    }
#endif
}
