using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HeroPlace : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    MouseSelect MouseSelect;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        MouseSelect = GameObject.FindGameObjectWithTag("GameManager").GetComponent<MouseSelect>();
    }

    private void OnMouseOver()
    {
        spriteRenderer.color = Color.black;
        if (MouseSelect.DragHero != null)
        {
            MouseSelect.SelectPlace = this;
        }
    }

    private void OnMouseExit()
    {
        spriteRenderer.color = Color.white;
        MouseSelect.SelectPlace = null;
    }
}
