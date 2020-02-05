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
    MouseSelect MouseSelect;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        HeroPlaceSetting();
    }
    
    void HeroPlaceSetting()
    {
        if (name.Equals("Hexagon")) PlaceType = PlaceType.OnBoard;
        PlaceId = transform.GetSiblingIndex();
    }
    #region HoverEffect
    void Update()
    {
        MouseSelect = GameObject.FindGameObjectWithTag("GameManager").GetComponent<MouseSelect>();
    }

    void OnMouseOver()
    {
        if (!(MouseSelect.gameManager.GameStatus == GameStatus.Playing && name.Equals("Hexagon")))
        {
            spriteRenderer.color = Color.black;
            if (MouseSelect.DragHero != null)
            {
                MouseSelect.SelectPlace = this;
            }
        }
    }

    void OnMouseExit()
    {
        spriteRenderer.color = Color.white;
        MouseSelect.SelectPlace = null;
    }
    #endregion
}
