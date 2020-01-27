using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArena : MonoBehaviour
{
    public GameObject FightArena;
    public GameObject SelfArena, EnemyArena;
    Vector3 leftTop, rightTop, leftDown, rightDown;
    // Start is called before the first frame update
    void Start()
    {
        SetCorner();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetCorner()
    {
        Vector3 boundExtents = FightArena.GetComponent<Renderer>().bounds.extents * 2;
        leftTop = FightArena.transform.position;
        leftDown = new Vector3(leftTop.x, leftTop.y, leftTop.z + boundExtents.z);
        rightTop = new Vector3(leftTop.x + boundExtents.x, leftTop.y, leftTop.z);
        rightDown = new Vector3(rightTop.x + boundExtents.x, rightTop.y, rightTop.z);
    }
}
