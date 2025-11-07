namespace BloomLines.Saving.Convertors
{
    // Конверторы сохранений с одной версии на другую, должны наследовать этот интерфейс
    public interface ISaveConverter
    {
        string GetVersion(); // Получить текущую версию на которую конвертирует

        string Convert<T>(string json) where T : SaveState;
    }
}