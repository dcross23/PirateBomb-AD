using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuelaCheckGround : MonoBehaviour
{
    protected Animator anim;

    void Start()
    {
        anim = GetComponentInParent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        anim.SetBool("estaEnSuelo", true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        anim.SetBool("estaEnSuelo", false);
    }

}
