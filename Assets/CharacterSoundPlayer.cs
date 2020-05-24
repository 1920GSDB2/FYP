using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFT;

public class CharacterSoundPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip attackSound,skillSound;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void playAttackSound() {
        if (attackSound != null)
        {
            audioSource.clip = attackSound;
            Debug.Log("Network" + NetworkManager.Instance.playerId);
            Debug.Log("play sound "+ SoundSettingManager.Instance.GameSound);
            audioSource.PlayOneShot(attackSound,SoundSettingManager.Instance.GameSound);
           
        }
    }
    public void playSkillSound()
    {
        if (skillSound != null)
        {
            audioSource.clip = skillSound;
            audioSource.PlayOneShot(skillSound, SoundSettingManager.Instance.GameSound);
        }
    }

}
