using Prism.Events;

namespace UnityControl.Events
{
    public struct ProgressInfo
    {
        public int TotalProgress;
        public int CurrentProgress;
    }

    public class ProgressSentEvent : PubSubEvent<ProgressInfo>
    {
    }
}
