using System.Collections.Generic;
using System.Linq;
using BloomLines.Assets;
using BloomLines.Boards;
using BloomLines.Managers;

namespace BloomLines
{
    // Логика соединений квадратов
    public class SquareCheckConnections : ICheckConnections
    {
        // Соединяем линии
        public bool CheckConnections(Board board)
        {
            var connectedSquares = GetConnectionTiles(board);

            bool haveConnection = connectedSquares != null && connectedSquares.Count > 0;

            bool extended = false;
            foreach (var squares in connectedSquares)
            {
                foreach(var tile in squares)
                {
                    // Если у двух квадратиков есть один общий тайл, значит собрали линии расширенным способом
                    var alreadyUsedTile = connectedSquares.FirstOrDefault(e => e != squares && e.FirstOrDefault(b => b == tile) != null) != null;
                    if (alreadyUsedTile)
                    {
                        extended = true;
                        break;
                    }
                }

                if (extended)
                    break;
            }

            if (haveConnection)
                EventsManager.Publish(new LineConnectedEvent(ConnectionType.Square, extended, connectedSquares));

            return haveConnection;
        }

        // Берем соединенные квадраты
        public List<HashSet<BoardTile>> GetConnectionTiles(Board board)
        {
            List<HashSet<BoardTile>> squares = new List<HashSet<BoardTile>>(1);

            foreach (var tile in board.Tiles)
            {
                if (!tile.HaveObject() || !(tile.Object is Plant))
                    continue;

                var tilePlant = tile.Object as Plant;

                if (!tilePlant.CanMakeLine())
                    continue;

                if (tile.TileX + 1 >= Board.WIDTH || tile.TileY - 1 < 0)
                    continue;

                var leftTile = board.GetTile(tile.TileX + 1, tile.TileY);
                var upTile = board.GetTile(tile.TileX, tile.TileY - 1);
                var upLeftTile = board.GetTile(tile.TileX + 1, tile.TileY - 1);

                if (!leftTile.HaveObject() || !(leftTile.Object is Plant) ||
                    !upTile.HaveObject() || !(upTile.Object is Plant) ||
                    !upLeftTile.HaveObject() || !(upLeftTile.Object is Plant))
                    continue;

                Plant leftPlant = leftTile.Object as Plant;
                Plant upPlant = upTile.Object as Plant;
                Plant upLeftPlant = upLeftTile.Object as Plant;

                if (!leftPlant.CanMakeLine() || !upPlant.CanMakeLine() || !upLeftPlant.CanMakeLine())
                    continue;

                var plantType = tilePlant.PlantData.PlantType;

                if (plantType == leftPlant.PlantData.PlantType &&
                    plantType == upPlant.PlantData.PlantType &&
                    plantType == upLeftPlant.PlantData.PlantType)
                {
                    HashSet<BoardTile> tiles = new HashSet<BoardTile>(4)
                    {
                        tile,
                        leftTile,
                        upLeftTile,
                        upTile,
                    };

                    squares.Add(tiles);
                }
            }

            return squares;
        }

        // Проверяем есть ли соединения
        public bool HaveAnyConnections(Board board)
        {
            var connectedSquares = GetConnectionTiles(board);
            return connectedSquares != null && connectedSquares.Count > 0;
        }
    }
}