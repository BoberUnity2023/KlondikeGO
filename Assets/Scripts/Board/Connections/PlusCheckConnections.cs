using System.Collections.Generic;
using System.Linq;
using BloomLines.Assets;
using BloomLines.Managers;

namespace BloomLines.Boards
{
    // Логика соединения плюсиков
    public class PlusCheckConnections : ICheckConnections
    {
        // Соединяем линии
        public bool CheckConnections(Board board)
        {
            var connectedPluses = GetConnectionTiles(board);
            bool haveConnection = connectedPluses != null && connectedPluses.Count > 0;

            bool extended = false;
            foreach (var squares in connectedPluses)
            {
                foreach (var tile in squares)
                {
                    // Если в двух разных плюсиках используется один и тот же тайл, значит сбор линии вышел расширенным способом
                    var alreadyUsedTile = connectedPluses.FirstOrDefault(e => e != squares && e.FirstOrDefault(b => b == tile) != null) != null;
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
                EventsManager.Publish(new LineConnectedEvent(ConnectionType.Plus, extended, connectedPluses));

            return haveConnection;
        }

        // Берем соединенные плюсики
        public List<HashSet<BoardTile>> GetConnectionTiles(Board board)
        {
            List<HashSet<BoardTile>> pluses = new List<HashSet<BoardTile>>(1);

            foreach (var tile in board.Tiles)
            {
                if (!tile.HaveObject() || !(tile.Object is Plant))
                    continue;

                var tilePlant = tile.Object as Plant;

                if (!tilePlant.CanMakeLine())
                    continue;

                if (tile.TileX + 1 >= Board.WIDTH || tile.TileY + 1 >= Board.HEIGHT || tile.TileX - 1 < 0 || tile.TileY - 1 < 0)
                    continue;

                var leftTile = board.GetTile(tile.TileX - 1, tile.TileY);
                var rightTile = board.GetTile(tile.TileX + 1, tile.TileY);
                var upTile = board.GetTile(tile.TileX, tile.TileY + 1);
                var downTile = board.GetTile(tile.TileX, tile.TileY - 1);

                if (!leftTile.HaveObject() || !(leftTile.Object is Plant) ||
                    !rightTile.HaveObject() || !(rightTile.Object is Plant) ||
                    !upTile.HaveObject() || !(upTile.Object is Plant) ||
                    !downTile.HaveObject() || !(downTile.Object is Plant))
                    continue;

                Plant leftPlant = leftTile.Object as Plant;
                Plant rightPlant = rightTile.Object as Plant;
                Plant upPlant = upTile.Object as Plant;
                Plant downPlant = downTile.Object as Plant;

                if (!leftPlant.CanMakeLine() || !rightPlant.CanMakeLine() || !upPlant.CanMakeLine() || !downPlant.CanMakeLine())
                    continue;

                var plantType = tilePlant.PlantData.PlantType;

                if (plantType == leftPlant.PlantData.PlantType &&
                    plantType == rightPlant.PlantData.PlantType &&
                    plantType == upPlant.PlantData.PlantType &&
                    plantType == downPlant.PlantData.PlantType)
                {
                    HashSet<BoardTile> tiles = new HashSet<BoardTile>(5)
                    {
                        downTile,
                        tile,
                        upTile,
                        rightTile,
                        leftTile,
                    };

                    pluses.Add(tiles);
                }
            }

            return pluses;
        }

        // Проверяем есть ли какие то соединения
        public bool HaveAnyConnections(Board board)
        {
            var connectedPluses = GetConnectionTiles(board);
            bool haveConnection = connectedPluses != null && connectedPluses.Count > 0;
            return haveConnection;
        }
    }
}