using UnityEngine;
using System;
using TFT;


public class MonsterWaveManager : MonoBehaviour
{
   
    public MonsterWave[] Wave;
    int currentIndex;
    public static MonsterWaveManager Instance;
    int monsterCount,dieCount;
    bool isDropEquipment;
    int dropRate;
    Award[] awardType= { Award.Equipment };

    
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }
    public void spawnCurrentWaveAllMonster() {
        dropRate = currentIndex * 10;
        if (currentIndex == Wave.Length)
            return;
        monsterCount = getCurrentTotalMonsterCount();
        for (int i = 0; i < monsterCount; i++) {
            
           
            // Debug.Log(monster.name+" "+ NetworkManager.Instance.posId+" "+ getMonsterPosition(i));
            NetworkManager.Instance.spawnMonster(getMonsterName(i), getMonsterPosition(i));
             
          //  monster.GetComponent<PhotonView>().RPC("RPC_ShowHpBar", PhotonTargets.All, NetworkManager.Instance.posId);
          
        }

        NetworkManager.Instance.BattleWithMonsters();
      //  currentIndex++;
    }
    public void monsterDie() {
        dieCount++;       
        int a= UnityEngine.Random.Range(1, 101);
        if (a < dropRate)
            randomAward();

        if (dieCount == monsterCount && !isDropEquipment)
            award(Award.Equipment);

    }
    void randomAward() {
        int index = UnityEngine.Random.Range(0, awardType.Length);
        award(awardType[index]);

    }
    void award(Award type) {
        if (type == Award.Equipment) {
            isDropEquipment = true;
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


