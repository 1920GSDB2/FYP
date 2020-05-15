
using UnityEngine;

public class IndomitableFighter : Hero
{
    float tempAttack,tempAttackSpeed;
   
    public override void syncAdjustHp(float damage)
    {
        base.syncAdjustHp(damage);
        float basicAttack= AttackDamage -tempAttack;
        float basicAttackSpeed = AttackSpeed - tempAttackSpeed;
        float lostPrecentage =Mathf.Floor((1-Health / MaxHealth)*100);
        tempAttack = lostPrecentage;
        tempAttackSpeed = lostPrecentage * 0.02f;
     //   Debug.Log("tempSpeed "+tempAttackSpeed+ "attackSpeed " + basicAttackSpeed + tempAttackSpeed + " damage " + basicAttack + tempAttack);
      //  AttackDamage = basicAttack+ tempAttack;
     //   AttackSpeed = basicAttactSpeed+ tempAttackSpeed;
        photonView.RPC("RPC_SyncHeroAttribute", PhotonTargets.All,(byte)HeroAttribute.Attack, basicAttack + tempAttack);
        photonView.RPC("RPC_SyncHeroAttribute", PhotonTargets.All, (byte)HeroAttribute.Attack_Speed, basicAttackSpeed + tempAttackSpeed);
    }
}
