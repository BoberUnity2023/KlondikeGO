namespace BloomLines.Saving.Adapters
{
    // Адаптер сохранения который должны наследовать разная логика сохранения
    public interface ISaveAdapter
    {
        void Initialize();
        void Sync();
        bool HasSave<T>() where T : SaveState;
        void Save<T>(T state) where T : SaveState;
        T Load<T>() where T : SaveState;
    }
}