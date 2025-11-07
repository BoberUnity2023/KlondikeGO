namespace BloomLines.Analytics
{
    // Адаптер под разную логику аналитики
    public interface IAnalyticsAdapter
    {
        void Initialize();
        void SendEvent(string id);
    }
}