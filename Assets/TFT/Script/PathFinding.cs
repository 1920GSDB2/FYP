using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    GridMap gridMap;
    Hero self;
    public Hero target;
    public Hero testHero;

    private void Awake()
    {
        gridMap = GetComponent<GridMap>();
    }
    private void Start()
    {
        self = GetComponent<Hero>();
    }
    private void Update()
    {
         if (Input.GetKeyDown(KeyCode.K)) {
            //Hero hero = GetComponent<Hero>();
            //test(hero.HeroPlace);
            target = testHero;
        }
        if (target != null) {
            findPath(self.HeroPlace, target.HeroPlace);
        }
       
    }
    void findPath(HeroPlace startPoint,HeroPlace endPoint) {        
        Node startNode = gridMap.getHeroPlaceGrid(startPoint);
        Node targetNode = gridMap.getHeroPlaceGrid(endPoint);
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);
        
        while (openSet.Count > 0) {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++) {
                if (openSet[i].fCost < currentNode.fCost ||openSet[i].fCost==currentNode.fCost&& openSet[i].hCost <= currentNode.hCost) {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            if (currentNode == targetNode)
            {
                trackPath(startNode,targetNode);
                return;
            }

            foreach (Node neighbour in gridMap.getNeighbours(currentNode)) {
                if (!neighbour.isWalkable || closedSet.Contains(neighbour))
                {
                    continue;
                }
                int neighbourDis = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (neighbourDis < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = neighbourDis;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
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
       return totalDis+disX * 10;
    }
    void trackPath(Node startNode,Node targetNode) {
        List<Node> path = new List<Node>();
        Node currentNode = targetNode;
        while (currentNode!= startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        path.RemoveAt(path.Count-1);
    
        gridMap.path = path;
        target = null;
    }
}
