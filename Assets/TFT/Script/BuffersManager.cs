using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[System.Serializable]
public class Buffers
{
    public string[] buffType = { "Class", "Race" };
    public int buffTypeIndex;
    public bool isShow = true;
    public HeroClass heroClass;
    public HeroRace heroRace;
    public int totalNumber;
    public int bronzeNumber;
    public int sliverNumber;
    public int goldenNumber;
}

public class BuffersManager : MonoBehaviour
{
    [SerializeField]
    public List<Buffers> buffers = new List<Buffers>();

    public static BuffersManager Instance;

    private void Awake()
    {
        Instance = this;
    }
}

#if UNITY_EDITOR 
[CustomEditor (typeof(BuffersManager))]
public class BuffersEditor : Editor
{

    BuffersManager target;

    void OnEnable()
    {
        target = (BuffersManager)base.target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Buffers Manager", EditorStyles.wordWrappedLabel);

        List<Buffers> bufferItems = target.buffers;

        int buffItemsSize = bufferItems.Count;

        
        buffItemsSize = EditorGUILayout.IntField("Number of Buffers: ", buffItemsSize);
        while (buffItemsSize > bufferItems.Count)
        {
            bufferItems.Add(new Buffers());
        }
        while (buffItemsSize < bufferItems.Count && buffItemsSize >= 0)
        {
            bufferItems.RemoveAt(bufferItems.Count - 1);
        }


        foreach (Buffers buffer in bufferItems)
        {
            //EditorGUILayout.Foldout(true,"Buff");
            buffer.isShow = EditorGUILayout.Foldout(buffer.isShow, buffer.buffType[buffer.buffTypeIndex]);

            if (buffer.isShow)
            {
                buffer.buffTypeIndex = EditorGUILayout.Popup("Type", buffer.buffTypeIndex, buffer.buffType);

                if (buffer.buffTypeIndex == 0)
                {
                    buffer.heroClass = (HeroClass)EditorGUILayout.EnumPopup("HeroClass", buffer.heroClass);
                    buffer.heroRace = HeroRace.None;
                }
                else if (buffer.buffTypeIndex == 1)
                {
                    buffer.heroRace = (HeroRace)EditorGUILayout.EnumPopup("HeroRare", buffer.heroRace);
                    buffer.heroClass = HeroClass.None;
                }
                buffer.totalNumber = EditorGUILayout.IntSlider("TotalNumber", buffer.totalNumber, 1, 6);
                buffer.goldenNumber = EditorGUILayout.IntSlider("Gold", buffer.goldenNumber, 1, buffer.totalNumber);
                buffer.sliverNumber = EditorGUILayout.IntSlider("Sliver", buffer.sliverNumber, 1, buffer.goldenNumber);
                buffer.bronzeNumber = EditorGUILayout.IntSlider("Bronze", buffer.bronzeNumber, 1, buffer.sliverNumber);
            }
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkSceneDirty(target.gameObject.scene);
        }
    }
}
#endif