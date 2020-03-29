using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
    public enum SelectingState
    {

    }
    //public class SelectManager : MonoBehaviour
    //{
    //    GameManager GameManager;

    //    //[HideInInspector]
    //    public GameObject SelectedObject, DragObject, ParentObject;
    //    public bool isDrag;
    //    public bool IsFinishDrag;

    //    [SerializeField]
    //    readonly float fixedDistance = 0f;
    //    Plane movePlane;
    //    float hitDist, t;
    //    Ray camRay;
    //    Vector3 startPos, point, corPoint;

    //    public static SelectManager Instance;

    //    void Awake()
    //    {
    //        Instance = this;
    //    }

    //    // Start is called before the first frame update
    //    void Start()
    //    {
    //        GameManager = GameManager.Instance;
    //    }

    //    // Update is called once per frame
    //    void Update()
    //    {
    //        //Take-up Hero
    //        if (Input.GetMouseButtonDown(0) && SelectedObject != null && DragObject == null)
    //        {
    //            DragObject = SelectedObject;
    //            DragObject.GetComponent<Collider>().enabled = false;

    //            startPos = DragObject.transform.position; // save position in case draged to invalid place
    //            movePlane = new Plane(-Camera.main.transform.forward, DragObject.transform.position);
    //            IsFinishDrag = false;

    //        }

    //        //Put down Hero
    //        if (Input.GetMouseButtonUp(0) && DragObject != null)
    //        {
    //            isDrag = !isDrag;
    //            IsFinishDrag = false;
    //            if (ParentObject != null)
    //            {

    //                if (!isDrag)
    //                {
    //                    DragObject.transform.parent = ParentObject.transform;
    //                    DragObject.GetComponent<Collider>().enabled = true;
    //                    DragObject.transform.localPosition = Vector3.zero;

    //                    //DragObject = null;
    //                    ParentObject = null;
    //                    IsFinishDrag = true;
    //                }
    //            }
    //            else
    //            {
    //                isDrag = true;
    //            }

    //        }

    //        //Dragging Hero Place
    //        else if (isDrag && DragObject != null)
    //        {
    //            if (DragObject != null && DragObject.transform.childCount == 0)
    //            {
    //                DragObject.transform.parent = ParentObject.transform;
    //            }

    //            camRay = GameManager.MainCamera.ScreenPointToRay(Input.mousePosition); // shoot a ray at the obj from mouse screen point

    //            // finde the collision on movePlane
    //            if (movePlane.Raycast(camRay, out hitDist))
    //            {
    //                point = camRay.GetPoint(hitDist);                                       // define the point on movePlane
    //                t = -(fixedDistance - camRay.origin.y) / (camRay.origin.y - point.y);   // the x,y or z plane you want to be fixed to

    //                #region calculate the new point t futher along the ray
    //                corPoint.x = camRay.origin.x + (point.x - camRay.origin.x) * t;
    //                corPoint.y = camRay.origin.y + (point.y - camRay.origin.y) * t;
    //                corPoint.z = camRay.origin.z + (point.z - camRay.origin.z) * t;
    //                #endregion

    //                //DragObject.HeroPlace = ParentObject;
    //                DragObject.transform.position = corPoint;
    //            }
    //        }
    //    }
    //}
    public class SelectManager : MonoBehaviour
    {
        GameManager GameManager;

        //[HideInInspector]
        public GameObject ParentObject;
        public ISelectable SelectedObject, DragObject;
        private bool isDrag;
        //public bool IsFinishDrag;

        [SerializeField]
        readonly float fixedDistance = 0f;
        Plane movePlane;
        float hitDist, t;
        Ray camRay;
        Vector3 startPos, point, corPoint;

        public static SelectManager Instance;

        void Awake()
        {
            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            GameManager = GameManager.Instance;
        }

        // Update is called once per frame
        void Update()
        {
            //Take-up Hero
            if (Input.GetMouseButtonDown(0) && SelectedObject != null && DragObject == null)
            {
                DragObject = SelectedObject;
                DragObject.DragUp();

                startPos = DragObject.transform.position; // save position in case draged to invalid place
                movePlane = new Plane(-Camera.main.transform.forward, DragObject.transform.position);
                //IsFinishDrag = false;

            }

            //Put down Hero
            if (Input.GetMouseButtonUp(0) && DragObject != null)
            {
                isDrag = !isDrag;
                //IsFinishDrag = false;
                if (ParentObject != null)
                {
                    if (!isDrag)
                    {
                        DragObject.PutDown();

                        DragObject = null;
                        ParentObject = null;
                        //IsFinishDrag = true;
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
                //if (DragObject != null && DragObject.transform.childCount == 0)
                //{
                //    DragObject.transform.parent = ParentObject.transform;
                //}

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
