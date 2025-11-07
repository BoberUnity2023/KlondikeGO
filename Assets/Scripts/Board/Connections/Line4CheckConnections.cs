using BloomLines.Assets;

namespace BloomLines.Boards
{
    // Наследуемся от логики соединения линиями
    public class Line4CheckConnections : LineCheckConnections
    {
        // Текущий тип соединения
        protected override ConnectionType GetConnectionType()
        {
            return ConnectionType.Line4;
        }
        
        // Длина линии минимум 4
        protected override int GetLineLength()
        {
            return 4;
        }
    }
}