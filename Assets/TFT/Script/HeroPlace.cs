using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlaceType
{
    OnBoard,
    NonBoard
}

namespace TFT
{

    public class HeroPlace : MonoBehaviour
    {
        public PlaceType PlaceType { get; private set; }
        public int PlaceId;
        SpriteRenderer spriteRenderer;
        MeshRenderer currMat;
        public Material defaultMat, hoverMat;
        [SerializeField]
        bool isSelect;

        public int gridX { get; private set; }
        public int gridY { get; private set; }
        public bool isWalkable = true;

        GameManager GameManager;
        SelectManager SelectManager;
        //MouseSelect MouseSelect;

        public void setGridPosition(int x, int y)
        {
            gridX = x;
            gridY = y;
        }

        // Start is called before the first frame update
        void Start()
        {
            GameManager = GameManager.Instance;
            SelectManager = SelectManager.Instance;
            HeroPlaceSetting();
            //MouseSelect = TFT.GameManager.Instance.gameObject.GetComponent<MouseSelect>();
            PlayerArena playerArena = transform.parent.parent.parent.GetComponent<PlayerArena>();
            if(transform.parent.parent.tag.Equals("SelfArena") &&
                playerArena == GameManager.SelfPlayerArena)
            {
                isSelect = true;
            }
        }

        void HeroPlaceSetting()
        {
            if (name.Equals("Square"))
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                PlaceType = PlaceType.NonBoard;
            }
            else
            {
                currMat = GetComponent<MeshRenderer>();
            }
            PlaceId = transform.GetSiblingIndex();
        }
        #region HoverEffect
        void Update()
        {
            if (!isWalkable)
                settColor(Color.green);
        }
        
        void OnMouseOver()
        {
            if ((!(GameManager.GameStatus == GameStatus.Playing && name.Equals("Hexagon")) ||
                GameManager.MainGameManager.isDebugMode) &&
                isSelect)
            {
                if (PlaceType == PlaceType.NonBoard)
                {
                    spriteRenderer.color = Color.black;
                }
                else
                {
                    currMat.material = hoverMat;
                }
                if (SelectManager.DragObject == null ||
                    SelectManager.DragObject as Hero == null ||
                    GameManager.PlayerHero.GameBoardHeroes.Count >= GameManager.LevelManager.Level &&
                    (SelectManager.DragObject as Hero).HeroPlace.PlaceType != PlaceType &&
                    PlaceType == PlaceType.OnBoard)
                {
                    return;
                }
                //if (SelectManager.DragObject != null && SelectManager.DragObject.GetComponent<Hero>() != null)
                else
                {
                    SelectManager.ParentObject = gameObject;
                }
            }
        }

        void OnMouseExit()
        {
            //spriteRenderer.color = Color.white;
            if (PlaceType == PlaceType.NonBoard)
            {
                spriteRenderer.color = Color.white;
            }
            else
            {
                currMat.material = defaultMat;
            }
            SelectManager.ParentObject = null;
        }
        public void settColor(Color color)
        {
            try
            {
                spriteRenderer.color = color;
            }
            catch(Exception)
            {

            }
        }
        public void setHeroOnPlace(Character hero)
        {
            hero.transform.parent = transform;
            isWalkable = false;
        }
        public void leavePlace()
        {
            isWalkable = true;
        }
        #endregion
    }

}