using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public enum BuffType
{
    Class,
    Race
}
[System.Serializable]
public class BuffStatus
{
    public BuffType BuffType;
    public HeroClass HeroClass;
    public HeroRace HeroRace;
    public BuffLevel BuffLevel;

    public BuffStatus(HeroClass heroClass)
    {
        BuffType = BuffType.Class;
        HeroClass = heroClass;
    }
    public BuffStatus(HeroRace heroRace)
    {
        BuffType = BuffType.Race;
        HeroRace = heroRace;
    }
}
[System.Serializable]
public class Buffers
{
    public BuffType BuffType;
    //public string[] buffType = { "Class", "Race" };
    public int buffTypeIndex;
    public bool isShow = true;
    public HeroClass heroClass;
    public HeroRace heroRace;
    public int totalNumber;
    public int bronzeNumber;
    public int sliverNumber;
    public int goldenNumber;
}
public delegate void BuffHandler(Hero hero, BuffLevel nextState);

public delegate void BuffEventHandler(object sender, BuffEventArgs e);
public delegate void BuffListenHandler(object sender, BuffListenEventArgs e);

public struct BuffEventArgs
{
    public BuffHandler BuffHandler;
    public BuffStatus BuffStatus;
}
public struct BuffListenEventArgs
{
    public BuffType BuffType;
    public HeroClass HeroClass;
    public HeroRace HeroRace;
    public int BuffCount;
}
public class BuffersManager : MonoBehaviour
{
    [SerializeField]
    public List<Buffers> buffers = new List<Buffers>();

    public static BuffersManager Instance;

    public BuffEventHandler buffChange;
    public BuffEventArgs argsBuff;

    private void Awake()
    {
        Instance = this;
    }
    
    public void OnBuffChange(BuffListenEventArgs args)
    {
        Buffers targetBuff = new Buffers();
        BuffLevel buffLevel = BuffLevel.Newbie;
        foreach (Buffers buffer in buffers)
        {
            if(buffer.heroClass ==args.HeroClass||buffer.heroRace == args.HeroRace)
            {
                targetBuff = buffer;
                break;
            }
        }

        if(args.BuffCount >= targetBuff.goldenNumber)
        {
            buffLevel = BuffLevel.Golden;
        }
        else if(args.BuffCount >= targetBuff.sliverNumber)
        {
            buffLevel = BuffLevel.Sliver;
        }
        else if(args.BuffCount >= targetBuff.bronzeNumber)
        {
            buffLevel = BuffLevel.Bronze;
        }

        if (args.BuffType == BuffType.Class)
        {
            argsBuff.BuffStatus = new BuffStatus(args.HeroClass);
            switch (args.HeroClass)
            {
                case HeroClass.Assassin:
                    argsBuff.BuffHandler = AssassinBuff;
                    break;
                case HeroClass.Brawler:
                    argsBuff.BuffHandler = BrawlerBuff;
                    break;
                case HeroClass.Ranger:
                    argsBuff.BuffHandler = RangerBuff;
                    break;
                case HeroClass.Supporter:
                    argsBuff.BuffHandler = SupporterBuff;
                    break;
                case HeroClass.Warrior:
                    argsBuff.BuffHandler = WarriorBuff;
                    break;
                case HeroClass.Wizard:
                    argsBuff.BuffHandler = WizardBuff;
                    break;
            }
        }
        else
        {
            argsBuff.BuffStatus = new BuffStatus(args.HeroRace);

            switch (args.HeroRace)
            {
                case HeroRace.Demon:
                    argsBuff.BuffHandler = DemonBuff;
                    break;
                case HeroRace.Divinity:
                    argsBuff.BuffHandler = DivinityBuff;
                    break;
                case HeroRace.Dwarf:
                    argsBuff.BuffHandler = DwarfBuff;
                    break;
                case HeroRace.Human:
                    argsBuff.BuffHandler = HumanBuff;
                    break;
                case HeroRace.Rebel:
                    argsBuff.BuffHandler = RebelBuff;
                    break;
                case HeroRace.Spirit:
                    argsBuff.BuffHandler = SpiritBuff;
                    break;
                case HeroRace.Wild:
                    argsBuff.BuffHandler = WildBuff;
                    break;
            }
        }
        argsBuff.BuffStatus.BuffLevel = buffLevel;
        buffChange?.Invoke(this, argsBuff);
    }

    public void DemonBuff(Hero hero, BuffLevel nextLevel)
    {
        BuffLevel currBuffLevel = BuffLevel.Newbie;
        foreach (BuffStatus buffStatus in hero.BuffStatuses)
        {
            if (buffStatus.HeroRace == HeroRace.Demon)
            {
                currBuffLevel = buffStatus.BuffLevel;
                break;
            }
        }
        if (currBuffLevel != nextLevel)
        {
            if (currBuffLevel == BuffLevel.Bronze)
            {
                hero.ManaRecoveryRate -= 0.2f;
            }
            else if (currBuffLevel == BuffLevel.Sliver)
            {
                hero.ManaRecoveryRate -= 0.4f;
            }
            else if (currBuffLevel == BuffLevel.Golden)
            {
                hero.ManaRecoveryRate -= 0.6f;
            }

            if (nextLevel == BuffLevel.Bronze)
            {
                hero.ManaRecoveryRate += 0.2f;
            }
            else if (nextLevel == BuffLevel.Sliver)
            {
                hero.ManaRecoveryRate += 0.4f;
            }
            else if (nextLevel == BuffLevel.Golden)
            {
                hero.ManaRecoveryRate += 0.6f;
            }
            hero.photonView.RPC("RPC_SyncAttribute", PhotonTargets.Others,
                HeroAttribute.Mana_Recovery_Rate, hero.ManaRecoveryRate);
        }
    }

    public void DivinityBuff(Hero hero, BuffLevel nextLevel)
    {
        BuffLevel currBuffLevel = BuffLevel.Newbie;
        foreach (BuffStatus buffStatus in hero.BuffStatuses)
        {
            if (buffStatus.HeroRace == HeroRace.Divinity)
            {
                currBuffLevel = buffStatus.BuffLevel;
                break;
            }
        }
        if (currBuffLevel != nextLevel)
        {
            if (currBuffLevel == BuffLevel.Bronze)
            {
                hero.ShieldInit -= 125;
            }
            else if (currBuffLevel == BuffLevel.Sliver)
            {
                hero.ShieldInit -= 250;
            }
            else if (currBuffLevel == BuffLevel.Golden)
            {
                hero.ShieldInit -= 500;
            }

            if (nextLevel == BuffLevel.Bronze)
            {
                hero.ShieldInit += 125;
            }
            else if (nextLevel == BuffLevel.Sliver)
            {
                hero.ShieldInit += 250;
            }
            else if (nextLevel == BuffLevel.Golden)
            {
                hero.ShieldInit += 500;
            }
            hero.photonView.RPC("RPC_SyncAttribute", PhotonTargets.Others,
                HeroAttribute.Shield, hero.ShieldInit);
        }
    }

    public void DwarfBuff(Hero hero, BuffLevel nextLevel) { }

    public void HumanBuff(Hero hero, BuffLevel nextLevel)
    {
        BuffLevel currBuffLevel = BuffLevel.Newbie;
        foreach (BuffStatus buffStatus in hero.BuffStatuses)
        {
            if (buffStatus.HeroRace == HeroRace.Human)
            {
                currBuffLevel = buffStatus.BuffLevel;
                break;
            }
        }
        if (currBuffLevel != nextLevel)
        {
            if (currBuffLevel == BuffLevel.Bronze)
            {
                hero.Health -= 150;
                hero.MagicDefense -= 15;
            }
            else if (currBuffLevel == BuffLevel.Sliver)
            {
                hero.Health -= 300;
                hero.MagicDefense -= 30;
            }
            else if (currBuffLevel == BuffLevel.Golden)
            {
                hero.Health -= 600;
                hero.MagicDefense -= 60;
            }

            if (nextLevel == BuffLevel.Bronze)
            {
                hero.Health += 150;
                hero.MagicDefense += 15;
            }
            else if (nextLevel == BuffLevel.Sliver)
            {
                hero.Health += 300;
                hero.MagicDefense += 30;
            }
            else if (nextLevel == BuffLevel.Golden)
            {
                hero.Health += 600;
                hero.MagicDefense += 60;
            }
            hero.photonView.RPC("RPC_SyncAttribute", PhotonTargets.Others,
                HeroAttribute.Health, hero.Health);
            hero.photonView.RPC("RPC_SyncAttribute", PhotonTargets.Others,
                HeroAttribute.Magic_Defense, hero.MagicDefense);
        }
    }

    public void RebelBuff(Hero hero, BuffLevel nextLevel) { }

    public void SpiritBuff(Hero hero, BuffLevel nextLevel) { }

    public void WildBuff(Hero hero, BuffLevel nextLevel)
    {
        BuffLevel currBuffLevel = BuffLevel.Newbie;
        foreach (BuffStatus buffStatus in hero.BuffStatuses)
        {
            if (buffStatus.HeroRace == HeroRace.Wild)
            {
                currBuffLevel = buffStatus.BuffLevel;
                break;
            }
        }
        if (currBuffLevel != nextLevel)
        {
            if (currBuffLevel == BuffLevel.Bronze)
            {
                hero.Health -= 100;
                hero.HealthRecoveryRate -= 0.01f;
            }
            else if (currBuffLevel == BuffLevel.Sliver)
            {
                hero.Health -= 200;
                hero.HealthRecoveryRate -= 0.02f;
            }
            else if (currBuffLevel == BuffLevel.Golden)
            {
                hero.Health -= 400;
                hero.HealthRecoveryRate -= 0.03f;
            }

            if (nextLevel == BuffLevel.Bronze)
            {
                hero.Health += 100;
                hero.HealthRecoveryRate += 0.01f;
            }
            else if (nextLevel == BuffLevel.Sliver)
            {
                hero.Health += 200;
                hero.HealthRecoveryRate += 0.02f;
            }
            else if (nextLevel == BuffLevel.Golden)
            {
                hero.Health += 400;
                hero.HealthRecoveryRate += 0.03f;
            }
            hero.photonView.RPC("RPC_SyncAttribute", PhotonTargets.Others,
                HeroAttribute.Health, hero.Health);
            hero.photonView.RPC("RPC_SyncAttribute", PhotonTargets.Others,
                HeroAttribute.Heal_Recovery_Rate, hero.HealthRecoveryRate);
        }
    }

    public void AssassinBuff(Hero hero, BuffLevel nextLevel)
    {
        BuffLevel currBuffLevel = BuffLevel.Newbie;
        foreach(BuffStatus buffStatus in hero.BuffStatuses)
        {
            if (buffStatus.HeroClass == HeroClass.Assassin)
            {
                currBuffLevel = buffStatus.BuffLevel;
                break;
            }
        }
        if (currBuffLevel != nextLevel)
        {
            if(currBuffLevel == BuffLevel.Bronze)
            {
                hero.CriticalChance -= 20;
                hero.CriticalDamage -= 1.25f;
            }
            else if (currBuffLevel == BuffLevel.Sliver)
            {
                hero.CriticalChance -= 40;
                hero.CriticalDamage -= 1.5f;
            }
            else if (currBuffLevel == BuffLevel.Golden)
            {
                hero.CriticalChance -= 60;
                hero.CriticalDamage -= 2f;
            }

            if (nextLevel == BuffLevel.Bronze)
            {
                hero.CriticalChance += 20;
                hero.CriticalDamage += 1.25f;
            }
            else if (nextLevel == BuffLevel.Sliver)
            {
                hero.CriticalChance += 40;
                hero.CriticalDamage += 1.5f;
            }
            else if (nextLevel == BuffLevel.Golden)
            {
                hero.CriticalChance += 60;
                hero.CriticalDamage += 2f;
            }
            hero.photonView.RPC("RPC_SyncAttribute", PhotonTargets.Others,
                HeroAttribute.Critical_Cahnce, hero.CriticalChance);
            hero.photonView.RPC("RPC_SyncAttribute", PhotonTargets.Others,
                HeroAttribute.Critical_Damage, hero.CriticalDamage);
        }
    }

    public void BrawlerBuff(Hero hero, BuffLevel nextLevel) { }

    public void RangerBuff(Hero hero, BuffLevel nextLevel)
    {
        BuffLevel currBuffLevel = BuffLevel.Newbie;
        foreach (BuffStatus buffStatus in hero.BuffStatuses)
        {
            if (buffStatus.HeroClass == HeroClass.Ranger)
            {
                currBuffLevel = buffStatus.BuffLevel;
                break;
            }
        }
        if (currBuffLevel != nextLevel)
        {
            if (currBuffLevel == BuffLevel.Bronze)
            {
                hero.CriticalChance -= 10;
                hero.AttackSpeed -= 0.2f/2;
            }
            else if (currBuffLevel == BuffLevel.Sliver)
            {
                hero.CriticalChance -= 20;
                hero.AttackSpeed -= 0.2f/2;
            }
            else if (currBuffLevel == BuffLevel.Golden)
            {
                hero.CriticalChance -= 50;
                hero.AttackSpeed -= 0.6f/2;
            }

            if (nextLevel == BuffLevel.Bronze)
            {
                hero.CriticalChance += 10;
                hero.AttackSpeed += 0.2f/2;
            }
            else if (nextLevel == BuffLevel.Sliver)
            {
                hero.CriticalChance += 20;
                hero.AttackSpeed += 0.4f/2;
            }
            else if (nextLevel == BuffLevel.Golden)
            {
                hero.CriticalChance += 50;
                hero.AttackSpeed += 0.6f/2;
            }
            hero.photonView.RPC("RPC_SyncAttribute", PhotonTargets.Others,
                HeroAttribute.Critical_Cahnce, hero.CriticalChance);
            hero.photonView.RPC("RPC_SyncAttribute", PhotonTargets.Others,
                HeroAttribute.Attack_Speed, hero.AttackSpeed);
        }
    }

    public void SupporterBuff(Hero hero, BuffLevel nextLevel) {
        BuffLevel currBuffLevel = BuffLevel.Newbie;
        foreach (BuffStatus buffStatus in hero.BuffStatuses)
        {
            if (buffStatus.HeroClass == HeroClass.Supporter)
            {
                currBuffLevel = buffStatus.BuffLevel;
                break;
            }
        }
        if (currBuffLevel != nextLevel)
        {
            if (currBuffLevel == BuffLevel.Bronze)
            {
                hero.PhysicalDefense -= 20;
                hero.MagicDefense -= 25;
            }
            else if (currBuffLevel == BuffLevel.Sliver)
            {
                hero.PhysicalDefense -= 40;
                hero.MagicDefense -= 50;
            }
            else if (currBuffLevel == BuffLevel.Golden)
            {
                hero.PhysicalDefense -= 80;
                hero.MagicDefense -= 100;
            }

            if (nextLevel == BuffLevel.Bronze)
            {
                hero.PhysicalDefense += 20;
                hero.MagicDefense += 25;
            }
            else if (nextLevel == BuffLevel.Sliver)
            {
                hero.PhysicalDefense += 40;
                hero.MagicDefense += 50;
            }
            else if (nextLevel == BuffLevel.Golden)
            {
                hero.PhysicalDefense += 80;
                hero.MagicDefense += 100;
            }
            hero.photonView.RPC("RPC_SyncAttribute", PhotonTargets.Others,
                HeroAttribute.Physic_Defense, hero.PhysicalDefense);
            hero.photonView.RPC("RPC_SyncAttribute", PhotonTargets.Others,
                HeroAttribute.Magic_Defense, hero.MagicDefense);
        }
    }

    public void WarriorBuff(Hero hero, BuffLevel nextLevel) {
        BuffLevel currBuffLevel = BuffLevel.Newbie;
        foreach (BuffStatus buffStatus in hero.BuffStatuses)
        {
            if (buffStatus.HeroClass == HeroClass.Warrior)
            {
                currBuffLevel = buffStatus.BuffLevel;
                break;
            }
        }
        if (currBuffLevel != nextLevel)
        {
            if (currBuffLevel == BuffLevel.Bronze)
            {
                hero.AttackDamage -= 20;
                hero.CriticalDamage -= 1.25f;
            }
            else if (currBuffLevel == BuffLevel.Sliver)
            {
                hero.AttackDamage -= 40;
                hero.CriticalDamage -= 1.25f;
            }
            else if (currBuffLevel == BuffLevel.Golden)
            {
                hero.AttackDamage -= 60;
                hero.CriticalDamage -= 1.25f;
            }

            if (nextLevel == BuffLevel.Bronze)
            {
                hero.AttackDamage += 20;
                hero.CriticalDamage += 1.25f;
            }
            else if (nextLevel == BuffLevel.Sliver)
            {
                hero.AttackDamage += 40;
                hero.CriticalDamage += 1.25f;
            }
            else if (nextLevel == BuffLevel.Golden)
            {
                hero.AttackDamage += 60;
                hero.CriticalDamage += 1.25f;
            }
            hero.photonView.RPC("RPC_SyncAttribute", PhotonTargets.Others,
                HeroAttribute.Attack, hero.AttackDamage);
            hero.photonView.RPC("RPC_SyncAttribute", PhotonTargets.Others,
                HeroAttribute.Critical_Damage, hero.CriticalDamage);
        }
    }

    public void WizardBuff(Hero hero, BuffLevel nextLevel)
    {
        BuffLevel currBuffLevel = BuffLevel.Newbie;
        foreach (BuffStatus buffStatus in hero.BuffStatuses)
        {
            if (buffStatus.HeroClass == HeroClass.Wizard)
            {
                currBuffLevel = buffStatus.BuffLevel;
                break;
            }
        }
        if (currBuffLevel != nextLevel)
        {
            if (currBuffLevel == BuffLevel.Bronze)
            {
                hero.SkillPower -= 30;
            }
            else if (currBuffLevel == BuffLevel.Sliver)
            {
                hero.AttackDamage -= 60;
            }
            else if (currBuffLevel == BuffLevel.Golden)
            {
                hero.AttackDamage -= 120;
            }

            if (nextLevel == BuffLevel.Bronze)
            {
                hero.SkillPower += 30;
            }
            else if (nextLevel == BuffLevel.Sliver)
            {
                hero.AttackDamage += 60;
            }
            else if (nextLevel == BuffLevel.Golden)
            {
                hero.AttackDamage += 120;
            }
            hero.photonView.RPC("RPC_SyncAttribute", PhotonTargets.Others,
                HeroAttribute.Skill_Damage, hero.SkillPower);
        }
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
            buffer.isShow = EditorGUILayout.Foldout(buffer.isShow, buffer.BuffType.ToString());

            if (buffer.isShow)
            {
                string[] buffType = { "Class", "Race" };
                buffer.buffTypeIndex = EditorGUILayout.Popup("Type", buffer.buffTypeIndex, buffType);
                buffer.BuffType = (BuffType)buffer.buffTypeIndex;

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