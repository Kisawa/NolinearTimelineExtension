namespace UnityEngine.Timeline
{
    [TrackColor(0.5f, 0, 0)]
    [TrackClipType(typeof(TimelineControlClip))]
    [TrackBindingType(typeof(TimelineController))]
    public class TimelineControlTrack : TrackAsset
    {
        protected override void OnCreateClip(TimelineClip clip)
        {
            base.OnCreateClip(clip);
            TimelineControlClip _clip = clip.asset as TimelineControlClip;
            _clip.track = this;
            _clip.clip = clip;
        }
    }
}