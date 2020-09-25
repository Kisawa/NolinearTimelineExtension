namespace UnityEngine.Timeline {
    public class TimelineController: MonoBehaviour
    {
        [Condition]
        public bool condition;

        void Enter(string str) 
        {
            Debug.LogError(str);
        }

        void Frame(int i)
        {
            Debug.LogError(i);
        }

        void Trigger(TimelineControlClip clip, bool b)
        {
            if (b)
                clip.ControlType = TimelineControlOperationType.Pause;
            else
                clip.ControlType = TimelineControlOperationType.JumpToFrame;
        }

        void Pass(testEnum e)
        {
            Debug.LogError(e);
        }
    }

    enum testEnum { test1 = 1001, test2 = 1005, test3 = 1010 }
}