using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Graph;

/*This script assumes a square plane.*/

public class GridCoordinate
{
    private int row;
    private int col;

    public int Row
    {
        get { return row; }
    }

    public int Column
    {
        get { return col; }
    }

    public GridCoordinate(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    public override string ToString()
    {
        return "[" + row + ", " + col + "]";
    }
}

public class GridSquare : IHeuristic<GridSquare>
{
    private Vector3 m_Centre;
    private GridCoordinate coordinate;
    private GraphNode<LevelNode> node;

    public GraphNode<LevelNode> Node
    {
        get { return node; }
        set { node = value; }
    }

    public Vector3 Position
    {
        get { return m_Centre; }
        set { m_Centre = value; }
    }

    public GridCoordinate Coordinate
    {
        get { return coordinate; }
        set { coordinate = value; }
    }

    public GridSquare(Vector3 centre)
    {
        this.m_Centre = centre;
    }

    public GridSquare(Vector3 centre, GridCoordinate coord)
    {
        this.m_Centre = centre;
        this.coordinate = coord;
    }

    public bool IsNeighbor(GridSquare s)
    {
        if (s.coordinate.Row == this.coordinate.Row && ((this.coordinate.Column - 1) == s.coordinate.Column || (this.coordinate.Column + 1) == s.coordinate.Column))
            return true;

        else if (s.coordinate.Column == this.coordinate.Column && ((this.coordinate.Row - 1) == s.coordinate.Row || (this.coordinate.Row + 1) == s.coordinate.Row))
            return true;

        /*
        else if (((s.coordinate.Column == (this.coordinate.Column - 1)) && s.coordinate.Row == (this.coordinate.Row - 1)) ||
                ((s.coordinate.Column == (this.coordinate.Column + 1)) && s.coordinate.Row == (this.coordinate.Row - 1)) ||
                ((s.coordinate.Column == (this.coordinate.Column - 1)) && s.coordinate.Row == (this.coordinate.Row + 1)) ||
                ((s.coordinate.Column == (this.coordinate.Column + 1)) && s.coordinate.Row == (this.coordinate.Row + 1))
                )
            return true;
            */

        else
            return false;
    }

    public double ComputeHeuristic(GridSquare goal)
    {
        return (this.Position - goal.Position).magnitude;
    }

    public override string ToString()
    {
        return coordinate.ToString() + " " + Position.ToString();
    }

}

//this class is used by the can build function for recursive depth from spawn searching
public class NodeDepth
{
    private int depth_index;
    private LevelNode node;

    public int DepthIndex
    {
        get { return depth_index; }
    }

    public LevelNode Node
    {
        get { return node; }
    }

    public NodeDepth(int depth_index, LevelNode node)
    {
        this.depth_index = depth_index;
        this.node = node;
    }
}

public class GenerateGrid : Subject
{
    public Transform m_TopRight;
    public Transform m_TopLeft;

    public int m_SquaresPerFace;

    private float PLANE_SIDELENGTH;
    private float GRIDSQUARE_SIDELENGTH;

    private List<GridSquare> m_GridSquares = new List<GridSquare>();
    PathFinderGraph<LevelNode> graph; //the graph used for pathfinding

    public GameObject level_node_prefab;
    public GameObject m_GraphContainer;

    public Camera cam;

    private List<LevelNode> player_base_nodes;
    private List<LevelNode> enemy_base_nodes;

    public GameObject test_tower;
    public GameObject terrain;
    public GameObject terrainRayCaster;
    public float y_offset_for_connection;

    public int NeighborDepthForSpawnBuilding = 2;
    
    [Header("Build Menu Prefab")]
    public GameObject BuildMenuPrefab;
    public GameObject ManageMenuPrefab;
    public GameObject Canvas;

    PlayerMovement playerScriptRef;

    private void AddPlayerBaseNode(LevelNode n)
    {
        player_base_nodes.Add(n);
    }

    private void AddEnemyBaseNode(LevelNode n)
    {
        enemy_base_nodes.Add(n);
    }

    public List<LevelNode> PlayerBaseNodes
    {
        get { return player_base_nodes; }
    }

    public List<LevelNode> EnemyBaseNodes
    {
        get { return enemy_base_nodes; }
    }

    public Graph<LevelNode> Graph
    {
        get { return graph; }
    }

    public List<GridSquare> GridSquares
    {
        get { return m_GridSquares; }
    }

    void Awake()
    {
        player_base_nodes = new List<LevelNode>();
        enemy_base_nodes = new List<LevelNode>();

        this.getPlaneDimensions();
        graph = new PathFinderGraph<LevelNode>();

        this.initializeSquares();

        //add each of the characters to the observer list
        foreach(Character c in FindObjectsOfType<Character>())
        {
            AttachObserver(c);
        }

        //playerScriptRef = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

    }

    private void Start()
    {
        //playerScriptRef = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    private bool CanBuild(LevelNode n)
    {
        if (PlayerBaseNodes.Contains(n) || EnemyBaseNodes.Contains(n))
            return false;

        int i = 0;
        List<NodeDepth> node_depths = new List<NodeDepth>();
        
        //first place the player and enemy base nodes in this list
        foreach(LevelNode ln in PlayerBaseNodes)
        {
            node_depths.Add(new NodeDepth(i, ln));
        }

        foreach(LevelNode ln in EnemyBaseNodes)
        {
            node_depths.Add(new NodeDepth(i, ln));
        }

        //now for each node already in the list and whose depth is equal to the current depth i
        //add all its neighbors with depth i + 1, then increment i when all have been added. do as long as i is
        //less than the max depth for search
        for(i = 0; i < NeighborDepthForSpawnBuilding; i++)
        {
            for(int j = 0; j < node_depths.Count; j++)
            {
                if(node_depths[j].DepthIndex == i)
                {
                    foreach(GraphNode<LevelNode> gn in node_depths[j].Node.GraphNode.Neighbors)
                    {
                        node_depths.Add(new NodeDepth(i + 1, gn.Value));
                    }
                }
            }
        }

        foreach(NodeDepth nd in node_depths)
        {
            if(nd.Node == n)
            {
                return false;
            }
        }

        return true;

    }


    void Update()
    {
        if(playerScriptRef==null)
            playerScriptRef = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        if (Input.GetMouseButtonDown(0) && playerScriptRef.inBuildMode && !playerScriptRef.building && !playerScriptRef.managingTower && !playerScriptRef.isDead)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.GetComponent<LevelNode>() && CanBuild(hit.transform.gameObject.GetComponent<LevelNode>()))
                {
                    if (hit.transform.gameObject.GetComponent<LevelNode>().Open)
                    {
                        //spawn menu
                        GameObject gb = Instantiate(BuildMenuPrefab, BuildMenuPrefab.transform.position, Quaternion.identity);
                        gb.transform.SetParent(Canvas.transform, false);
                        gb.GetComponent<BuildMenu>().spawnPos = hit.transform;
                    }
                }
                else if (hit.transform.tag == "Tower")
                {
                    //Debug.Log("Clicked tower");
                    playerScriptRef.managingTower = true;
                    GameObject gb = Instantiate(ManageMenuPrefab);
                    //Debug.Log("Parent of tower: "+hit.transform.parent.name);
                    gb.GetComponent<ManageMenu>().currentTower = hit.transform.gameObject;
                    gb.GetComponent<ManageMenu>().InitializeMenu();
                }
            }

        }
        
    }

    public void PlaceTower(GameObject towerPrefab, Transform hitLocation)
    {
        GameObject tower = Instantiate(towerPrefab, hitLocation.transform.position, Quaternion.identity);
        hitLocation.transform.gameObject.GetComponent<LevelNode>().Tower = tower;
        hitLocation.transform.gameObject.GetComponent<LevelNode>().ToggleOpen();
        tower.transform.parent = hitLocation.transform.gameObject.transform;

        Notify();
    }

    public void DestroyTower(GameObject Tower, int refund)
    {
        Tower.transform.parent.GetComponent<LevelNode>().ToggleOpen();
        playerScriptRef.AddGold(refund);

        Destroy(Tower.gameObject);

        Notify();
    }

    private List<GraphNode<LevelNode>> getNeighbors(GridSquare square)
    {
        List<GraphNode<LevelNode>> neighbors = new List<GraphNode<LevelNode>>();
        foreach(GraphNode<LevelNode> n in graph.Nodes)
        {
            if (n.Value.GridSquare.IsNeighbor(square) && !neighbors.Contains(n))
                neighbors.Add(n);
        }

        return neighbors;
    }

    void initializeSquares()
    {
        this.GRIDSQUARE_SIDELENGTH = this.PLANE_SIDELENGTH / (float)m_SquaresPerFace;
        Vector3 start = this.m_TopLeft.position;
        for (int i = 0; i < m_SquaresPerFace; i++)
        {
            for (int j = 0; j < m_SquaresPerFace; j++)
            {
                float x = start.x + (GRIDSQUARE_SIDELENGTH / 2.0f) + (GRIDSQUARE_SIDELENGTH * i);
                float z = start.z - (GRIDSQUARE_SIDELENGTH / 2.0f) - (GRIDSQUARE_SIDELENGTH * j);

                RaycastHit hit;
                float y = this.transform.position.y;

                //we need to linecast to the terrain to determine the y value at that x z position
                if(Physics.Linecast(terrainRayCaster.transform.position, new Vector3(x, this.transform.position.y, z), out hit))
                {                   
                    y = hit.point.y;
                }

                //check if the hit should have a node on it. If so add a grid square otherwise skip it
                if(hit.collider.tag != "NoNode")
                    this.m_GridSquares.Add(new GridSquare(new Vector3(x, y, z), new GridCoordinate(i, j)));
            }
        }

        //when the squares are initialized they also need to be added to the graph for pathfinding
        foreach(GridSquare s in m_GridSquares)
        {
            GameObject node = Instantiate(level_node_prefab, s.Position, Quaternion.identity);
            node.transform.parent = this.m_GraphContainer.transform;

            LevelNode l = node.GetComponent<LevelNode>();
            l.GridSquare = s;

            s.Node = l.GraphNode;

            graph.Add(l.GraphNode);

            //we need to check if the node is a base node for the player or the enemy
            //if we have m_SquaresPerFace even, then there will be two nodes that consist each camp
            //they are the two nodes in the middle of one side
            //otherwise its the node in the middle of the side
            if(s.Coordinate.Column == 0 || s.Coordinate.Column == (m_SquaresPerFace - 1))
            {
                if (m_SquaresPerFace % 2 == 0)
                {
                    //check if we are in the middle two nodes
                    if (s.Coordinate.Row == m_SquaresPerFace / 2 || s.Coordinate.Row == m_SquaresPerFace / 2 - 1)
                    {
                        l.IsBaseNode = true;

                        if (s.Coordinate.Column == 0)
                            AddEnemyBaseNode(l);

                        else
                            AddPlayerBaseNode(l);

                    }
                }

                else
                {
                    if (s.Coordinate.Row == Mathf.Floor(m_SquaresPerFace / 2))
                    {
                        l.IsBaseNode = true;

                        if (s.Coordinate.Column == 0)
                            AddEnemyBaseNode(l);

                        else
                            AddPlayerBaseNode(l);
                    }
                        
                }
            }
        }

        //now we need to connect neighbors
        foreach(GraphNode<LevelNode> n in graph.Nodes)
        {
            foreach(GraphNode<LevelNode> g in getNeighbors(n.Value.GridSquare))
            {
                //we need to check if its actually possible to get from node to node first. If the angle of the line connection the two nodes is too large in the y direction, then we should not add the neighbor (will have to go up a straight wall)
                //we will take the difference between this nodes position and the neighbor node
                //if (Mathf.Abs((g.Value.transform.position - n.Value.transform.position).y) > max_y_offset_for_connection)
                    //continue;


       
                RaycastHit hit;

                if (Physics.Linecast(new Vector3(n.Value.transform.position.x, n.Value.transform.position.y + y_offset_for_connection, n.Value.transform.position.z), 
                                     new Vector3(g.Value.transform.position.x, g.Value.transform.position.y + y_offset_for_connection, g.Value.transform.position.z), 
                                     out hit))
                {
                    if (hit.collider.tag == "GridObstacle")
                        continue;
                }
            

                n.AddNeighbor(g, n.Value.ComputeHeuristic(g.Value));

                n.Value.AddLineRenderer();
            }
        }

        
    }

    //A function to find out the dimensions of our floor plane. 
    void getPlaneDimensions()
    {
        this.PLANE_SIDELENGTH = (this.m_TopRight.position - this.m_TopLeft.position).magnitude;
    }
}
