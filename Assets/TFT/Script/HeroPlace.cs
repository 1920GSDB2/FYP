using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlaceType
{
    NonBoard,
    OnBoard
}

[RequireComponent(typeof(SpriteRenderer))]
public class HeroPlace : MonoBehaviour
{
    public PlaceType PlaceType { get; private set; }
    public int PlaceId { get; private set; }
    SpriteRenderer spriteRenderer;

    public int gridX;
    public int gridY;
    public bool isWalkable = true;
    //MouseSelect MouseSelect;

    public void setGridPosition(int x,int y) {
        gridX = x;
        gridY = y;
    }
  
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        HeroPlaceSetting();
        //MouseSelect = TFT.GameManager.Instance.gameObject.GetComponent<MouseSelect>();
    }



    void HeroPlaceSetting()
    {
        if (name.Equals("Hexagon")) PlaceType = PlaceType.OnBoard;
        PlaceId = transform.GetSiblingIndex();
    }
    #region HoverEffect
    void Update()
    {
    }

    void OnMouseOver()
    {
        if (!(TFT.GameManager.Instance.GameStatus == GameStatus.Playing && name.Equals("Hexagon")))
        {
            spriteRenderer.color = Color.black;
            if (MouseSelect.Instance.DragHero != null)
            {
                MouseSelect.Instance.SelectPlace = this;
            }
        }
    }

    void OnMouseExit()
    {
        spriteRenderer.color = Color.white;
        MouseSelect.Instance.SelectPlace = null;
    }
    public void settColor(Color color) {
        spriteRenderer.color = color;
    } 
    #endregion
}
