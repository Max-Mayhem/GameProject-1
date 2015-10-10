using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project
{
    class Node
    {
        public int x; public int y;
        public float f, g, h;
        public int parent;
        public Node(int i, int j)
        {
            x = i; y = j;
            f = g = h = 0;
            parent = -1;
        }

        public bool isEqual(Node n)
        {
            if (n.x == x && n.y == y) return true;
            else return false;
        }
    }

    /// <summary>
    /// Level class holds the description of the level and has navigation functions
    /// </summary>
    class Level
    {
        public int size = 32;      //32 by 32 cells in map
        public float cellSize = 2;
        Vector3 origin = Vector3.Zero;

        //What is contained in each cell
        /* 0 = Nothing
         * 1 = Player
         * 2 = Enemy
         * 3 = Obstacle
         */
        public int[,] map { get; protected set; }  
     
        //For path finding
        List<Node> nodes = new List<Node>();
        List<List<int>> neighbours = new List<List<int>>();    //Each node has a list of neighbours
        
        //Index of each node for each cell in map. This value is -1 if there is no node.
        //The array is required to get the index of the neighbours for each node
        int[,] nodeIndex;   

        public Level()
        {
            map = new int[size, size];
            initLevel();
            initGraph();
        }

        public void initLevel()
        {
            //Fill map with zeros
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    map[i, j] = 0;
                }                
            }
                       

            //Boundary
            for (int i = 0; i < size; i++)
            {
                map[i, 0] = 3;
                map[i, size - 1] = 3;
            }

            //Boundary
            for (int i = 1; i < size - 1; i++)
            {
                map[0, i] = 3;
                map[size - 1, i] = 3;
            }

            map[14, 0] = 3;
            map[14, 1] = 3;
            map[14, 2] = 3;
            map[14, 3] = 3;
            map[14, 4] = 3;
            map[14, 5] = 3;
                        
            map[3, 4] = 3;
            map[4, 4] = 3;
            map[5, 4] = 3;
            map[6, 4] = 3;

            map[17, 31] = 3;
            map[17, 30] = 3;
            map[17, 29] = 3;
            map[17, 28] = 3;
            map[17, 27] = 3;
            map[17, 26] = 3;

            map[26, 27] = 3;
            map[27, 27] = 3;
            map[28, 27] = 3;
            map[29, 27] = 3;

            map[22, 4] = 3;
            map[23, 4] = 3;
            map[24, 4] = 3;
            map[25, 4] = 3;
            map[26, 4] = 3;
            map[27, 4] = 3;
            map[28, 4] = 3;
            map[29, 4] = 3;

            map[29, 5] = 3;
            map[29, 6] = 3;

            map[9, 27] = 3;
            map[8, 27] = 3;
            map[7, 27] = 3;
            map[6, 27] = 3;
            map[5, 27] = 3;
            map[4, 27] = 3;
            map[3, 27] = 3;
            map[2, 27] = 3;
            
            map[2, 26] = 3;
            map[2, 25] = 3;

            //Some columns
            map[3, 12] = 3;
            map[4, 12] = 3;
            map[3, 13] = 3;
            map[4, 13] = 3;

            map[28, 19] = 3;
            map[27, 19] = 3;
            map[28, 18] = 3;
            map[27, 18] = 3;

            map[3, 19] = 3;
            map[4, 19] = 3;
            map[3, 18] = 3;
            map[4, 18] = 3;

            map[28, 12] = 3;
            map[27, 12] = 3;
            map[28, 13] = 3;
            map[27, 13] = 3;

            map[15, 10] = 3;
            map[16, 12] = 3;
            map[15, 14] = 3;
            map[16, 16] = 3;
            map[15, 18] = 3;
            map[16, 20] = 3;
            map[15, 22] = 3;


            map[10, 25] = 1;    //Player
            map[21, 6] = 2;    //Enemy
        }

        public void initGraph()
        {
            nodeIndex = new int[size, size];
            int nNodes = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    nodeIndex[i, j] = -1;
                    //If this node is not an obstacle, then add to the list of nodes
                    if (map[i, j] != 3)
                    {   
                        nodes.Add(new Node(i, j));
                        nodeIndex[i, j] = nNodes;
                        nNodes++;
                    }
                }
            }

            //These are the offsets to the 8 surrounding cells of each node.
            int[,] surrCells = new int[8, 2]{   {-1, 1},
                                                {0, 1},
                                                {1, 1},
                                                {1, 0},
                                                {1, -1},
                                                {0, -1},
                                                {-1, -1},
                                                {-1, 0}
                                            };
            //Check neighbours - also adds diagonal members.
            foreach (Node node in nodes)
            {
                List<int> nodeNeighbours = new List<int>();
                int x = node.x;
                int y = node.y;
                for (int i = 0; i < 8; i++)
                {
                    int ix = surrCells[i, 0];
                    int iy = surrCells[i, 1];

                    //Is the cell in bounds?
                    if (x + ix < 0) continue;
                    if (x + ix > size - 1) continue;
                    if (y + iy < 0) continue;
                    if (y + iy > size - 1) continue;
                    if (nodeIndex[x + ix, y + iy] == -1) continue;

                    //The cell can be added as a neighbour
                    nodeNeighbours.Add(nodeIndex[x + ix, y + iy]);
                }                
                neighbours.Add(nodeNeighbours);
            }
        }

        /// <summary>
        /// Returns the world space location of the center of cell(x, y). This cell can be outside the map.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector3 centerOfCell(int x, int y) 
        {
            return new Vector3(cellSize * x + cellSize / 2, 0, cellSize * y + cellSize / 2) + origin;
        }

        /// <summary>
        /// Convert world space position to cell coordinates
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        int[] coordOfPosition(Vector3 pos)
        {
            int x = (int)Math.Floor(pos.X / cellSize);
            int y = (int)Math.Floor(pos.Z / cellSize);
            return new int[2] { x, y };
        }

        /// <summary>
        /// Converts world space position to a node
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        Node nodeOfPosition(Vector3 pos)
        {            
            int x = coordOfPosition(pos)[0];
            int y = coordOfPosition(pos)[1];
            
            if (x < 0) x = 0;
            if (x > size - 1) x = size - 1;
            if (y < 0) y = 0;
            if (y > size - 1) y = size - 1;

            //Might return the cell of an obstacle
            int nIndex = nodeIndex[x, y];
            if (nIndex != -1) return nodes[nIndex];

            int[,] surrCells = new int[8, 2]{   {-1, 1},
                                                {0, 1},
                                                {1, 1},
                                                {1, 0},
                                                {1, -1},
                                                {0, -1},
                                                {-1, -1},
                                                {-1, 0}
                                            };
            int closest = -1;
            float diffLength = 2f;
            
            //Purpose of this loop is to return the closest cell to pos that is is bounds and not an obstacle.
            for (int i = 0; i < 8; i++)
            {
                int ix = surrCells[i, 0];
                int iy = surrCells[i, 1];
                
                //Is the cell in bounds?
                if (x + ix < 0) continue;
                if (x + ix > size - 1) continue;
                if (y + iy < 0) continue;
                if (y + iy > size - 1) continue;
                if (nodeIndex[x + ix, y + iy] == -1) continue;  //Cell has obstacle?

                Vector3 cellPos = centerOfCell(ix, iy);
                if ((cellPos - pos).Length() < diffLength)
                {
                    diffLength = (cellPos - pos).Length();
                    closest = i;
                }
            }

            if (closest != -1)
            {
                x += surrCells[closest, 0];
                y += surrCells[closest, 1];
                nIndex = nodeIndex[x, y];
            }
            else
            {
                nIndex = 0;
            }

            return nodes[nIndex];
        }

        /// <summary>
        /// This is the function
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<Vector3> findPathBetween(Vector3 start, Vector3 end)
        {
            List<Node> path = getPathBetween(nodeOfPosition(start), nodeOfPosition(end));
            List<Vector3> worldPath = new List<Vector3>();
            path.Reverse();

            foreach (Node node in path)
            {
                Vector3 cellPos = centerOfCell(node.x, node.y);
                cellPos.Y = -2;
                worldPath.Add(cellPos);
            }
            Console.WriteLine(path.Count);
            return worldPath;
        }

        //This is the function that the enemy class could call to get the path to the player
        List<Node> getPathBetween(Node start, Node end) 
        {
            //Reset f, g, h and parent of each node
            foreach (Node node in nodes)
            {
                node.f = node.g = node.h = 0;
                node.parent = -1;
            }

            List<Node> path = new List<Node>();         //Final path of nodes
            List<Node> openList = new List<Node>();     //Open list to be evaluated
            List<Node> closedList = new List<Node>();   //Closed list of evaluated nodes

            openList.Add(start);
                        
            while (openList.Count > 0)
            {
                //Get node in openList with the lowest f value
                float fLow = 1000;
                int iLow = -1;
                foreach (Node pnode in openList)
                {
                    if (pnode.f < fLow)
                    {
                        fLow = pnode.f;
                        iLow = openList.IndexOf(pnode);
                    }
                }

                Node currentNode = openList[iLow];

                //Is the current node the end node?
                if (currentNode.isEqual(end))
                {
                    path.Add(currentNode);
                    while (currentNode.parent != -1) {
                        currentNode = nodes[currentNode.parent];
                        path.Add(currentNode);
                    }
                    return path;
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                List<int> nodeNeighbours = neighbours[nodes.IndexOf(currentNode)];
                foreach (int i in nodeNeighbours)
                {
                    if (closedList.Contains(nodes[i]))
                        continue;

                    float gScore = currentNode.g + 1;
                    
                    if (!openList.Contains(nodes[i]))  //neighbour not visited
                    {
                        openList.Add(nodes[i]);                        
                        nodes[i].h = heuristic(nodes[i], end);
                        nodes[i].g = -1;
                    }
                    
                    //If this iteration has a better gScore or neighbour has not been visited, update its f, g and parent values.
                    if (gScore < nodes[i].g || nodes[i].g == -1)
                    {
                        nodes[i].parent = nodes.IndexOf(currentNode);
                        nodes[i].g = gScore;
                        nodes[i].f = nodes[i].g + nodes[i].h;
                    }
                }                
            }

            return path;
        }

        float heuristic(Node n1, Node n2)
        {
            return Math.Abs(n1.x - n2.x) + Math.Abs(n1.y - n2.y);
        }
    }
}
