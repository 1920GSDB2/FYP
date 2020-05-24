using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public Collection collectionPrefab;
    public Transform collectionBag;
    public void Start()
    {
        
    }
    public void createCollection(string name,Sprite icon) {
        Collection collection = Instantiate(collectionPrefab);


    }

}
