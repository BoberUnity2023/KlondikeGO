namespace BloomLines.Boards
{
    // Интерфейс для проверки соединений на поле
    public interface ICheckConnections
    {
        bool HaveAnyConnections(Board board);
        bool CheckConnections(Board board);
    }
}