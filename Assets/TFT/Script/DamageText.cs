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
  
    public void setText(string text,DamageType type) {
        damageText.text = text;
        switch (type) {
            case DamageType.Magic:
                damageText.faceColor = Color.blue;
                break;
            case DamageType.Physical:
                damageText.faceColor = Color.red;
                break;
            case DamageType.TrueDamage:
                damageText.faceColor = Color.white;
                break;
            case DamageType.Heal:
                damageText.faceColor = Color.green;
                break;
        }
       
    }

}
