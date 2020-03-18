using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{

    public class MouseSelect : MonoBehaviour
    {
        GameManager GameManager;

        //[HideInInspector]
        public Hero SelectedHero, DragHero;     //Drag Hero is the hero of dragging
        //[HideInInspector]
        public HeroPlace SelectPlace;
        bool isDrag;
        //public TFTGameManager gameManager { get; private set; }

        Plane movePlane;
        [SerializeField]
        float fixedDistance = 0f;
        float hitDist, t;
        Ray camRay;
        [SerializeField]
        Vector3 startPos, point, corPoint;

        public static MouseSelect Instance;

        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
            GameManager = GameManager.Instance;
            //gameManager = GetComponent<TFTGameManager>();
        }

        // Update is called once per frame
        void Update()
        {
            //Take-up Hero
            if (Input.GetMouseButtonDown(0) && SelectedHero != null && DragHero == null)
            {
                DragHero = SelectedHero;
                DragHero.GetComponent<Collider>().enabled = false;
                DragHero.LastHeroPlace = DragHero.HeroPlace;

                startPos = DragHero.transform.position; // save position in case draged to invalid place
                movePlane = new Plane(-Camera.main.transform.forward, DragHero.transform.position);
            }
            //Put down Hero
            if (Input.GetMouseButtonUp(0) && DragHero != null )
            {
                if (SelectPlace.PlaceType == PlaceType.OnBoard && GameManager.PlayerHero.GameBoardHeroes.Count >= GameManager.LevelManager.Level)
                {
                    ;
                }
                else
                {
                    isDrag = !isDrag;
                    if (!isDrag)
                    {
                        //DragHero.ChangeStatus();
                        DragHero.HeroPlace = DragHero.transform.parent.GetComponent<HeroPlace>();

                        GameManager.ChangeHeroPos(ref DragHero);
                        DragHero.GetComponent<Collider>().enabled = true;
                        DragHero.transform.localPosition = Vector3.zero;
                        DragHero = null;
                        SelectPlace = null;

                    }
                }
                

            }
            //Dragging Hero Place
            else if (isDrag && DragHero != null)
            {
                if (SelectPlace != null && SelectPlace.transform.childCount == 0)
                {
                    DragHero.transform.parent = SelectPlace.transform;
                }

                camRay = GameManager.MainCamera.ScreenPointToRay(Input.mousePosition); // shoot a ray at the obj from mouse screen point

                // finde the collision on movePlane
                if (movePlane.Raycast(camRay, out hitDist))
                {
                    point = camRay.GetPoint(hitDist);                                       // define the point on movePlane
                    t = -(fixedDistance - camRay.origin.y) / (camRay.origin.y - point.y);   // the x,y or z plane you want to be fixed to
                    
                    //Debug.Log("MousePos: " + Input.mousePosition);
                    //Debug.Log("Point: " + point);
                    //Debug.Log("camRay: " + camRay);

                    #region calculate the new point t futher along the ray
                    corPoint.x = camRay.origin.x + (point.x - camRay.origin.x) * t;
                    corPoint.y = camRay.origin.y + (point.y - camRay.origin.y) * t;
                    corPoint.z = camRay.origin.z + (point.z - camRay.origin.z) * t;
                    #endregion

                    DragHero.HeroPlace = SelectPlace;
                    DragHero.transform.position = corPoint;

                    //DragHero.transform.localPosition.Set(DragHero.transform.localPosition.x, 1, DragHero.transform.localPosition.z);
                    //Debug.Log("Hero Position: " + DragHero.transform.position);
                    //Debug.Log("Hero Local Position: " + DragHero.transform.localPosition);
                    //Debug.Log("Hero Local Scale: " + DragHero.transform.localScale);
                }
            }
        }

    }

}