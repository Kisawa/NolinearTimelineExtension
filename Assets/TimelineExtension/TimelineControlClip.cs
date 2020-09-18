using System;
using UnityEngine.Playables;

namespace UnityEngine.Timeline 
{
    [Serializable]
    public class TimelineControlClip : PlayableAsset, ITimelineClipAsset
    {
        public bool Marker;
        public string Label;
        public bool Controller;
        public TimelineControlBehaviour template = new TimelineControlBehaviour();
        public TimelineControlTrack track;
        public TimelineClip clip;

        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            clip.displayName = "\"None\"";
            if (Marker)
            {
                if (string.IsNullOrEmpty(Label))
                    clip.displayName = "\"Empty Label\"";
                else
                    clip.displayName = $"\"{Label}\"";
            }
            if (Controller)
            {
                string res = "";
                if (Marker)
                    res = "   |   ";
                else
                    clip.displayName = "";
                switch (template.ControlType)
                {
                    case TimelineControlBehaviour.TimelineControlOperationType.Pause:
                        clip.displayName += $"{res}Pause";
                        break;
                    case TimelineControlBehaviour.TimelineControlOperationType.Repeat:
                        clip.displayName += $"{res}Repeat";
                        break;
                    case TimelineControlBehaviour.TimelineControlOperationType.JumpToTime:
                        clip.displayName += $"{res}Jump to {template.JumpTime}";
                        break;
                    case TimelineControlBehaviour.TimelineControlOperationType.JumpToMarker:
                        if (string.IsNullOrEmpty(template.JumpLabel))
                        {
                            if (!Marker)
                                clip.displayName = "\"None\"";
                        }
                        else
                            clip.displayName += $"{res}Jump to \"{template.JumpLabel}\"";
                        break;
                    default:
                        break;
                }
            }
            var playable = ScriptPlayable<TimelineControlBehaviour>.Create(graph, template);
            TimelineControlBehaviour control = playable.GetBehaviour();
            control.Marker = Marker;
            control.Label = Label;
            control.Controller = Controller;
            control.track = track;
            return playable;
        }
    }
}