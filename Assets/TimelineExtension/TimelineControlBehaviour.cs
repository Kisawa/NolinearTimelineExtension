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

            public override void OnGraphStart(Playable playable)
            {
                director = playable.GetGraph().GetResolver() as PlayableDirector;
                foreach (var item in clip.track.outputs)
                    trackBinding = director.GetGenericBinding(item.sourceObject) as MonoBehaviour;
                initCondition();
            }

            public override void OnBehaviourPlay(Playable playable, FrameData info)
            {
                base.OnBehaviourPlay(playable, info);
                invokeBindingFunc(clip.onEnter);
            }

            public override void OnBehaviourPause(Playable playable, FrameData info)
            {
                if (Math.Abs(playable.GetTime() - playable.GetDuration()) < 0.000001f)
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
                        invokeBindingFunc(clip.onTrigger);
                        return;
                    }
                    invokeBindingFunc(clip.onPass);
                }
            }

            public override void ProcessFrame(Playable playable, FrameData info, object playerData)
            {
                base.ProcessFrame(playable, info, playerData);
                trackBinding = playerData as TimelineController;
                invokeBindingFunc(clip.onFrame);
            }

            void invokeBindingFunc(clipEventData eventData)
            {
                if (trackBinding != null && eventData.register && eventData.selectIndex > 0) 
                {
                    MethodInfo method = null;
                    if (eventData.clipParamIndex == -1)
                    {
                        if (eventData.otherParamType == clipEventParamType.None)
                        {
                            method = trackBinding.GetType().GetMethod(eventData.methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new Type[] { }, null);
                            if (method != null)
                                method.Invoke(trackBinding, null);
                        }
                        else
                        {
                            method = trackBinding.GetType().GetMethod(eventData.methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new Type[] { eventData.OtherParamType }, null);
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
                                method.Invoke(trackBinding, objs);
                            }
                        }
                    }
                    else if (eventData.clipParamIndex == 0)
                    {
                        if (eventData.otherParamType == clipEventParamType.None)
                        {
                            method = trackBinding.GetType().GetMethod(eventData.methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new Type[] { typeof(TimelineControlClip) }, null);
                            if (method != null)
                            {
                                method.Invoke(trackBinding, new object[] { clip });
                            }
                        }
                        else
                        {
                            method = trackBinding.GetType().GetMethod(eventData.methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new Type[] { typeof(TimelineControlClip), eventData.OtherParamType }, null);
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
                                method.Invoke(trackBinding, objs);
                            }
                        }
                    }
                    else if (eventData.clipParamIndex == 1) 
                    {
                        if (eventData.otherParamType != clipEventParamType.None)
                        {
                            method = trackBinding.GetType().GetMethod(eventData.methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new Type[] { eventData.OtherParamType, typeof(TimelineControlClip) }, null);
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
                                method.Invoke(trackBinding, objs);
                            }
                        }
                    }
                }
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
                            FieldInfo targetField = bindType.GetField(clip.conditionName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            PropertyInfo targetProperty = bindType.GetProperty(clip.conditionName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            Type targetType = typeof(string);
                            float _float_val = 0;
                            int _int_val = 0;
                            bool _bool_val = false;
                            if (targetField == null)
                            {
                                if (targetProperty != null)
                                {
                                    targetType = targetProperty.PropertyType;
                                    if (targetType == typeof(float))
                                    {
                                        _float_val = (float)targetProperty.GetValue(trackBinding);
                                    }
                                    if (targetType == typeof(int))
                                    {
                                        _int_val = (int)targetProperty.GetValue(trackBinding);
                                    }
                                    if (targetType == typeof(bool))
                                    {
                                        _bool_val = (bool)targetProperty.GetValue(trackBinding);
                                    }
                                }
                            }
                            else
                            {
                                targetType = targetField.FieldType;
                                if (targetType == typeof(float))
                                {
                                    _float_val = (float)targetField.GetValue(trackBinding);
                                }
                                if (targetType == typeof(int))
                                {
                                    _int_val = (int)targetField.GetValue(trackBinding);
                                }
                                if (targetType == typeof(bool))
                                {
                                    _bool_val = (bool)targetField.GetValue(trackBinding);
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