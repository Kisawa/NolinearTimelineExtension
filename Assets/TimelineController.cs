namespace UnityEngine.Timeline {
    public class TimelineController: MonoBehaviour
    {
        [Condition]
        public virtual bool Pass { get; set; }
    }
}