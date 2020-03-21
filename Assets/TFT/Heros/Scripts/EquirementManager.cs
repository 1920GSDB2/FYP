using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquirementManager : MonoBehaviour
{
    public List<Equirement> Equirements = new List<Equirement>(3);
    public Transform ItemList;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool AddEquirement(Equirement _addEquirement)
    {
        if (Equirements.Count == 3 && !Equirements[2].isComponent) return false;
        else if (!Equirements[Equirements.Count - 1].isComponent)
        {
            Equirements.Add(_addEquirement);
            return true;
        }
        else
        {
            //Equirement Composite
            Composite(Equirements[Equirements.Count - 1].EquirementType, _addEquirement.EquirementType);
            return true;
        }
    }

    private void Composite(EquirementType _lastEquirementType, EquirementType _addEquirement)
    {

    }
}
