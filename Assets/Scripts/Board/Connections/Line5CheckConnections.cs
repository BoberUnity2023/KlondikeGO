using BloomLines.Assets;

namespace BloomLines.Boards
{   
    // Наследуемся от логики соединения линиями
    public class Line5CheckConnections : LineCheckConnections
    {
        // Текущий тип соединения
        protected override ConnectionType GetConnectionType()
        {
            return ConnectionType.Line5;
        }

        // Длина линии минимум 5
        protected override int GetLineLength()
        {
            return 5;
        }
    }
}