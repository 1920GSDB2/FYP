using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool isWalkable { get { return heroPlace.isWalkable; } }

    public HeroPlace heroPlace { get; private set; }
    public Node parent;
    public int gridX { get; private set; }
    public int gridY { get; private set; }
    public int gCost, hCost;
    public int fCost
    {
        get { return gCost + hCost; }
    }

    public Node(int x, int y,HeroPlace heroPlace)
    {
        gridX = x;
        gridY = y;
        this.heroPlace = heroPlace;
    }
}
