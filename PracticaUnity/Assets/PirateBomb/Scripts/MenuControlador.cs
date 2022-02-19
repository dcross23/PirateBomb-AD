using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControlador : MonoBehaviour
{
    protected enum Pantallas { PantallaMenu, PantallaOpciones }
    protected enum OpcionesMenuPrincipal { Comenzar, Opciones, Salir }
    protected enum OpcionesMenuOpciones { Musica, Efectos, ReiniciarNiveles, Volver }

    [Header("Pantallas")]
    public GameObject pantallaMenu;
    public GameObject pantallaOpciones;

    [Header("Opciones Menu Principal")]
    public SpriteRenderer comenzar;
    public SpriteRenderer opciones;
    public SpriteRenderer salir;

    [Header("Sprites Menu Principal")]
    public Sprite comenzar_on;
    public Sprite comenzar_off;
    public Sprite opciones_on;
    public Sprite opciones_off;
    public Sprite salir_on;
    public Sprite salir_off;

    [Header("Opciones Menu Opciones")]
    public SpriteRenderer musica;
    public SpriteRenderer efectos;
    public SpriteRenderer reiniciarNiveles;
    public SpriteRenderer volver;

    [Header("Sprites Menu Opciones")]
    public Sprite musica_on;
    public Sprite musica_off;
    public SpriteRenderer[] volMusica;
    public Sprite efectos_on;
    public Sprite efectos_off;
    public SpriteRenderer[] volEfectos;
    public Sprite reiniciarNiveles_on;
    public Sprite reiniciarNiveles_off;
    public Sprite volver_on;
    public Sprite volver_off;
    public Sprite volumen_on;
    public Sprite volumen_off;

    [Header("Sonidos")]
    public AudioSource musicaFondo;
    public AudioSource sonidoCambioOpcion;
    public AudioSource sonidoSeleccionarOpcion;

    //Privados
    protected int volumenMusica;
    protected int volumenEfectos;
    protected Pantallas pantalla;
    protected OpcionesMenuPrincipal opcionSelMenuPrinc, opcionAntMenuPrinc;
    protected OpcionesMenuOpciones opcionSelMenuOpc, opcionAntMenuOpc;
    protected float verAxis, horAxis, tiempoV, tiempoH;

    // Start is called before the first frame update
    void Start()
    {
        pantalla = Pantallas.PantallaMenu;
        opcionSelMenuPrinc = opcionAntMenuPrinc = OpcionesMenuPrincipal.Comenzar;
        tiempoV = tiempoH = 0;

        CargarOpcionesGuardadas();
        AjustarMusicaFondo();
        AjustarEfectos();
    }

    // Update is called once per frame
    void Update()
    {
        verAxis = Input.GetAxisRaw("Vertical");
        horAxis = Input.GetAxisRaw("Horizontal");

        if (verAxis == 0) tiempoV = 0;
        if (horAxis == 0) tiempoH = 0;

        switch (pantalla)
        {
            case Pantallas.PantallaMenu: MenuPrincipal(); break;
            case Pantallas.PantallaOpciones: MenuOpciones();  break;
        }
    }

    #region Opciones
    void CargarOpciones()
    {
        pantallaMenu.SetActive(false);
        pantalla = Pantallas.PantallaOpciones;
        opcionSelMenuOpc = opcionAntMenuOpc = OpcionesMenuOpciones.Musica;
        musica.sprite = musica_on;
        efectos.sprite = efectos_off;
        reiniciarNiveles.sprite = reiniciarNiveles_off;
        volver.sprite = volver_off;
        pantallaOpciones.SetActive(true);
        GameObject.Find("Tick").transform.localScale = new Vector3(0, 0, 0);
    }

    void MenuOpciones()
    { 
        //Cambia la opcion si le damos al boton
        if (verAxis != 0)
        {
            if (tiempoV == 0 || tiempoV > 0.2f)
            {
                if (verAxis == 1 && opcionSelMenuOpc > OpcionesMenuOpciones.Musica)   SeleccionarOpcion(opcionSelMenuOpc - 1);
                if (verAxis == -1 && opcionSelMenuOpc < OpcionesMenuOpciones.Volver) SeleccionarOpcion(opcionSelMenuOpc + 1);

                if (tiempoV > 0.2f) tiempoV = 0;
            }
            tiempoV += Time.deltaTime;
        }


        if (horAxis == 0) 
            tiempoH = 0;
        else
        {
            if ((tiempoH == 0 || tiempoH > 0.2f) && (opcionSelMenuOpc==OpcionesMenuOpciones.Musica || opcionSelMenuOpc==OpcionesMenuOpciones.Efectos))
            {
                if(opcionSelMenuOpc == OpcionesMenuOpciones.Musica)
                {
                    if((horAxis<0 && volumenMusica>0) || (horAxis>0 && volumenMusica < 10))
                    {
                        volumenMusica += (int) horAxis;
                        AjustarMusicaFondo();
                        sonidoCambioOpcion.Play();
                    }
                }

                if (opcionSelMenuOpc == OpcionesMenuOpciones.Efectos)
                {
                    if ((horAxis < 0 && volumenEfectos > 0) || (horAxis > 0 && volumenEfectos < 10))
                    {
                        volumenEfectos += (int)horAxis;
                        AjustarEfectos();
                        sonidoCambioOpcion.Play();
                    }
                }

                if (tiempoH > 0.2f) tiempoH = 0;
            }
            tiempoH += Time.deltaTime;
        }

        //Selecciona la opcion si le damos al enter
        if (Input.GetButtonDown("Submit"))
        {
            switch (opcionSelMenuOpc)
            {
                case OpcionesMenuOpciones.Musica: break;
                case OpcionesMenuOpciones.Efectos: break;
                case OpcionesMenuOpciones.ReiniciarNiveles: ReiniciarNiveles(); break;

                case OpcionesMenuOpciones.Volver: GuardarOpciones(); 
                                                  CargarMenu(); 
                                                  sonidoSeleccionarOpcion.Play(); break;
            }
        }
    }

    void SeleccionarOpcion(OpcionesMenuOpciones opcion)
    {
        sonidoCambioOpcion.Play();
        switch (opcion)
        {
            case OpcionesMenuOpciones.Musica:  musica.sprite = musica_on; break;
            case OpcionesMenuOpciones.Efectos: efectos.sprite = efectos_on; break;
            case OpcionesMenuOpciones.ReiniciarNiveles: reiniciarNiveles.sprite = reiniciarNiveles_on; break;
            case OpcionesMenuOpciones.Volver: volver.sprite = volver_on; break;
        }

        switch (opcionAntMenuOpc)
        {
            case OpcionesMenuOpciones.Musica: musica.sprite = musica_off; break;
            case OpcionesMenuOpciones.Efectos: efectos.sprite = efectos_off; break;
            case OpcionesMenuOpciones.ReiniciarNiveles: reiniciarNiveles.sprite = reiniciarNiveles_off; break;
            case OpcionesMenuOpciones.Volver: volver.sprite = volver_off; break;
        }

        opcionSelMenuOpc = opcionAntMenuOpc = opcion;
    }

    void ReiniciarNiveles()
    {
        sonidoSeleccionarOpcion.Play();
        PlayerPrefs.SetInt("NivelesCompletados", 0);
        PlayerPrefs.SetInt("VidasJugador", 3);
        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.SetInt("Vida", 0);
        PlayerPrefs.Save();
        GameObject.Find("Tick").transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
    }

    void AjustarMusicaFondo()
    {
        for(int i=0; i<volumenMusica; i++)
        {
            volMusica[i].sprite = volumen_on;
        }

        for (int i = volumenMusica; i < 10; i++)
        {
            volMusica[i].sprite = volumen_off;
        }

        musicaFondo.volume = volumenMusica / 10f;
    }

    void AjustarEfectos()
    {
        for (int i = 0; i < volumenEfectos; i++)
        {
            volEfectos[i].sprite = volumen_on;
        }

        for (int i = volumenEfectos; i < 10; i++)
        {
            volEfectos[i].sprite = volumen_off;
        }

        foreach(GameObject sonido in GameObject.FindGameObjectsWithTag("SonidoEfectos"))
            sonido.GetComponent<AudioSource>().volume = volumenEfectos / 10f;
    }

    void GuardarOpciones()
    {
        PlayerPrefs.SetInt("VolumenMusica", volumenMusica);
        PlayerPrefs.SetInt("VolumenEfectos", volumenEfectos);
        PlayerPrefs.Save();
    }

    void CargarOpcionesGuardadas()
    {
        volumenMusica = PlayerPrefs.GetInt("VolumenMusica", 5);
        volumenEfectos = PlayerPrefs.GetInt("VolumenEfectos", 5);
    }
    #endregion

    #region MenuPrincipal
    void CargarMenu()
    {
        pantallaOpciones.SetActive(false);
        pantalla = Pantallas.PantallaMenu;
        pantallaMenu.SetActive(true);
    }

    void MenuPrincipal()
    {
        //Cambia la opcion si le damos al boton
        if (verAxis != 0)
        {
            if (tiempoV==0 || tiempoV > 0.2f)
            {
                if (verAxis ==  1 && opcionSelMenuPrinc > OpcionesMenuPrincipal.Comenzar)  Seleccionar(opcionSelMenuPrinc - 1);
                if (verAxis == -1 && opcionSelMenuPrinc < OpcionesMenuPrincipal.Salir)     Seleccionar(opcionSelMenuPrinc + 1);

                if (tiempoV > 0.2f) tiempoV = 0;
            }
            tiempoV += Time.deltaTime;
        }
        
        //Selecciona la opcion si le damos al enter
        if(Input.GetButtonDown("Submit"))
        {
            sonidoSeleccionarOpcion.Play();     
            switch (opcionSelMenuPrinc)
            {
                case OpcionesMenuPrincipal.Comenzar: SceneManager.LoadScene("SelectorNiveles"); break;
                case OpcionesMenuPrincipal.Opciones: CargarOpciones(); break;
                case OpcionesMenuPrincipal.Salir:    Application.Quit(); break;
            }
        }
    }

    void Seleccionar(OpcionesMenuPrincipal opcion)
    {
        sonidoCambioOpcion.Play();
        switch (opcion)
        {
            case OpcionesMenuPrincipal.Comenzar: comenzar.sprite = comenzar_on; break;
            case OpcionesMenuPrincipal.Opciones: opciones.sprite = opciones_on; break;
            case OpcionesMenuPrincipal.Salir: salir.sprite = salir_on;    break;
        }

        switch (opcionAntMenuPrinc)
        {
            case OpcionesMenuPrincipal.Comenzar: comenzar.sprite = comenzar_off; break;
            case OpcionesMenuPrincipal.Opciones: opciones.sprite = opciones_off; break;
            case OpcionesMenuPrincipal.Salir: salir.sprite = salir_off;    break;
        }

        opcionSelMenuPrinc = opcionAntMenuPrinc = opcion;
    }
    #endregion
}
