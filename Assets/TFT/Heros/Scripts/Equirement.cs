using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Equirement : MonoBehaviour
{
    public Button Icon;
    public HeroAttribute[] HeroAttributes;
    public int[] AttributeValues;
    //[HideInInspector]
    public bool isComponent;

    public bool isDrag;

    // Start is called before the first frame update
    public virtual void Start()
    {
        Icon.onClick.AddListener(delegate { DraggingObject(); });        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (isDrag)
        {
            Debug.Log("Drag Object");
            transform.position = Input.mousePosition;
        }
    }
    public void DraggingObject()
    {
        Debug.Log("DraggingObject");
        isDrag = !isDrag;
        
    }
    
    
}
