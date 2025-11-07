using System.Collections.Generic;
using System.Linq;
using BloomLines.Assets;
using BloomLines.Managers;
using Unity.VisualScripting;
using UnityEngine;

namespace BloomLines.Boards
{
    // Логика соединения линиями
    public abstract class LineCheckConnections : ICheckConnections
    {
        protected abstract int GetLineLength();
        protected abstract ConnectionType GetConnectionType();

        // Соединяем линии
        public bool CheckConnections(Board board)
        {
            var connectedLines = GetDiagonalConnectionTiles(board, GetLineLength());
            var secondConnectedLines = GetRowConnectionTiles(board, GetLineLength());

            if (secondConnectedLines != null && secondConnectedLines.Count > 0)
                connectedLines.AddRange(secondConnectedLines);

            bool haveConnection = connectedLines != null && connectedLines.Count > 0;
            if (haveConnection)
            {
                // Если какая то линия длинее чем минимально требуемая, значит способ сборки расширенный получился
                bool isExtendedMethod = connectedLines.FirstOrDefault(e => e.Count > GetLineLength()) != null;
                EventsManager.Publish(new LineConnectedEvent(GetConnectionType(), isExtendedMethod, connectedLines));
            }

            return haveConnection;
        }
        
        // Берем вертикальные и горизонтальные линии
        public List<HashSet<BoardTile>> GetRowConnectionTiles(Board board, int length)
        {
            List<HashSet<BoardTile>> lines = new List<HashSet<BoardTile>>(2);
            HashSet<BoardTile> tempTiles = new HashSet<BoardTile>(8);

            foreach (var tile in board.Tiles)
            {
                if (!tile.HaveObject() || !(tile.Object is Plant))
                    continue;

                var tilePlant = tile.Object as Plant;

                if (!tilePlant.CanMakeLine())
                    continue;

                bool alreadyHaveTile = false;
                foreach (var line in lines)
                {
                    if (line.Contains(tile))
                    {
                        alreadyHaveTile = true;
                        break;
                    }
                }

                if (alreadyHaveTile)
                    continue;

                #region Horizontal Right
                tempTiles.Clear();
                int lineLength = 1;
                for (int x = tile.TileX + 1; x < Board.WIDTH; x++) // Проходимся горизонтально
                {
                    var horizontalTile = board.GetTile(x, tile.TileY);
                    Plant plant = (horizontalTile.HaveObject() && horizontalTile.Object is Plant) ? horizontalTile.Object as Plant : null;
                    if (plant != null && plant.PlantData.PlantType == tilePlant.PlantData.PlantType && plant.CanMakeLine())
                    {
                        lineLength++;
                        if (!tempTiles.Contains(horizontalTile))
                            tempTiles.Add(horizontalTile);
                    }
                    else
                    {
                        break;
                    }
                }

                if (lineLength >= length)
                {
                    lines.Add(new HashSet<BoardTile>(tempTiles.Count + 1));
                    lines[lines.Count - 1].Add(tile);
                    lines[lines.Count - 1].AddRange(tempTiles);
                }
                #endregion

                #region Vertical Up
                tempTiles.Clear();
                lineLength = 1;
                for (int y = tile.TileY + 1; y < Board.HEIGHT; y++) // Проходимся вертикально 
                {
                    var verticalTile = board.GetTile(tile.TileX, y);
                    Plant plant = (verticalTile.HaveObject() && verticalTile.Object is Plant) ? verticalTile.Object as Plant : null;
                    if (plant != null && plant.PlantData.PlantType == tilePlant.PlantData.PlantType && plant.CanMakeLine())
                    {
                        lineLength++;
                        if (!tempTiles.Contains(verticalTile))
                            tempTiles.Add(verticalTile);
                    }
                    else
                    {
                        break;
                    }
                }

                if (lineLength >= length)
                {
                    lines.Add(new HashSet<BoardTile>(tempTiles.Count + 1));
                    lines[lines.Count - 1].Add(tile);
                    lines[lines.Count - 1].AddRange(tempTiles);
                }
                #endregion
            }

            return lines;
        }

        // Берем диагональные линии
        public List<HashSet<BoardTile>> GetDiagonalConnectionTiles(Board board, int length)
        {
            List<HashSet<BoardTile>> lines = new List<HashSet<BoardTile>>(2);
            HashSet<BoardTile> tempTiles = new HashSet<BoardTile>(8);

            foreach (var tile in board.Tiles)
            {
                if (!tile.HaveObject() || !(tile.Object is Plant))
                    continue;

                var tilePlant = tile.Object as Plant;

                if (!tilePlant.CanMakeLine())
                    continue;

                bool alreadyHaveTile = false;
                foreach (var line in lines)
                {
                    if (line.Contains(tile))
                    {
                        alreadyHaveTile = true;
                        break;
                    }
                }

                if (alreadyHaveTile)
                    continue;

                int x = 0;
                int y = 0;

                #region Diagonal /
                tempTiles.Clear();
                int lineLength = 1;

                x = tile.TileX + 1;
                y = tile.TileY + 1;

                for (int i = 0; i < Mathf.Max(Board.WIDTH, Board.HEIGHT); i++)
                {
                    if (x + i >= Board.WIDTH || y + i >= Board.HEIGHT)
                        break;

                    var horizontalTile = board.GetTile(x + i, y + i);

                    Plant plant = (horizontalTile.HaveObject() && horizontalTile.Object is Plant) ? horizontalTile.Object as Plant : null;
                    if (plant != null && plant.PlantData.PlantType == tilePlant.PlantData.PlantType && plant.CanMakeLine())
                    {
                        lineLength++;
                        if (!tempTiles.Contains(horizontalTile))
                            tempTiles.Add(horizontalTile);
                    }
                    else
                    {
                        break;
                    }
                }

                if (lineLength >= length)
                {
                    List<BoardTile> tilesTemp = new List<BoardTile>(tempTiles.Count + 1)
                    {
                        tile,
                    };
                    tilesTemp.AddRange(tempTiles);
                    tilesTemp.Reverse();

                    lines.Add(new HashSet<BoardTile>(tilesTemp));
                }
                #endregion

                #region Diagonal \
                tempTiles.Clear();
                lineLength = 1;

                x = tile.TileX - 1;
                y = tile.TileY + 1;

                for (int i = 0; i < Mathf.Max(Board.WIDTH, Board.HEIGHT); i++)
                {
                    if (x + i < 0 || y + i >= Board.HEIGHT)
                        break;

                    var horizontalTile = board.GetTile(x - i, y + i);

                    Plant plant = (horizontalTile.HaveObject() && horizontalTile.Object is Plant) ? horizontalTile.Object as Plant : null;
                    if (plant != null && plant.PlantData.PlantType == tilePlant.PlantData.PlantType && plant.CanMakeLine())
                    {
                        lineLength++;
                        if (!tempTiles.Contains(horizontalTile))
                            tempTiles.Add(horizontalTile);
                    }
                    else
                    {
                        break;
                    }
                }

                if (lineLength >= length)
                {
                    List<BoardTile> tilesTemp = new List<BoardTile>(tempTiles.Count + 1)
                    {
                        tile,
                    };
                    tilesTemp.AddRange(tempTiles);
                    tilesTemp.Reverse();

                    lines.Add(new HashSet<BoardTile>(tilesTemp));
                }
                #endregion
            }

            return lines;
        }

        // Проверяем соединенны ли какие то линии
        public bool HaveAnyConnections(Board board)
        {
            var connectedLines = GetDiagonalConnectionTiles(board, GetLineLength());
            var secondConnectedLines = GetRowConnectionTiles(board, GetLineLength());

            return (connectedLines != null && connectedLines.Count > 0) || (secondConnectedLines != null && secondConnectedLines.Count > 0);
        }
    }
}