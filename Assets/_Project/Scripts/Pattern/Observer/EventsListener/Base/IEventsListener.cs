namespace Virtuesky.Events
{
    public interface IEventsListener
    {
        void OnEventRaised(EventName eventName);
    }

    public interface IEventsListener<in TType>
    {
        void OnEventRaised(EventName eventName, TType value);
    }
}