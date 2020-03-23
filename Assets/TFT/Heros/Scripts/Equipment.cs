using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Equipment : MonoBehaviour
{
    public Image Icon;
    public HeroAttribute[] HeroAttributes;
    public int[] AttributeValues;
    //[HideInInspector]
    public bool isComponent;

    public bool isDrag;

    Plane movePlane;
    float fixedDistance = 0f;
    float hitDist, t;
    Ray camRay;
    Vector3 startPos, point, corPoint;

    public virtual void Start()
    {

    }
    // Update is called once per frame
    public virtual void Update()
    {
        if (isDrag)
        {
            Debug.Log("Drag Object");
            camRay = TFT.GameManager.Instance.MainCamera.ScreenPointToRay(Input.mousePosition); // shoot a ray at the obj from mouse screen point

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

            }
        }
    }
    public void DraggingObject()
    {
        Debug.Log("DraggingObject");
        isDrag = !isDrag;
    }

    private void OnMouseEnter()
    {
        isDrag = true;
        movePlane = new Plane(-Camera.main.transform.forward, 1);
    }

    private void OnMouseExit()
    {
        isDrag = false;
    }

    private void OnMouseDrag()
    {
        transform.position = Input.mousePosition;
    }
    
}
