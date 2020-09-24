#pragma warning disable 0649
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
    [Serializable]
    public partial class TimelineControlClip : PlayableAsset, ITimelineClipAsset
    {
        public bool Marker;
        public string Label;
        public bool Controller;
        public TimelineControlOperationType ControlType;
        public float JumpFrame;
        public string JumpLabel;
        public bool Condition;

        #region EditorUes
        TimelineControlTrack track;
        [SerializeField] MonoBehaviour trackBinding;
        [SerializeField] int condition_index;
        [SerializeField] floatEnum float_enum;
        [SerializeField] float float_val;
        [SerializeField] intEnum int_enum;
        [SerializeField] int int_val;
        [SerializeField] boolEnum bool_enum;
        [SerializeField] string conditionName;

        [SerializeField] clipEventData onEnter;
        [SerializeField] clipEventData onFrame;
        [SerializeField] clipEventData onTrigger;
        [SerializeField] clipEventData onPass;
        #endregion

#if UNITY_EDITOR
        bool marker;
        string label;
        bool controller;
        TimelineControlOperationType controlType;
        float jumpFrame;
        string jumpLabel;
        bool condition;
#endif

        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= EditorApplication_playModeStateChanged;
            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
#endif
            var playable = ScriptPlayable<TimelineControlBehaviour>.Create(graph);
            TimelineControlBehaviour control = playable.GetBehaviour();
            control.clip = this;
            return playable;
        }

#if UNITY_EDITOR
        private void EditorApplication_playModeStateChanged(PlayModeStateChange obj)
        {
            switch (obj)
            {
                case PlayModeStateChange.EnteredEditMode:
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    marker = Marker;
                    label = Label;
                    controller = Controller;
                    controlType = ControlType;
                    jumpFrame = JumpFrame;
                    jumpLabel = JumpLabel;
                    condition = Condition;
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    Marker = marker;
                    Label = label;
                    Controller = controller;
                    ControlType = controlType;
                    JumpFrame = jumpFrame;
                    JumpLabel = jumpLabel;
                    Condition = condition;
                    break;
                default:
                    break;
            }
        }
#endif
    }

    public enum TimelineControlOperationType { Pause, Repeat, JumpToFrame, JumpToMarker }

    public enum floatEnum { Greater, Less }
    public enum boolEnum { True, Flase }
    public enum intEnum { Greater, Less, Equals, NotEquals }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class ConditionAttribute : Attribute { }

    [Serializable]
    struct clipEventData
    {
        public bool register;
        public int selectIndex;
        public string methodName;
        public int clipParamIndex;
        public clipEventParamType otherParamType;
        public string strVal;
        public int intVal;
        public float floatVal;
        public bool boolVal;
        public string enumTypeName;
        public int enumIndexVal;
        public Type OtherParamType {
            get {
                switch (otherParamType)
                {
                    case clipEventParamType.String:
                        return typeof(string);
                    case clipEventParamType.Int:
                        return typeof(int);
                    case clipEventParamType.Float:
                        return typeof(float);
                    case clipEventParamType.Bool:
                        return typeof(bool);
                    case clipEventParamType.Enum:
                        return Type.GetType(enumTypeName);
                    default:
                        return null;
                }
            }
        }
    }

    enum clipEventParamType { None, String, Int, Float, Bool, Enum }
}