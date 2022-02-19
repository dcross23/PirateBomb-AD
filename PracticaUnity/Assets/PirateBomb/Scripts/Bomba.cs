using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomba : MonoBehaviour
{
    protected float radioExplosion = 3;
    protected AudioSource sonidoCargaBomba;
    protected AudioSource sonidoExplosionBomba;

    void Start()
    {
        sonidoCargaBomba = GameObject.Find("SonidoCargarBomba").GetComponent<AudioSource>();
        sonidoExplosionBomba = GameObject.Find("SonidoExplosion").GetComponent<AudioSource>();
    }

    public void Explotar()
    {
        Collider2D[] collider2D = Physics2D.OverlapCircleAll(transform.position, radioExplosion);

        Jugador jugador = GameObject.FindGameObjectWithTag("Jugador").GetComponent<Jugador>();

        for (int i = 0; i < collider2D.Length; i++)
            if (collider2D[i].gameObject.CompareTag("Enemigo"))
            {
                EnemigoBasico enemigoPirata = collider2D[i].gameObject.GetComponent<EnemigoBasico>();
                enemigoPirata.explosionBomba();
            }
            else if(collider2D[i].gameObject.CompareTag("Jugador"))
            {
                jugador.RecibirHit(transform.position.x > jugador.transform.position.x);
            }

        Destroy(this.gameObject);
        jugador.RecuperarBomba();
    }

    public void QuitarBarra()
    {
        Destroy(this.transform.GetChild(0).gameObject);
    }

    public void SonidoAlCargar()
    {
        sonidoCargaBomba.Play();
    }
    public void SonidoAlExplotar()
    {
        sonidoExplosionBomba.Play();
    }

}
