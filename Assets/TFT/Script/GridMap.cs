using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFT;

public class GridMap : MonoBehaviour
{
    public PlayerArena playerArena;
    public int GridSizeX = 7;
    public int GridSizeY = 8;
    public Node[,] grid;
    //public List<Node> path = new List<Node>();

    void Start()
    {
        createGrid();

    }
    public void setPlayerArena(PlayerArena playerMap) {
        playerArena = playerMap;

        createGrid();
    }


  
    public void createGrid() {
        grid = new Node[GridSizeX, GridSizeY];
       

        int child = playerArena.SelfArena.GameBoard.childCount - 1;

        for (int y = GridSizeY - 1; y >= 0; y--)
        {
            for (int x = 0; x < GridSizeX; x++)
            {
                HeroPlace heroPlace;
                if (y > 3)
                {
                    heroPlace = playerArena.EnemyArena.GameBoard.GetChild(child).GetComponent<HeroPlace>();

                    if (child != 0)
                        child--;
                }
                else
                {
                    heroPlace = playerArena.SelfArena.GameBoard.GetChild(child).GetComponent<HeroPlace>();
                    child++;
                }
                Node newNode= new Node(x, y, heroPlace);
                grid[x, y] = newNode;
              //  Debug.Log("Grid X "+x+"Y "+y+" data "+grid[x,y]+" testINt ");
            }
        }     
    }
    public Node getHeroPlaceGrid(HeroPlace heroPlace)
    {
       // Debug.Log(heroPlace.gridX + " , " + heroPlace.gridY+" Place " + grid[heroPlace.gridX, heroPlace.gridY]);
        return grid[heroPlace.gridX, heroPlace.gridY];
    }
    public List<Node> getNeighbours(Node node) {
        List<Node> neighbours = new List<Node>();
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (x == 0 && y == 0)
                    continue;
                int checkX = x+node.gridX;
                int checkY = y+node.gridY;

                if (checkX < 0 || checkX > GridSizeX-1 || checkY < 0 || checkY > GridSizeY-1)
                    continue;

                if (node.gridY % 2 == 0)
                {
                    if (y != 0&& x == 1)
                        continue;                   
                }
                else
                {
                    if (y !=0 && x == -1)
                        continue;                  
                }
                neighbours.Add(grid[checkX, checkY]);
            }
        }
        return neighbours;
    }
    public void resetMap() {
        int child = playerArena.SelfArena.GameBoard.childCount - 1;
        for (int y = GridSizeY - 1; y >= 0; y--)
        {
            for (int x = 0; x < GridSizeX; x++)
            {
                HeroPlace heroPlace;
                if (y > 3)
                {
                    heroPlace = playerArena.EnemyArena.GameBoard.GetChild(child).GetComponent<HeroPlace>();
                    
                    if (child != 0)
                        child--;
                }
                else
                {
                    heroPlace = playerArena.SelfArena.GameBoard.GetChild(child).GetComponent<HeroPlace>();
                    child++;
                }
                heroPlace.isWalkable = true;
            }
        }
    }
    public int GetDistance(Node nodeA, Node nodeB)
    {
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
        return totalDis + disX * 10;
    }
}
