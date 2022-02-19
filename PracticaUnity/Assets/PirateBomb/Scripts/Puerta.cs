using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Puerta : MonoBehaviour
{
    protected AudioSource sonidoPuertaAbrir;
    protected AudioSource sonidoPuertaCerrar;

    protected GameObject jugador;
    protected Animator anim;

    protected bool jugadorEnPuerta;

    // Start is called before the first frame update
    void Start()
    {
        jugadorEnPuerta = false;
        anim = GetComponent<Animator>();
        jugador = GameObject.FindGameObjectWithTag("Jugador");

        sonidoPuertaAbrir = GameObject.Find("SonidoPuertaAbrir").GetComponent<AudioSource>();
        sonidoPuertaCerrar = GameObject.Find("SonidoPuertaCerrar").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //Si la puerta está abierta, presionar el boton "Submit" para entrar
        if(Input.GetButtonDown("Submit") && jugadorEnPuerta && transform.CompareTag("PuertaSalida") && anim.GetBool("abrirPuerta") == true)
        {
            //Para que el jugador haga la animación de entrar quieto, bloqueo el movimiento
            jugador.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            jugador.GetComponent<Animator>().SetBool("entrarPuerta", true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Jugador"))
        {
            jugadorEnPuerta = true;

            if (transform.gameObject.CompareTag("PuertaEntrada"))
            {
                if (anim.GetBool("cerrarPuerta") == false)
                {
                    //Para que el jugador haga la animación de salir quieto, bloqueo el movimiento 
                    jugador.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Jugador"))
            jugadorEnPuerta = false;
    }

    public void NuevaEscena()
    {
        string escena = SceneManager.GetActiveScene().name;
        if (transform.gameObject.CompareTag("PuertaSalida"))
        {
            if (escena == "Nivel1" || escena == "Nivel2")
            {
                GameObject.FindGameObjectWithTag("NivelControlador").GetComponent<NivelControlador>().NivelCompletado(escena);
                SceneManager.LoadScene("SelectorNiveles");
            }
            else if(escena == "Nivel3")
            {
                GameObject.FindGameObjectWithTag("NivelControlador").GetComponent<NivelControlador>().NivelCompletado(escena);
                SceneManager.LoadScene("Creditos");
            }
            else if(escena == "SelectorNiveles")
            {
                string nivel = transform.gameObject.name.Remove(0, 6);
                SceneManager.LoadScene(nivel);
            }
        }
    
    }

    public void SonidoPuertaAbrir()
    {
        sonidoPuertaAbrir.Play();
    }

    public void SonidoPuertaCerrar()
    {
        sonidoPuertaCerrar.Play();
    }
}
