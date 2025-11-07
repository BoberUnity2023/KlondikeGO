using System.Collections.Generic;
using System.Linq;
using BloomLines.Boards;
using BloomLines.Saving;
using UnityEngine;

namespace Lines.Helpers
{
    public class Node
    {
        public int X;
        public int Y;
        public int Index; // Индекс в одномерном массиве
        public float GCost; // Расстояние от стартовой точки
        public float HCost; // Ожидаемое расстояние до конечной точки (эвристика)
        public float FCost => GCost + HCost; // Сумма G и H
        public bool IsWalkable; // Проходимость ячейки
        public Node Parent; // Родительская ячейка для восстановления пути

        public Node(int x, int y, int index, bool isWalkable)
        {
            X = x;
            Y = y;
            Index = index;
            IsWalkable = isWalkable;
        }
    }

    public static class AStarPathFinding
    {
        private static Node[,] BoardTileToNodes(BoardTile[] tiles, int startIndex, int endIndex, int width, int height)
        {
            Node[,] nodes = new Node[width, height];

            for(int i = 0; i < tiles.Length; i++)
            {
                var tile = tiles[i];

                bool isWalkable = !tile.HaveObject() || tile.Object.CanMoveThrough();
                if (i == startIndex || i == endIndex)
                    isWalkable = true;

                int x = i % width;
                int y = i / width;

                nodes[x, y] = new Node(x, y, i, isWalkable);
            }

            return nodes;
        }

        public static List<Node> FindPath(BoardTile[] tiles, int startIndex, int endIndex, int width, int height)
        {
            var nodes = BoardTileToNodes(tiles, startIndex, endIndex, width, height);

            Node startNode = nodes[startIndex % width, startIndex / width];
            Node endNode = nodes[endIndex % width, endIndex / width];

            List<Node> openList = new List<Node> { startNode };
            HashSet<Node> closedList = new HashSet<Node>();

            while (openList.Count > 0)
            {
                Node currentNode = openList.OrderBy(node => node.FCost).First();

                if (currentNode == endNode)
                {
                    return RetracePath(startNode, endNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (Node neighbor in GetNeighbors(nodes, currentNode, width, height))
                {
                    if (closedList.Contains(neighbor))
                        continue;

                    float newMovementCostToNeighbor = currentNode.GCost + GetDistance(currentNode, neighbor);
                    if (newMovementCostToNeighbor < neighbor.GCost || !openList.Contains(neighbor))
                    {
                        neighbor.GCost = newMovementCostToNeighbor;
                        neighbor.HCost = GetDistance(neighbor, endNode);
                        neighbor.Parent = currentNode;

                        if (!openList.Contains(neighbor))
                            openList.Add(neighbor);
                    }
                }
            }

            return null; // Если путь не найден
        }

        private static List<Node> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            path.Reverse();
            return path;
        }

        private static float GetDistance(Node a, Node b)
        {
            int dstX = Mathf.Abs(a.X - b.X);
            int dstY = Mathf.Abs(a.Y - b.Y);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }

        private static List<Node> GetNeighbors(Node[,] nodes, Node node, int width, int height)
        {
            List<Node> neighbors = new List<Node>();

            // Перебираем соседей только по горизонтали и вертикали
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    // Пропустить саму ячейку и диагональные соседние ячейки
                    if (x == 0 && y == 0) continue;
                    //if (x == 0 && y == 0 || Mathf.Abs(x) == Mathf.Abs(y)) continue;

                    int checkX = node.X + x;
                    int checkY = node.Y + y;

                    // Убедимся, что мы остаемся в пределах сетки
                    if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
                    {
                        Node neighbor = nodes[checkX, checkY];
                        if (neighbor.IsWalkable) // Проверяем проходимость ячейки
                        {
                            neighbors.Add(neighbor);
                        }
                    }
                }
            }

            return neighbors;
        }
    }
}