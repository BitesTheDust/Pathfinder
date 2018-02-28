using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGridManager : MonoBehaviour
{
    public LayerMask m_ObstacleMask;
    public LayerMask m_PlayerMask;
    public Vector2 m_vGridSize;
    public float m_fHalfNodeWidth;

    Node[,] m_aGrid;

    public List<Node> m_aPath;
    List<Node> neighbors;
    Node playerNode;

    public int NodeCount 
    {
        get { return ((int) m_vGridSize.x) * ((int) m_vGridSize.y); }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(m_vGridSize.x, 1, m_vGridSize.y));

        if(m_aGrid != null)
        {
            foreach (Node node in m_aGrid)
            {
                bool playerCollide = false;
                playerCollide = Physics.CheckSphere(node.m_vPosition, m_fHalfNodeWidth * 0.1f, m_PlayerMask);

                Gizmos.color = node.m_bIsblocked ? Color.red : Color.white;

                if(playerCollide)
                {
                    playerNode = node;
                    Gizmos.color = Color.magenta;
                }

                if (neighbors.Contains(node))
                {
                    Gizmos.color = Color.blue;
                }

                if (m_aPath != null)
                {
                    if(m_aPath.Contains(node))
                    {
                        Gizmos.color = Color.black;
                    }
                }

                Gizmos.DrawWireCube(node.m_vPosition, new Vector3(m_fHalfNodeWidth * 2, 1, m_fHalfNodeWidth * 2));
            }
        }
    }

    private void Start()
    {
        CreateGrid();

        neighbors = new List<Node>();
        m_aPath = new List<Node>();
    }

    private void Update()
    {
        if (m_aGrid != null && playerNode != null)
        {
            neighbors = GetNeighbours(playerNode);
        }
    }

    public void CreateGrid()
    {
        m_aGrid = new Node[(int)m_vGridSize.x, (int)m_vGridSize.y];

        Vector2 currentGridPos = new Vector2();

        for (int iterX = 0; iterX < m_vGridSize.x; iterX++)
        {
            for (int iterY = 0; iterY < m_vGridSize.y; iterY++)
            {
                currentGridPos = new Vector2((-m_vGridSize.x / 2) + iterX, (-m_vGridSize.y / 2) + iterY);
                Vector3 newNodePos = new Vector3(currentGridPos.x + m_fHalfNodeWidth, transform.position.y, currentGridPos.y + m_fHalfNodeWidth);

                bool isBlocked = false;
                isBlocked = Physics.CheckSphere(newNodePos, m_fHalfNodeWidth * 0.1f, m_ObstacleMask);

                Node newNode = new Node(isBlocked, newNodePos, m_vGridSize, iterX, iterY);
                m_aGrid[iterX, iterY] = newNode;
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int iCheckX = node.m_iGridX + x;
                int iCheckY = node.m_iGridY + y;

                if (iCheckX >= 0 && iCheckY >= 0 && iCheckX < m_vGridSize.x && iCheckY < m_vGridSize.y)
                {
                    neighbours.Add(m_aGrid[iCheckX, iCheckY]);
                }
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPos(Vector3 pos)
    {
        Node foundNode = m_aGrid[0, 0];
        float lowDistance = Vector3.Distance(foundNode.m_vPosition, pos); ;

        foreach (Node node in m_aGrid)
        {
            float newDistance = Vector3.Distance(node.m_vPosition, pos);

            if(newDistance < lowDistance)
            {
                foundNode = node;
                lowDistance = newDistance;
            }
        }

        return foundNode;
    }
}
