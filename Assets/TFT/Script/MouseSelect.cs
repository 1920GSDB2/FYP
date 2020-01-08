using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSelect : MonoBehaviour
{
    [HideInInspector]
    public Hero SelectedHero, DragHero;     //Drag Hero is the hero of dragging
    [HideInInspector]
    public HeroPlace SelectPlace;
    bool isDrag;
    public TFTGameManager gameManager { get; private set; }

    Plane movePlane;
    [SerializeField]
    float fixedDistance = 0f;
    float hitDist, t;
    Ray camRay;
    Vector3 startPos, point, corPoint;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GetComponent<TFTGameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Take-up Hero
        if (Input.GetMouseButtonDown(0) && SelectedHero != null && DragHero == null)
        {
            DragHero = SelectedHero;
            DragHero.GetComponent<Collider>().enabled = false;

            startPos = DragHero.transform.position; // save position in case draged to invalid place
            movePlane = new Plane(-Camera.main.transform.forward, DragHero.transform.position);
        }
        //Put-up Hero
        if (Input.GetMouseButtonUp(0) && DragHero!=null)
        {
            isDrag = !isDrag;
            if (!isDrag)
            {
                DragHero.ChangeStatus();
                DragHero.GetComponent<Collider>().enabled = true;
                DragHero.transform.localPosition = Vector3.zero;
                DragHero = null;
                SelectPlace = null;
            }
            
        }
        //Select Hero Place
        else if (isDrag && DragHero != null)
        {
            if (SelectPlace != null && SelectPlace.transform.childCount == 0)
            {
                //Debug.Log("Parent Null" +Time.time);
                //if (!(SelectPlace.name.Equals("Hexagon") && gameManager.GameStatus == GameStatus.Playing))
                DragHero.transform.parent = SelectPlace.transform;

            }
            //Vector3 pos = Input.mousePosition;
            //pos.z = transform.position.z - Camera.main.transform.position.z;
            //DragHero.transform.position = Camera.main.ScreenToWorldPoint(pos);

            camRay = Camera.main.ScreenPointToRay(Input.mousePosition); // shoot a ray at the obj from mouse screen point

            if (movePlane.Raycast(camRay, out hitDist))
            { // finde the collision on movePlane
                point = camRay.GetPoint(hitDist); // define the point on movePlane
                t = -(fixedDistance - camRay.origin.y) / (camRay.origin.y - point.y); // the x,y or z plane you want to be fixed to
                corPoint.x = camRay.origin.x + (point.x - camRay.origin.x) * t; // calculate the new point t futher along the ray
                corPoint.y = camRay.origin.y + (point.y - camRay.origin.y) * t;
                corPoint.z = camRay.origin.z + (point.z - camRay.origin.z) * t;
                DragHero.HeroPlace = SelectPlace;
                DragHero.transform.position = corPoint;
            }
        }
    }
    
}
