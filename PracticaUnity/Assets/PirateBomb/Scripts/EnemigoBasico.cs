using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemigoBasico : MonoBehaviour
{
    public Animator anim;
    public Jugador jugador;
    public SpriteRenderer sprite;
    public Transform[] wayPoints;

    [Header("Propiedades")]
    public float velocidad = 1f;
    public float minTiempoEspera = 1f;
    public float maxTiempoEspera = 2f;

    protected AudioSource efectoGolpe;
    protected AudioSource sonidoMorir;
    protected float espera;
    protected Vector2 pos;
    protected int puntoActual;
    protected bool direccionDerecha = true;


    // Start is called before the first frame update
    void Start()
    {
        espera = (Random.value % (maxTiempoEspera-minTiempoEspera+1)) + minTiempoEspera;
        puntoActual = 0;
        efectoGolpe = GameObject.Find("SonidoGolpe").GetComponent<AudioSource>();
        sonidoMorir = GameObject.Find("SonidoMuerte").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!anim.GetBool("morir"))
        {
            Desplazar();
            BuscarJugador();
        }
    }

    private void Desplazar()
    {
        //Esto sirve para cambiar de dirección al enemigo
        if (wayPoints[puntoActual].transform.position.x <= transform.position.x && direccionDerecha)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            direccionDerecha = false;
        }
        else if (wayPoints[puntoActual].transform.position.x > transform.position.x && !direccionDerecha)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            direccionDerecha = true;
        }

        //Una vez cambia de dirección lo movemos hacia ese punto
        transform.position = Vector2.MoveTowards(transform.position, wayPoints[puntoActual].transform.position, velocidad * Time.deltaTime);

        //Si el enemigo está llegando al waypoint, espera un poco parado y pasa al siguiente punto
        if (Vector2.Distance(transform.position, wayPoints[puntoActual].transform.position) < 0.1f)
        {
            if (espera <= 0)
            {
                anim.SetBool("correr", true);

                //Si no es el ultimo punto, pasa al siguiente
                if (puntoActual != (wayPoints.Length - 1)) puntoActual++;
                else puntoActual = 0;

                espera = (Random.value % (maxTiempoEspera - minTiempoEspera + 1)) + minTiempoEspera;
            }
            else
            {
                anim.SetBool("correr", false);
                espera -= Time.deltaTime;
            }
        }
    }

    private void BuscarJugador()
    {
        Collider2D[] collider2D = Physics2D.OverlapCircleAll(transform.position, 2f);

        for (int i = 0; i < collider2D.Length; i++) { 
            if (collider2D[i].gameObject.tag == "Jugador")
            {
                //El jugador esta la derecha del pirata
                if(collider2D[i].gameObject.transform.position.x >= transform.position.x && direccionDerecha)
                    anim.SetBool("atacar", true);

                if (collider2D[i].gameObject.transform.position.x <= transform.position.x && !direccionDerecha)
                    anim.SetBool("atacar", true);
            }
        }
    }

    public void HitJugador()
    {
        Collider2D[] collider2D = Physics2D.OverlapCircleAll(transform.position, 2f);

        for (int i = 0; i < collider2D.Length; i++)
        {
            if (collider2D[i].gameObject.tag == "Jugador")
                jugador.RecibirHit(transform.position.x > jugador.transform.position.x);
        }

        anim.SetBool("atacar", false);
    }

    public void explosionBomba()
    {
        anim.SetBool("morir", true);
        this.gameObject.GetComponent<Collider2D>().isTrigger = true;
    }

    public void Destruir()
    {
        Destroy(this.gameObject);
        Destroy(this.transform.parent.gameObject);
        jugador.GetComponent<Animator>().SetBool("estaEnSuelo", true);
        GameObject.FindGameObjectWithTag("NivelControlador").GetComponent<NivelControlador>().ActualizarScore(20);
    }

    public void SonidoAtaque()
    {
        efectoGolpe.Play();
    }

    public void SonidoAlMorir()
    {
        sonidoMorir.Play();
    }
}
