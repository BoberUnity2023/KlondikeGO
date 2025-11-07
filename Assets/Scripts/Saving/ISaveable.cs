namespace BloomLines.Saving
{
    // Интерфейс который должны наследовать все обьекты которые должны сохраняться
    public interface ISaveable
    {
        string GetSaveData(); // Получить данные обьекта 
        void LoadSaveData(string data); // Загрузить данные обьекта
    }
}