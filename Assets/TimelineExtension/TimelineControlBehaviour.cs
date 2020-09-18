using System;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
    [Serializable]
    public class TimelineControlBehaviour : PlayableBehaviour
    {
        public bool Marker;
        public string Label;
        public bool Controller;
        public TimelineControlOperationType ControlType;
        public float JumpTime;
        public string JumpLabel;

        public TimelineControlTrack track;
        PlayableDirector director;
        TimelineController trackBinding;

        public override void OnGraphStart(Playable playable)
        {
            director = playable.GetGraph().GetResolver() as PlayableDirector;
        }

        public override void OnGraphStop(Playable playable)
        {

        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {

        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (Controller && trackBinding != null && !trackBinding.Pass && playable.GetTime() > 0)
            {
                switch (ControlType)
                {
                    case TimelineControlOperationType.Pause:
                        director.Pause();
                        break;
                    case TimelineControlOperationType.Repeat:
                        director.time -= playable.GetTime();
                        break;
                    case TimelineControlOperationType.JumpToTime:
                        director.time = JumpTime;
                        break;
                    case TimelineControlOperationType.JumpToMarker:
                        if (!string.IsNullOrEmpty(JumpLabel))
                        {
                            foreach (var item in track.GetClips())
                            {
                                TimelineControlClip clip = item.asset as TimelineControlClip;
                                if (clip.Marker && clip.Label == JumpLabel)
                                    director.time = item.start;
                            }
                        }
                        break;
                }
            }
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as TimelineController;
        }

        public enum TimelineControlOperationType { Pause, Repeat, JumpToTime, JumpToMarker }
    }
}