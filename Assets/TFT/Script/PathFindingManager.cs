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

    public void requestPath(HeroPlace starPoint,HeroPlace endPoint,Action<List<Node>,bool> callback) {
       pathFindingTool.findPath(starPoint, endPoint,callback);    
    }
    public void requestNextStep(HeroPlace starPoint, HeroPlace endPoint, Action<Node, bool> callback)
    {
        pathFindingTool.findNextStep(starPoint, endPoint, callback);
    }
    public void pathFinish(List<Node> path,bool isPathFind, Action<List<Node>, bool> callback) {
        callback(path,isPathFind);
    }
    public void nextStepFinish(Node step, bool isStepFind, Action<Node, bool> callback)
    {
        callback(step, isStepFind);
    }
}
