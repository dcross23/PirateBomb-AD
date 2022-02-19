using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Jugador : MonoBehaviour
{
    [Header("Configuracion Jugador")]
    public float velocidad = 2;
    public float salto = 5;
    public GameObject bomba;

    //Elementos
    protected Rigidbody2D rb;
    protected Animator anim;
    protected Collider2D collider2d;
    protected GameObject barraVidaJugador;
    protected GameObject puerta;
    protected GameObject nivelControlador;
    protected AudioSource sonidoGolpear;
    protected AudioSource sonidoRecibirHit;
    protected AudioSource sonidoMorir;

    //Valores privados script
    protected int bombas = 2;
    protected float horAxis;
    protected float verAxis;
    protected bool vivo = true;
    protected bool direccionDerecha = true;
    protected bool lanzarBomba = false;
    protected AudioSource sonidoVidaExtra;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        collider2d = GetComponent<Collider2D>();
        barraVidaJugador = GameObject.Find("VidaJugador");
        puerta = GameObject.FindGameObjectWithTag("PuertaEntrada");
        nivelControlador = GameObject.FindGameObjectWithTag("NivelControlador");

        sonidoGolpear = GameObject.Find("SonidoGolpe").GetComponent<AudioSource>();
        sonidoRecibirHit = GameObject.Find("SonidoRecibirHit").GetComponent<AudioSource>();
        sonidoMorir = GameObject.Find("SonidoMuerte").GetComponent<AudioSource>();
        sonidoVidaExtra = GameObject.Find("SonidoCogerVida").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        horAxis = Input.GetAxisRaw("Horizontal");
        verAxis = Input.GetAxisRaw("Vertical");

        if (horAxis != 0) anim.SetBool("correr", true);
        else              anim.SetBool("correr", false);

        //Se puede disparar en todos los niveles menos en el de seleccionar niveles
        if (Input.GetAxisRaw("Fire1") != 0 && SceneManager.GetActiveScene().name != "SelectorNiveles" && Time.timeScale == 1)
        {
            if (lanzarBomba == false && bombas>0)
            {
                bombas--;
                lanzarBomba = true;

                Vector3 pos;
                if (direccionDerecha) pos = new Vector3(transform.position.x+1, transform.position.y, transform.position.z);
                else                  pos = new Vector3(transform.position.x-1, transform.position.y, transform.position.z);

                Instantiate(bomba, pos, Quaternion.identity);
            }
        }
        else
            lanzarBomba = false;
    }

    void FixedUpdate()
    {
        if (vivo)
        {
            MoveHorizontal(horAxis * velocidad * Time.fixedDeltaTime * 100);
            MoveVertical(verAxis * salto * Time.fixedDeltaTime * 100);
        }
    }

    void MoveHorizontal(float velX)
    {
        //Si el jugador recibe un hit, no cambiamos la velocidad ya que si no pararía el impulso del hit
        if(!anim.GetBool("hit")) rb.velocity = new Vector2(velX, rb.velocity.y);

        //Si va hacia la derecha, y la velocidad pasa a ser negativa, cambia de orientación a la izquierda
        //Si va hacia la izquierda, y la velocidad pasa a ser positiva, cambia de orientación a la derecha
        if (direccionDerecha && velX < 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            direccionDerecha = false;
        }
        else if (!direccionDerecha && velX > 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            direccionDerecha = true;
        }
    }

    void MoveVertical(float velY)
    {
        if (anim.GetBool("estaEnSuelo") && rb.velocity.y==0 && velY > 0)
            rb.AddForce(new Vector2(0, velY), ForceMode2D.Impulse);

        //Para que haga la animación de saltar o caer
        anim.SetFloat("velocidadY", rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Enemigo"))
        {
            //Si el pirata no ha muerto, podrá quitarnos Esto es para la animacion de muerte, que podría darnos si no
            if (!collision.gameObject.GetComponent<Animator>().GetBool("morir"))
                RecibirHit(collision.transform.position.x > transform.position.x);
        
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PuertaEntrada") || collision.gameObject.CompareTag("PuertaSalida"))
        {
            puerta = collision.gameObject;
        }
        else if (collision.gameObject.CompareTag("Oro"))
        {
            Destroy(collision.gameObject);
            nivelControlador.GetComponent<NivelControlador>().DecrementarOrosRestantes();
        }
        else if (collision.gameObject.CompareTag("Vida"))
        {
            AddVida();
        }
    }

    public void RecibirHit(bool fromRight)
    {
        anim.SetBool("hit", true);
        rb.velocity = Vector2.zero;

        float forceImpulse = 40f;
        //Dependiendo de a que lado del enemigo esté el jugador, el impulso será hacia
        // el lado contrario
        if (fromRight) rb.AddForce(new Vector2(-forceImpulse, forceImpulse), ForceMode2D.Impulse);
        else           rb.AddForce(new Vector2( forceImpulse, forceImpulse), ForceMode2D.Impulse);

        QuitarVida();
    }

    public void AddVida()
    {
        int vidas = PlayerPrefs.GetInt("VidasJugador", 3);
        if (vidas < 3)
        {
            nivelControlador.GetComponent<SeleccionControlador>().AddVidaJugador();
            sonidoVidaExtra.Play();
        }
       
    }

    public void QuitarVida()
    {
        int vidas = PlayerPrefs.GetInt("VidasJugador", 3);

        if (vidas > 0)
        {
            nivelControlador.GetComponent<NivelControlador>().QuitarVidaJugador();
            if (PlayerPrefs.GetInt("VidasJugador") == 0)
            {
                vivo = false;
                anim.SetBool("morir", true);
            }
        }
    }

    public void Muerte()
    {
        anim.SetBool("morir", false);
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        sonidoMorir.Play();
    }

    public void HaEntradoPuerta()
    {
        puerta.GetComponent<Animator>().SetBool("cerrarPuerta", true);
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void FinHit()
    {
        anim.SetBool("hit", false);
    }

    public void RecuperarBomba()
    {
        bombas++;
    }

    public void JugadorMuerto()
    {
        nivelControlador.GetComponent<NivelControlador>().RecargarEscenaMuerte();
    }

    public void SonidoAlRecibirHit()
    {
        sonidoRecibirHit.Play();
    }
}
