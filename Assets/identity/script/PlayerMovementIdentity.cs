using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementIdentity : MonoBehaviour
{
    private PhotonView photonView;
    private Animator anim;
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        if(photonView.isMine)
        CheckInput();
    }
    private void CheckInput() {
        float moveSpeed = 20f;
        float rotateSpeed = 500f;
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        transform.position += transform.forward * (vertical * moveSpeed * Time.deltaTime);
        transform.Rotate(new Vector3(0, horizontal * rotateSpeed * Time.deltaTime, 0));
        anim.SetFloat("Speed", vertical);
    }
}
