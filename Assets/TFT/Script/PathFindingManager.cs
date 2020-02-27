using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingManager : MonoBehaviour
{
    PathFinding pathFindingTool;
    public static PathFindingManager Instance;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        pathFindingTool = GetComponent<PathFinding>();
    }

    public void requestPath(HeroPlace starPoint,HeroPlace endPoint,Action<List<Node>> callback) {
        List<Node> path=pathFindingTool.findPath(starPoint, endPoint);
        callback(path);
    }
}
