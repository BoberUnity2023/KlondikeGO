using System.Collections.Generic;
using System.Linq;
using BloomLines.Assets;
using BloomLines.Managers;

namespace BloomLines.Boards
{
    // Логика соединений треугольником
    public class TriangleCheckConnections : ICheckConnections
    {
        // Соединяем линии
        public bool CheckConnections(Board board)
        {
            var connectedTriangles = GetConnectionTiles(board);

            bool haveConnection = connectedTriangles != null && connectedTriangles.Count > 0;

            bool extended = false;
            foreach (var squares in connectedTriangles)
            {
                foreach (var tile in squares)
                {
                    // Если в двух треугольниках используется один общий тайл, то собрали линии расширенным способом
                    var alreadyUsedTile = connectedTriangles.FirstOrDefault(e => e != squares && e.FirstOrDefault(b => b == tile) != null) != null;
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
                EventsManager.Publish(new LineConnectedEvent(ConnectionType.Triangle, extended, connectedTriangles));

            return haveConnection;
        }

        // Берем соединенные треугольники
        public List<HashSet<BoardTile>> GetConnectionTiles(Board board)
        {
            List<HashSet<BoardTile>> triangles = new List<HashSet<BoardTile>>(1);

            foreach (var tile in board.Tiles)
            {
                if (!tile.HaveObject() || !(tile.Object is Plant))
                    continue;

                var tilePlant = tile.Object as Plant;

                if (!tilePlant.CanMakeLine())
                    continue;

                //Запоминием соседние плитки
                List<BoardTile> adjacentTiles = new List<BoardTile>();    
                if (tile.TileX + 1 < Board.WIDTH)
                    adjacentTiles.Add(board.GetTile(tile.TileX + 1, tile.TileY));

                if (tile.TileX - 1 >= 0)
                    adjacentTiles.Add(board.GetTile(tile.TileX - 1, tile.TileY));

                if (tile.TileY + 1 < Board.HEIGHT)
                    adjacentTiles.Add(board.GetTile(tile.TileX, tile.TileY + 1));

                if (tile.TileY - 1 >= 0)
                    adjacentTiles.Add(board.GetTile(tile.TileX, tile.TileY - 1));

                if (adjacentTiles.Count < 3)
                    continue;

                //Выбираем растения                
                List<Plant> plants = new List<Plant>();
                foreach(BoardTile boardTile in adjacentTiles)
                {
                    Plant plant = boardTile.Object as Plant;
                    if (plant != null && plant.CanMakeLine())
                        plants.Add(plant);
                }

                PlantType plantType = tilePlant.PlantData.PlantType;
                //Считаем растения нужного типа
                int count = (from p in plants where p.PlantData.PlantType == plantType select p).Count();                

                if (count >= 3)
                {
                    HashSet<BoardTile> tiles = new HashSet<BoardTile>(4)
                    {
                        tile
                    };

                    foreach (BoardTile boardTile in adjacentTiles)
                    {
                        Plant plant = boardTile.Object as Plant;
                        
                        if (plant != null && plantType == plant.PlantData.PlantType && tiles.Count <= 3)
                            tiles.Add(boardTile);
                    }

                    triangles.Add(tiles);
                }
            }

            return triangles;
        }

        // Проверяем есть ли соединения
        public bool HaveAnyConnections(Board board)
        {
            var connectedTriangles = GetConnectionTiles(board);
            return connectedTriangles != null && connectedTriangles.Count > 0;
        }
    }
}