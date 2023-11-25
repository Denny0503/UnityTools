using Prism.Events;

namespace PhoenixControl.Events
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
