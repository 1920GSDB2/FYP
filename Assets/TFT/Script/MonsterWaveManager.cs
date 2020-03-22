using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TFT;
using System.IO;

public class MonsterWaveManager : MonoBehaviour
{
   
    public MonsterWave[] Wave;
    int currentIndex;
    public static MonsterWaveManager Instance;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }
    public void spawnCurrentWaveAllMonster() {
        if (currentIndex == Wave.Length)
            return;
        for (int i = 0; i < getCurrentTotalMonsterCount(); i++) {
            
           
            // Debug.Log(monster.name+" "+ NetworkManager.Instance.posId+" "+ getMonsterPosition(i));
            NetworkManager.Instance.spawnMonster(getMonsterName(i), getMonsterPosition(i));
             
          //  monster.GetComponent<PhotonView>().RPC("RPC_ShowHpBar", PhotonTargets.All, NetworkManager.Instance.posId);

           
        }
       // NetworkManager.Instance.StartCoroutine(BattleWithMonster);
        currentIndex++;
    }
    
    public int getMonsterPosition(int number) {
        return Wave[currentIndex].monster[number].position;
    }
    public string getMonsterName(int number)
    {
        return Wave[currentIndex].monster[number].name;
    }
    public int getCurrentTotalMonsterCount() {
        return Wave[currentIndex].monster.Length;
    }

}
[Serializable]
public class MonsterWave{
    public int waveNumber;
    public MonsterData[] monster;
    
}
[Serializable]
public class MonsterData
{
    public string name;
    public int position;

}


