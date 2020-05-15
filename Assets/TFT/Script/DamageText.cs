using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public Animator animator;
    public TextMeshProUGUI damageText;
    // Start is called before the first frame update
    void Start()
    {
        
        AnimatorClipInfo[] clipInfos = animator.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clipInfos[0].clip.length);
  
    }
    public void setText(string text) {
        damageText.text = text;
    }

}
