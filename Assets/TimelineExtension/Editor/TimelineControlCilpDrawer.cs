using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEngine.Timeline 
{
    [CustomEditor(typeof(TimelineControlClip))]
    public class TimelineControlCilpDrawer : Editor
    {
        SerializedObject timelineControlCilp;

        SerializedProperty m_Marker;
        SerializedProperty m_Label;
        SerializedProperty m_Controller;
        SerializedProperty m_template;

        GUIContent marker = new GUIContent("Marker", "Mark on this cilp.");
        GUIContent controller = new GUIContent("Controller", "Open controller.");

        void OnEnable()
        {
            if (target == null)
                return;
            timelineControlCilp = new SerializedObject(target);
            m_Marker = timelineControlCilp.FindProperty("Marker");
            m_Label = timelineControlCilp.FindProperty("Label");
            m_Controller = timelineControlCilp.FindProperty("Controller");
            m_template = timelineControlCilp.FindProperty("template");
        }

        public override void OnInspectorGUI()
        {
            timelineControlCilp.Update();
            m_Marker.boolValue = EditorGUILayout.BeginToggleGroup(marker, m_Marker.boolValue);
            EditorGUILayout.PropertyField(m_Label);
            EditorGUILayout.EndToggleGroup();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            m_Controller.boolValue = EditorGUILayout.BeginToggleGroup(controller, m_Controller.boolValue);
            EditorGUILayout.PropertyField(m_template);
            EditorGUILayout.EndToggleGroup();

            timelineControlCilp.ApplyModifiedProperties();
        }
    }
}