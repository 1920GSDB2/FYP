using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Buffers
{
    public string[] buffType = { "Class", "Race" };
    public int buffTypeIndex;
    public bool isShow = true;
    public HeroClass heroClass;
    public HeroRace heroRare;
    public int totalNumber;
    public int bronzeNumber;
    public int sliverNumber;
    public int goldenNumber;
}

public class BuffersManager : MonoBehaviour
{
    [SerializeField]
    public List<Buffers> buffersClass = new List<Buffers>();
}

#if UNITY_EDITOR 
[CustomEditor (typeof(BuffersManager))]
public class BuffersEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Buffers Manager");
        EditorGUILayout.Space();

        List<Buffers> buffersClass = (target as BuffersManager).buffersClass;
        
        foreach (Buffers buffer in buffersClass)
        {
            //EditorGUILayout.Foldout(true,"Buff");
            buffer.isShow = EditorGUILayout.Foldout(buffer.isShow, buffer.buffType[buffer.buffTypeIndex]);

            if (buffer.isShow)
            {
                buffer.buffTypeIndex = EditorGUILayout.Popup("Type", buffer.buffTypeIndex, buffer.buffType);

                if (buffer.buffTypeIndex == 0)
                {
                    buffer.heroClass = (HeroClass)EditorGUILayout.EnumPopup("HeroClass", buffer.heroClass);
                    buffer.heroRare = HeroRace.None;
                }
                else if (buffer.buffTypeIndex == 1)
                {
                    buffer.heroRare = (HeroRace)EditorGUILayout.EnumPopup("HeroRare", buffer.heroRare);
                    buffer.heroClass = HeroClass.None;
                }
                buffer.totalNumber = EditorGUILayout.IntSlider("TotalNumber", buffer.totalNumber, 1, 6);
                buffer.goldenNumber = EditorGUILayout.IntSlider("TotalNumber", buffer.goldenNumber, 1, buffer.totalNumber);
                buffer.sliverNumber = EditorGUILayout.IntSlider("TotalNumber", buffer.sliverNumber, 1, buffer.goldenNumber);
                buffer.bronzeNumber = EditorGUILayout.IntSlider("TotalNumber", buffer.bronzeNumber, 1, buffer.sliverNumber);
            }
        }
        if (GUILayout.Button("+"))
        {
            buffersClass.Add(new Buffers());
        }
        if (buffersClass.Count > 0)
        {
            if (GUILayout.Button("-"))
            {
                buffersClass.Remove(buffersClass[buffersClass.Count - 1]);
            }
        }
    }
}
#endif