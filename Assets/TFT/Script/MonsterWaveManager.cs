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
    // Start is called before the first frame update

    public void spawnCurrentWaveAllMonster() {
        for (int i = 0; i < getCurrentTotalMonsterCount(); i++) {
            Monster monster = (PhotonNetwork.Instantiate(Path.Combine("Prefabs", getMonsterName(i)), Vector3.zero, Quaternion.identity, 0)).GetComponent<Monster>();
        }
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


