using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
    public enum SelectingState
    {

    }
    public class SelectManager : MonoBehaviour
    {
        GameManager GameManager;

        [HideInInspector]
        //public Hero SelectedHero, DragHero;     //Drag Hero is the hero of dragging
        public GameObject SelectedObject, DragObject, ParentObject;
        [HideInInspector]
        //public HeroPlace SelectPlace;
        bool isDrag;
        //public TFTGameManager gameManager { get; private set; }

        Plane movePlane;
        [SerializeField]
        float fixedDistance = 0f;
        float hitDist, t;
        Ray camRay;
        Vector3 startPos, point, corPoint;

        public static SelectManager Instance;

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
            if (Input.GetMouseButtonDown(0) && SelectedObject != null && DragObject == null)
            {
                DragObject = SelectedObject;
                DragObject.GetComponent<Collider>().enabled = false;
                DragHero.LastHeroPlace = DragHero.HeroPlace;
                DragHero.SelectingBox.SetActive(true);

                startPos = DragObject.transform.position; // save position in case draged to invalid place
                movePlane = new Plane(-Camera.main.transform.forward, DragObject.transform.position);
            }
            //Put down Hero
            if (Input.GetMouseButtonUp(0) && DragObject != null)
            {
                isDrag = !isDrag;
                if (ParentObject != null)
                    //!(
                    //    SelectPlace.PlaceType == PlaceType.OnBoard &&
                    //    GameManager.PlayerHero.GameBoardHeroes.Count >= GameManager.LevelManager.Level &&
                    //    DragHero.LastHeroPlace.PlaceType == PlaceType.NonBoard
                    //    )
                    //)
                {

                    if (!isDrag)
                    {
                        //DragHero.ChangeStatus();
                        DragHero.HeroPlace = DragHero.transform.parent.GetComponent<HeroPlace>();

                        GameManager.ChangeHeroPos(ref DragHero);
                        DragHero.GetComponent<Collider>().enabled = true;
                        DragObject.transform.localPosition = Vector3.zero;
                        DragHero.SelectingBox.SetActive(false);

                        DragObject = null;
                        ParentObject = null;

                    }
                }
                else
                {
                    isDrag = true;
                }


            }
            //Dragging Hero Place
            else if (isDrag && DragObject != null)
            {
                if (DragObject != null && DragObject.transform.childCount == 0)
                {
                    DragObject.transform.parent = ParentObject.transform;
                }

                camRay = GameManager.MainCamera.ScreenPointToRay(Input.mousePosition); // shoot a ray at the obj from mouse screen point

                // finde the collision on movePlane
                if (movePlane.Raycast(camRay, out hitDist))
                {
                    point = camRay.GetPoint(hitDist);                                       // define the point on movePlane
                    t = -(fixedDistance - camRay.origin.y) / (camRay.origin.y - point.y);   // the x,y or z plane you want to be fixed to

                    #region calculate the new point t futher along the ray
                    corPoint.x = camRay.origin.x + (point.x - camRay.origin.x) * t;
                    corPoint.y = camRay.origin.y + (point.y - camRay.origin.y) * t;
                    corPoint.z = camRay.origin.z + (point.z - camRay.origin.z) * t;
                    #endregion

                    //DragObject.HeroPlace = ParentObject;
                    DragObject.transform.position = corPoint;
                }
            }
        }
    }

}
