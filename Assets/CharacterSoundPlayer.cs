using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            audioSource.Play();
            Debug.Log("play sound ");
        }
    }
    public void playSkillSound()
    {
        if (skillSound != null)
        {
            audioSource.clip = skillSound;
            audioSource.Play();
        }
    }

}
