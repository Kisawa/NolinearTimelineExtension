using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEngine.Timeline 
{
    [CustomPropertyDrawer(typeof(TimelineControlBehaviour))]
    public class TimelineControlBehaviourDrawer : PropertyDrawer
    {
        GUIContent m_ControllerType = new GUIContent("Control Type", "Select a control mode.");
        GUIContent m_JumpTime = new GUIContent("Jump Time", "Jump to a time.");
        GUIContent m_JumpLabel = new GUIContent("Jump Label", "Jump to a marker with label.");

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int fieldCount = 1;
            SerializedProperty ControllerType = property.FindPropertyRelative("ControlType");
            if (ControllerType.enumValueIndex == 2 || ControllerType.enumValueIndex == 3)
                fieldCount = 2;
            return fieldCount * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty ControllerType = property.FindPropertyRelative("ControlType");
            SerializedProperty JumpTime = property.FindPropertyRelative("JumpTime");
            SerializedProperty JumpLabel = property.FindPropertyRelative("JumpLabel");

            Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(singleFieldRect, ControllerType, m_ControllerType);

            if (ControllerType.enumValueIndex == 2)
            {
                singleFieldRect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(singleFieldRect, JumpTime, m_JumpTime);
            }
            if (ControllerType.enumValueIndex == 3)
            {
                singleFieldRect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(singleFieldRect, JumpLabel, m_JumpLabel);
            }
        }
    }
}