using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
    public partial class TimelineControlClip {
        public class TimelineControlBehaviour : PlayableBehaviour
        {
            public TimelineControlClip clip;
            MonoBehaviour trackBinding;
            PlayableDirector director;
            Func<bool> condition = () => false;
            FieldInfo conditionField;
            PropertyInfo conditionProperty;
            (MethodInfo, object[]) onEnter;
            (MethodInfo, object[]) onFrame;
            (MethodInfo, object[]) onTrigger;
            (MethodInfo, object[]) onPass;

            public override void OnGraphStart(Playable playable)
            {
                director = playable.GetGraph().GetResolver() as PlayableDirector;
                foreach (var item in clip.track.outputs)
                    trackBinding = director.GetGenericBinding(item.sourceObject) as MonoBehaviour;
                initCondition();
                onEnter = initBindingMethod(clip.onEnter);
                onFrame = initBindingMethod(clip.onFrame);
                onTrigger = initBindingMethod(clip.onTrigger);
                onPass = initBindingMethod(clip.onPass);
            }

            public override void OnBehaviourPlay(Playable playable, FrameData info)
            {
                base.OnBehaviourPlay(playable, info);
                if (Math.Abs(playable.GetTime()) < 0.01f)
                {
                    invokeBindingMethod(onEnter);
                    if (clip.ControlTimingType == TimelineControlTimingType.Start)
                    {
                        if (clip.ControlType == TimelineControlOperationType.Repeat)
                            Debug.LogWarning("TimelineController: Please check your clip controller. There is a clip controller timing is on clip Start, but the controller type still is Repeat so this controller won't used");
                        else
                            control(playable);
                    }
                }
            }

            public override void OnBehaviourPause(Playable playable, FrameData info)
            {
                if (Math.Abs(playable.GetTime() - playable.GetDuration()) < 0.01f)
                {
                    if(clip.ControlTimingType != TimelineControlTimingType.End || !control(playable))
                        invokeBindingMethod(onPass);
                }
            }

            public override void ProcessFrame(Playable playable, FrameData info, object playerData)
            {
                base.ProcessFrame(playable, info, playerData);
                trackBinding = playerData as TimelineController;
                invokeBindingMethod(onFrame);
                if (clip.ControlTimingType == TimelineControlTimingType.Frame)
                    control(playable);
            }

            bool control(Playable playable)
            {
                if (condition())
                {
                    switch (clip.ControlType)
                    {
                        case TimelineControlOperationType.Pause:
                            director.Pause();
                            break;
                        case TimelineControlOperationType.Repeat:
                            director.time -= playable.GetTime();
                            break;
                        case TimelineControlOperationType.JumpToFrame:
                            director.time = clip.JumpFrame / clip.track.timelineAsset.editorSettings.fps;
                            break;
                        case TimelineControlOperationType.JumpToMarker:
                            if (!string.IsNullOrEmpty(clip.JumpLabel))
                            {
                                foreach (var item in clip.track.GetClips())
                                {
                                    TimelineControlClip clip = item.asset as TimelineControlClip;
                                    if (clip.Marker && clip.Label == clip.JumpLabel)
                                        director.time = item.start;
                                }
                            }
                            break;
                    }
                    invokeBindingMethod(onTrigger);
                    return true;
                }
                else
                    return false;
            }

            void invokeBindingMethod((MethodInfo, object[]) data) 
            {
                if (trackBinding != null && data.Item1 != null) 
                {
                    data.Item1.Invoke(trackBinding, data.Item2);
                }
            }

            (MethodInfo, object[]) initBindingMethod(clipEventData eventData)
            {
                if (trackBinding != null && eventData.register && eventData.selectIndex > 0) 
                {
                    if (eventData.clipParamIndex == -1)
                    {
                        if (eventData.otherParamType == clipEventParamType.None)
                        {
                            MethodInfo method = trackBinding.GetType().GetMethod(eventData.methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new Type[] { }, null);
                            return (method, null);
                        }
                        else
                        {
                            MethodInfo method = trackBinding.GetType().GetMethod(eventData.methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new Type[] { eventData.OtherParamType }, null);
                            if (method != null)
                            {
                                object[] objs = new object[1];
                                switch (eventData.otherParamType)
                                {
                                    case clipEventParamType.String:
                                        objs[0] = eventData.strVal;
                                        break;
                                    case clipEventParamType.Int:
                                        objs[0] = eventData.intVal;
                                        break;
                                    case clipEventParamType.Float:
                                        objs[0] = eventData.floatVal;
                                        break;
                                    case clipEventParamType.Bool:
                                        objs[0] = eventData.boolVal;
                                        break;
                                    case clipEventParamType.Enum:
                                        objs[0] = Enum.GetValues(eventData.OtherParamType).GetValue(eventData.enumIndexVal);
                                        break;
                                }
                                return (method, objs);
                            }
                        }
                    }
                    else if (eventData.clipParamIndex == 0)
                    {
                        if (eventData.otherParamType == clipEventParamType.None)
                        {
                            MethodInfo method = trackBinding.GetType().GetMethod(eventData.methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new Type[] { typeof(TimelineControlClip) }, null);
                            return (method, new object[] { clip });
                        }
                        else
                        {
                            MethodInfo method = trackBinding.GetType().GetMethod(eventData.methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new Type[] { typeof(TimelineControlClip), eventData.OtherParamType }, null);
                            if (method != null)
                            {
                                object[] objs = new object[2];
                                objs[0] = clip;
                                switch (eventData.otherParamType)
                                {
                                    case clipEventParamType.String:
                                        objs[1] = eventData.strVal;
                                        break;
                                    case clipEventParamType.Int:
                                        objs[1] = eventData.intVal;
                                        break;
                                    case clipEventParamType.Float:
                                        objs[1] = eventData.floatVal;
                                        break;
                                    case clipEventParamType.Bool:
                                        objs[1] = eventData.boolVal;
                                        break;
                                    case clipEventParamType.Enum:
                                        objs[1] = Enum.GetValues(eventData.OtherParamType).GetValue(eventData.enumIndexVal);
                                        break;
                                }
                                return (method, objs);
                            }
                        }
                    }
                    else if (eventData.clipParamIndex == 1) 
                    {
                        if (eventData.otherParamType != clipEventParamType.None)
                        {
                            MethodInfo method = trackBinding.GetType().GetMethod(eventData.methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new Type[] { eventData.OtherParamType, typeof(TimelineControlClip) }, null);
                            if (method != null)
                            {
                                object[] objs = new object[2];
                                objs[1] = clip;
                                switch (eventData.otherParamType)
                                {
                                    case clipEventParamType.String:
                                        objs[0] = eventData.strVal;
                                        break;
                                    case clipEventParamType.Int:
                                        objs[0] = eventData.intVal;
                                        break;
                                    case clipEventParamType.Float:
                                        objs[0] = eventData.floatVal;
                                        break;
                                    case clipEventParamType.Bool:
                                        objs[0] = eventData.boolVal;
                                        break;
                                    case clipEventParamType.Enum:
                                        objs[0] = Enum.GetValues(eventData.OtherParamType).GetValue(eventData.enumIndexVal);
                                        break;
                                }
                                return (method, objs);
                            }
                        }
                    }
                }
                return (null, null);
            }

            void initCondition()
            {
                condition = () =>
                {
                    if (clip.Controller)
                    {
                        if (clip.Condition && trackBinding != null)
                        {
                            bool res = false;
                            Type bindType = trackBinding.GetType();
                            if(conditionField == null)
                                conditionField = bindType.GetField(clip.conditionName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            if(conditionProperty == null)
                                conditionProperty = bindType.GetProperty(clip.conditionName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            Type targetType = typeof(string);
                            float _float_val = 0;
                            int _int_val = 0;
                            bool _bool_val = false;
                            if (conditionField == null)
                            {
                                if (conditionProperty != null)
                                {
                                    targetType = conditionProperty.PropertyType;
                                    if (targetType == typeof(float))
                                    {
                                        _float_val = (float)conditionProperty.GetValue(trackBinding);
                                    }
                                    if (targetType == typeof(int))
                                    {
                                        _int_val = (int)conditionProperty.GetValue(trackBinding);
                                    }
                                    if (targetType == typeof(bool))
                                    {
                                        _bool_val = (bool)conditionProperty.GetValue(trackBinding);
                                    }
                                }
                            }
                            else
                            {
                                targetType = conditionField.FieldType;
                                if (targetType == typeof(float))
                                {
                                    _float_val = (float)conditionField.GetValue(trackBinding);
                                }
                                if (targetType == typeof(int))
                                {
                                    _int_val = (int)conditionField.GetValue(trackBinding);
                                }
                                if (targetType == typeof(bool))
                                {
                                    _bool_val = (bool)conditionField.GetValue(trackBinding);
                                }
                            }

                            if (targetType == typeof(float))
                            {
                                switch (clip.float_enum)
                                {
                                    case floatEnum.Greater:
                                        res = _float_val > clip.float_val;
                                        break;
                                    case floatEnum.Less:
                                        res = _float_val < clip.float_val;
                                        break;
                                }
                            }
                            if (targetType == typeof(int))
                            {
                                switch (clip.int_enum)
                                {
                                    case intEnum.Greater:
                                        res = _int_val > clip.int_val;
                                        break;
                                    case intEnum.Less:
                                        res = _int_val < clip.int_val;
                                        break;
                                    case intEnum.Equals:
                                        res = _int_val == clip.int_val;
                                        break;
                                    case intEnum.NotEquals:
                                        res = _int_val != clip.int_val;
                                        break;
                                }
                            }
                            if (targetType == typeof(bool))
                            {
                                switch (clip.bool_enum)
                                {
                                    case boolEnum.True:
                                        res = _bool_val;
                                        break;
                                    case boolEnum.Flase:
                                        res = !_bool_val;
                                        break;
                                }
                            }
                            return res;
                        }
                        else
                            return true;
                    }
                    else
                    {
                        return false;
                    }
                };
            }
        }
    }
}