using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityEngine.Timeline 
{
    [CustomEditor(typeof(TimelineControlClip))]
    public class TimelineControlCilpInspectorEditor : Editor
    {
        SerializedObject timelineControlCilp;

        SerializedProperty m_Marker;
        SerializedProperty m_Label;
        SerializedProperty m_Controller;
        SerializedProperty m_ControlType;
        SerializedProperty m_JumpFrame;
        SerializedProperty m_JumpLabel;
        SerializedProperty m_Condition;
        SerializedProperty m_trackBinding;

        GUIContent marker = new GUIContent("Marker", "Mark on this cilp.");
        GUIContent controller = new GUIContent("Controller", "Open the control.");
        GUIContent controllerType = new GUIContent("Control Type", "Select a control mode.");
        GUIContent jumpFrame = new GUIContent("Jump Frame", "Jump to a frame.");
        GUIContent jumpLabel = new GUIContent("Jump Label", "Jump to a marker with label.");
        GUIContent condition = new GUIContent("Condition", "Open condition control. Supported types: float, int and bool.");
        GUIContent enterEvent = new GUIContent("On Enter", "Register an event for the clip start.");
        GUIContent triggerEvent = new GUIContent("On Trigger", "Register an event for the clip controller triggering.");
        GUIContent passEvent = new GUIContent("On Pass", "Register an event for the clip pass.");
        GUIContent frameEvent = new GUIContent("On Frame", "Register an event for the clip every frame.");

        SerializedProperty condition_index;
        SerializedProperty float_enum;
        SerializedProperty float_val;
        SerializedProperty int_enum;
        SerializedProperty int_val;
        SerializedProperty bool_enum;
        SerializedProperty conditionName;

        SerializedProperty onEnter;
        SerializedProperty onFrame;
        SerializedProperty onTrigger;
        SerializedProperty onPass;

        void OnEnable()
        {
            if (target == null)
                return;
            timelineControlCilp = new SerializedObject(target);
            m_Marker = timelineControlCilp.FindProperty("Marker");
            m_Label = timelineControlCilp.FindProperty("Label");
            m_Controller = timelineControlCilp.FindProperty("Controller");
            m_ControlType = timelineControlCilp.FindProperty("ControlType");
            m_JumpFrame = timelineControlCilp.FindProperty("JumpFrame");
            m_JumpLabel = timelineControlCilp.FindProperty("JumpLabel");
            m_Condition = timelineControlCilp.FindProperty("Condition");
            m_trackBinding = timelineControlCilp.FindProperty("trackBinding");

            condition_index = timelineControlCilp.FindProperty("condition_index");
            float_enum = timelineControlCilp.FindProperty("float_enum");
            float_val = timelineControlCilp.FindProperty("float_val"); 
            int_enum = timelineControlCilp.FindProperty("int_enum");
            int_val = timelineControlCilp.FindProperty("int_val");
            bool_enum = timelineControlCilp.FindProperty("bool_enum");
            conditionName = timelineControlCilp.FindProperty("conditionName");

            onEnter = timelineControlCilp.FindProperty("onEnter");
            onFrame = timelineControlCilp.FindProperty("onFrame");
            onTrigger = timelineControlCilp.FindProperty("onTrigger");
            onPass = timelineControlCilp.FindProperty("onPass");
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
            EditorGUILayout.PropertyField(m_ControlType, controllerType);
            if(m_ControlType.enumValueIndex == 2)
                EditorGUILayout.PropertyField(m_JumpFrame, jumpFrame);
            if(m_ControlType.enumValueIndex == 3)
                EditorGUILayout.PropertyField(m_JumpLabel, jumpLabel);

            EditorGUILayout.Space();

            if (m_Controller.boolValue)
            {
                if (m_trackBinding.objectReferenceValue != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space(15, false);
                    EditorGUILayout.BeginVertical();
                    m_Condition.boolValue = EditorGUILayout.BeginToggleGroup(condition, m_Condition.boolValue);
                    if (m_Condition.boolValue)
                    {
                        Type controllerType = m_trackBinding.objectReferenceValue.GetType();
                        Dictionary<string, Type> conditions = new Dictionary<string, Type>();
                        FieldInfo[] fields = controllerType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        for (int i = 0; i < fields.Length; i++)
                        {
                            FieldInfo field = fields[i];
                            Type fieldType = field.FieldType;
                            if (field.IsDefined(typeof(ConditionAttribute), true) && (fieldType == typeof(float) || fieldType == typeof(int) || fieldType == typeof(bool)))
                                conditions.Add(field.Name, fieldType);
                        }
                        PropertyInfo[] properties = controllerType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        for (int i = 0; i < properties.Length; i++)
                        {
                            PropertyInfo property = properties[i];
                            Type propertyType = property.PropertyType;
                            if (property.IsDefined(typeof(ConditionAttribute), true) && (propertyType == typeof(float) || propertyType == typeof(int) || propertyType == typeof(bool)))
                                conditions.Add(property.Name, propertyType);
                        }
                        string[] keys = conditions.Keys.ToArray();
                        if (keys.Length > 0)
                        {
                            EditorGUILayout.BeginHorizontal();
                            if (condition_index.intValue >= keys.Length)
                                condition_index.intValue = keys.Length - 1;
                            condition_index.intValue = EditorGUILayout.Popup(condition_index.intValue, keys);
                            string targetName = keys[condition_index.intValue];
                            Type targetType = conditions[targetName];
                            if (targetType == typeof(float))
                            {
                                float_enum.enumValueIndex = (int)(floatEnum)EditorGUILayout.EnumPopup((floatEnum)float_enum.enumValueIndex);
                                float_val.floatValue = EditorGUILayout.FloatField(float_val.floatValue);
                                int_val.intValue = (int)float_val.floatValue;
                            }
                            if (targetType == typeof(int))
                            {
                                int_enum.enumValueIndex = (int)(intEnum)EditorGUILayout.EnumPopup((intEnum)int_enum.enumValueIndex);
                                int_val.intValue = EditorGUILayout.IntField(int_val.intValue);
                                float_val.floatValue = int_val.intValue;
                            }
                            if (targetType == typeof(bool))
                            {
                                bool_enum.enumValueIndex = (int)(boolEnum)EditorGUILayout.EnumPopup((boolEnum)bool_enum.enumValueIndex);
                            }
                            conditionName.stringValue = targetName;
                            EditorGUILayout.EndHorizontal();
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("The condition for the controller is NULL.", MessageType.Warning);
                        }
                    }
                    EditorGUILayout.EndToggleGroup();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
                else 
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space(15, false);
                    EditorGUILayout.HelpBox("No condition", MessageType.None);
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndToggleGroup();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (m_trackBinding.objectReferenceValue != null) 
            {
                MethodInfo[] methods = m_trackBinding.objectReferenceValue.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                if (methods.Length > 0) 
                {
                    List<(string, int, Type)> canUseMethods = new List<(string, int, Type)>();
                    canUseMethods.Add(("None", -1, null));
                    for (int i = 0; i < methods.Length; i++)
                    {
                        MethodInfo method = methods[i];
                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters.Length == 0)
                            canUseMethods.Add((method.Name, -1, null));
                        else if (parameters.Length == 1)
                        {
                            if (parameters[0].ParameterType == typeof(TimelineControlClip))
                                canUseMethods.Add((method.Name, 0, null));
                            else if (checkEventParamType(parameters[0].ParameterType))
                                canUseMethods.Add((method.Name, -1, parameters[0].ParameterType));
                        }
                        else if (parameters.Length == 2)
                        {
                            if (parameters[0].ParameterType == typeof(TimelineControlClip) && checkEventParamType(parameters[1].ParameterType))
                                canUseMethods.Add((method.Name, 0, parameters[1].ParameterType));
                            else if (parameters[1].ParameterType == typeof(TimelineControlClip) && checkEventParamType(parameters[0].ParameterType))
                                canUseMethods.Add((method.Name, 1, parameters[0].ParameterType));
                        }
                    }
                    string[] names = canUseMethods.Select(x =>
                    {
                        if (x.Item2 == -1)
                        {
                            if (x.Item3 == null)
                                return x.Item1;
                            else
                                return $"{x.Item1} ({getTypeViewName(x.Item3)})";
                        }
                        else if (x.Item2 == 0)
                        {
                            if (x.Item3 == null)
                                return $"{x.Item1} (clip)";
                            else
                                return $"{x.Item1} (clip, {getTypeViewName(x.Item3)})";
                        }
                        else if (x.Item2 == 1)
                        {
                            if (x.Item3 == null)
                                return "Error";
                            else
                                return $"{x.Item1} ({getTypeViewName(x.Item3)}, clip)";
                        }
                        else
                            return "Error";
                    }).ToArray();
                    initEventGUI(enterEvent, onEnter, canUseMethods, names);
                    initEventGUI(frameEvent, onFrame, canUseMethods, names);
                    initEventGUI(triggerEvent, onTrigger, canUseMethods, names);
                    initEventGUI(passEvent, onPass, canUseMethods, names);
                }
                else
                    EditorGUILayout.HelpBox("No function", MessageType.None);
            }
            else
                EditorGUILayout.HelpBox("No function", MessageType.None);
            
            timelineControlCilp.ApplyModifiedProperties();
        }

        void initEventGUI(GUIContent content, SerializedProperty eventProperty, List<(string, int, Type)> canUseMethods, string[] viewNames) 
        {
            SerializedProperty register = eventProperty.FindPropertyRelative("register");
            SerializedProperty selectIndex = eventProperty.FindPropertyRelative("selectIndex");
            SerializedProperty methodName = eventProperty.FindPropertyRelative("methodName");
            SerializedProperty clipParamIndex = eventProperty.FindPropertyRelative("clipParamIndex");
            SerializedProperty otherParamType = eventProperty.FindPropertyRelative("otherParamType");
            SerializedProperty strVal = eventProperty.FindPropertyRelative("strVal");
            SerializedProperty intVal = eventProperty.FindPropertyRelative("intVal");
            SerializedProperty floatVal = eventProperty.FindPropertyRelative("floatVal");
            SerializedProperty boolVal = eventProperty.FindPropertyRelative("boolVal");
            SerializedProperty enumTypeName = eventProperty.FindPropertyRelative("enumTypeName");
            SerializedProperty enumIndexVal = eventProperty.FindPropertyRelative("enumIndexVal");

            if (selectIndex.intValue >= canUseMethods.Count || 
                canUseMethods[selectIndex.intValue].Item1 != methodName.stringValue || 
                canUseMethods[selectIndex.intValue].Item2 != clipParamIndex.intValue ||
                getTypeEnumIndex(canUseMethods[selectIndex.intValue].Item3) != otherParamType.enumValueIndex)
            {
                selectIndex.intValue = 0;
                for (int i = 0; i < viewNames.Length; i++)
                {
                    if (canUseMethods[i].Item1 == methodName.stringValue &&
                        canUseMethods[i].Item2 == clipParamIndex.intValue && 
                        getTypeEnumIndex(canUseMethods[i].Item3) == otherParamType.enumValueIndex)
                        selectIndex.intValue = i;
                }
            }
            register.boolValue = EditorGUILayout.BeginToggleGroup(content, register.boolValue);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(30, false);
            selectIndex.intValue = EditorGUILayout.Popup(selectIndex.intValue, viewNames);
            EditorGUILayout.EndHorizontal();

            (string, int, Type) data = canUseMethods[selectIndex.intValue];
            methodName.stringValue = data.Item1;
            clipParamIndex.intValue = data.Item2;
            otherParamType.enumValueIndex = 0;
            if (data.Item2 > -1 || data.Item3 != null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space(30, false);
                if (data.Item2 == 0)
                    EditorGUILayout.HelpBox("with self clip", MessageType.None);
                if (data.Item3 == typeof(string))
                {
                    otherParamType.enumValueIndex = 1;
                    strVal.stringValue = EditorGUILayout.TextField(strVal.stringValue);
                }
                if (data.Item3 == typeof(int))
                {
                    otherParamType.enumValueIndex = 2;
                    intVal.intValue = EditorGUILayout.IntField(intVal.intValue);
                }
                if (data.Item3 == typeof(float))
                {
                    otherParamType.enumValueIndex = 3;
                    floatVal.floatValue = EditorGUILayout.FloatField(floatVal.floatValue);
                }
                if (data.Item3 == typeof(bool))
                {
                    otherParamType.enumValueIndex = 4;
                    boolVal.boolValue = EditorGUILayout.Toggle("Parameter value:", boolVal.boolValue);
                }
                if (data.Item3 != null && data.Item3.IsEnum)
                {
                    otherParamType.enumValueIndex = 5;
                    enumTypeName.stringValue = data.Item3.FullName;
                    string[] enumStrs = data.Item3.GetEnumNames();
                    enumIndexVal.intValue = EditorGUILayout.Popup(enumIndexVal.intValue, enumStrs);
                }
                if (data.Item2 == 1)
                    EditorGUILayout.HelpBox("with self clip", MessageType.None);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndToggleGroup();
        }

        string getTypeViewName(Type type) 
        {
            if (type == typeof(string))
                return "string";
            if (type == typeof(int))
                return "int";
            if (type == typeof(float))
                return "float";
            if (type == typeof(bool))
                return "bool";
            return type.Name;
        }

        bool checkEventParamType(Type type) 
        {
            if (type == typeof(string) || 
                type == typeof(int) || 
                type == typeof(float) || 
                type == typeof(bool) || 
                type.IsEnum)
                return true;
            return false;
        }

        int getTypeEnumIndex(Type type) 
        {
            if (type == typeof(string))
            {
                return 1;
            }
            if (type == typeof(int))
            {
                return 2;
            }
            if (type == typeof(float))
            {
                return 3;
            }
            if (type == typeof(bool))
            {
                return 4;
            }
            if (type != null && type.IsEnum)
            {
                return 5;
            }
            return 0;
        }
    }
}