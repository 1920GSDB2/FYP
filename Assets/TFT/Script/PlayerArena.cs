using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
    public class PlayerArena : MonoBehaviour
    {
        public GameObject FightArena;
        public GamePlace SelfArena, EnemyArena;
        public GameObject Camera;
        Vector3 leftTop, rightTop, leftDown, rightDown;
        int GridSizeY = 8;
        int GridSizeX = 7;
        // Start is called before the first frame update
        void Start()
        {
            SetCorner();
            setGridPosition();
        }

        // Update is called once per frame
        void Update()
        {

        }
        private void setGridPosition()
        {
            
           // Debug.Log("Place " + EnemyArena.GameBoard.GetChild(11).GetComponent<HeroPlace>().PlaceId + " x " + 1 + " y " + 1);
            int child = SelfArena.GameBoard.childCount - 1;
            for (int y = GridSizeY - 1; y >= 0; y--)
            {
                for (int x = 0; x < GridSizeX; x++)
                {
                    if (y > 3)
                    {
                        try
                        {      
                            EnemyArena.GameBoard.GetChild(child).GetComponent<HeroPlace>().setGridPosition(x, y);
                        }
                        catch (NullReferenceException) {
                            Debug.Log(" X " + x + " Y " + y + " child " + child);
                        }
                        if (child != 0)
                            child--;
                    }
                    else
                    {
                        SelfArena.GameBoard.GetChild(child).GetComponent<HeroPlace>().setGridPosition(x, y);
                        child++;
                    }
                    //Debug.Log("X " + x + " Y " + y+" child "+child);
                }
            }
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
}

