using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_Star : MonoBehaviour {

    public Transform m_tSeeker, m_tTarget;
    PathGridManager m_Grid;

    private void Awake()
    {
        m_Grid = GetComponent<PathGridManager>();
    }

    public int GetDistance(Node a, Node b)
    {
        int iDistX = Mathf.Abs(a.m_iGridX - b.m_iGridX);
        int iDistY = Mathf.Abs(a.m_iGridY - b.m_iGridY);

        if (iDistX > iDistY)
        {
            return 14 * iDistY + 10 * (iDistX - iDistY);
        }
        else
        {
            return 14 * iDistX + 10 * (iDistY - iDistX);
        }
    }
    
    public void FindPath(Vector3 start, Vector3 end)
    {
        Node startNode = m_Grid.NodeFromWorldPos(start);
        Node endNode = m_Grid.NodeFromWorldPos(end);

        Heap<Node> openSet = new Heap<Node>(m_Grid.NodeCount);
        List<Node> closedSet = new List<Node>();

        openSet.Add(startNode);

        while(openSet.Count > 0)
        {
            Node current = openSet.RemoveFirst();
            closedSet.Add(current);

            if(current == endNode)
            {
                RetracePath(startNode, endNode);
                return;
            }

            foreach(Node node in m_Grid.GetNeighbours(current))
            {
                if (node.m_bIsblocked || closedSet.Contains(node))
                    continue;

                int newMovementCost = current.m_iGCost + GetDistance(current, node);

                if(newMovementCost < node.m_iGCost || !openSet.Contains(node))
                {
                    node.m_iGCost = newMovementCost;
                    node.m_iHCost = GetDistance(node, endNode);
                    node.m_Parent = current;

                    if (!openSet.Contains(node)) 
                    {
                        openSet.Add(node);
                    }
                    else 
                    {
                        openSet.UpdateItem(node);
                    }
                }
            }
        }
    }

    public void RetracePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node current = end;

        while(current != start)
        {
            path.Add(current);
            current = current.m_Parent;
        }

        path.Reverse();

        m_Grid.m_aPath = path;
    }
	
	void Update ()
    {
        if(m_Grid != null)
            FindPath(m_tSeeker.position, m_tTarget.position);
	}
}
