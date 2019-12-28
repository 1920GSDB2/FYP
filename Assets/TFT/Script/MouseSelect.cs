using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSelect : MonoBehaviour
{
    public Hero SelectedHero, DragHero;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && SelectedHero != null)
        {
            DragHero = SelectedHero;
        }
        else if (Input.GetMouseButton(0) && DragHero != null)
        {
            Vector3 pos = Input.mousePosition;
            pos.z = transform.position.z - Camera.main.transform.position.z;
            Debug.Log(pos);
            DragHero.transform.position = Camera.main.ScreenToWorldPoint(pos);

        }
        else if (Input.GetMouseButtonUp(0) && DragHero!=null)
        {
            DragHero.transform.localPosition = Vector3.zero;
            DragHero = null;
        }
    }
    
}
