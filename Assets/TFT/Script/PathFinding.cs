using System;
using System.Collections;
using System.Collections.Generic;
using TFT;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    GridMap gridMap;
   // public List<Node> path = new List<Node>();

    private void Awake()
    {
        gridMap = GetComponent<GridMap>();
    }
  
    public void findPath(HeroPlace startPoint, HeroPlace endPoint, Action<List<Node>, bool> callback)
    {
        Node startNode = gridMap.getHeroPlaceGrid(startPoint);
        Node targetNode = gridMap.getHeroPlaceGrid(endPoint);
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        List<Node> path = null;
        openSet.Add(startNode);
        bool isFindPath = false;

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost <= currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            if (currentNode == targetNode)
            {
                isFindPath = true;
                break;
            }

            foreach (Node neighbour in gridMap.getNeighbours(currentNode))
            {
                //  Debug.Log("Check "+(neighbour == targetNode)+" Ne "+neighbour.isWalkable+" Ta "+targetNode.isWalkable);
                if (!neighbour.isWalkable || closedSet.Contains(neighbour))
                {
                    if (!(neighbour == targetNode))
                        continue;
                }
                int neighbourDis = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (neighbourDis < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = neighbourDis;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
        if (isFindPath)
        {
            path = trackPath(startNode, targetNode);
            if (path.Count == 0)
            {
                path = null;
                isFindPath = false;
            }
        }

        PathFindingManager.Instance.pathFinish(path, isFindPath, callback);

    }
    public void findNextStep(HeroPlace startPoint, HeroPlace endPoint, Action<Node, bool> callback)
    {
        Node startNode = gridMap.getHeroPlaceGrid(startPoint);
        Node targetNode = gridMap.getHeroPlaceGrid(endPoint);

     
        bool isStepFind = false;
        Node currentNode = startNode;

        List<Node> checkNode = new List<Node>();
       
        foreach (Node neighbour in gridMap.getNeighbours(currentNode))
        {
            if (!neighbour.isWalkable )
            {
                continue;
            }
               // neighbour.gCost = GetDistance(currentNode, neighbour);
                neighbour.hCost = GetDistance(neighbour, targetNode);
                checkNode.Add(neighbour);
         //   Debug.Log("FCost " + neighbour.fCost + " " + currentNode.fCost + "s hCost " + neighbour.hCost + " " + currentNode.hCost+" X "+neighbour.gridX+" Y "+neighbour.gridY);         
        }
        currentNode = checkNode[0];
        for (int i = 1; i < checkNode.Count; i++)
        {
            if (checkNode[i].fCost < currentNode.fCost || checkNode[i].fCost == currentNode.fCost && checkNode[i].hCost <= currentNode.hCost)
            {
                currentNode = checkNode[i];
                isStepFind = true;
            }
        }

        PathFindingManager.Instance.nextStepFinish(currentNode, isStepFind, callback);
    }

    int GetDistance(Node nodeA,Node nodeB) {
        int disX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int disY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        int totalDis = disY * 11;

        while (disY >= 2)
        {
            if (disX == 0)
                break;
            disX--;
            disY -= 2;
        }
        if (disY == 1 && disX == 1)
            disX = 0;
       return totalDis+disX * 10;
    }

    List<Node> trackPath(Node startNode,Node targetNode) {
        List<Node> path = new List<Node>();
        Node currentNode = targetNode;
        while (currentNode!= startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        path.RemoveAt(path.Count-1);
        return path;
      //  this.path = path;
        //target = null;
    }
}
