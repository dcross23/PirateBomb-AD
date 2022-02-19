using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SeleccionControlador : MonoBehaviour
{
    [Header("Elementos escena")]
    public GameObject barraVida;
    public GameObject menuPausa;
    public GameObject[] puertas;

    [Header("Sonidos")]
    public AudioSource musicaFondo;
    public AudioSource sonidoMenuPausa;

    protected GameObject dialogo;
    protected int volumenMusica;
    protected int volumenEfectos;
    protected int nivelesCompletados;
    protected int score;
    protected bool juegoActivo;
    protected int crearVida;


    // Start is called before the first frame update
    void Start()
    {
        CargarDatosGuardados();
        AjustarSonidos();
        AbrirPuertasNivelesCompletados();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoActivo) MenuPausa();
            else ContinuarJuego();
        }
    }

    #region MenuPausa
    void MenuPausa()
    {
        juegoActivo = false;
        Time.timeScale = 0;
        barraVida.SetActive(false);
        menuPausa.SetActive(true);

        if (dialogo == null) dialogo = GameObject.Find("DialogoControles");
        if (dialogo != null)  dialogo.SetActive(false);
    }

    public void ContinuarJuego()
    {
        sonidoMenuPausa.Play();
        juegoActivo = true;
        Time.timeScale = 1;
        barraVida.SetActive(true);
        menuPausa.SetActive(false);
    }

    public void MenuPrincipal()
    {
        sonidoMenuPausa.Play();
        SceneManager.LoadScene("MenuPrincipal");
        Time.timeScale = 1;
    }

    public void Controles()
    {
        if (dialogo != null)
        {
            if (dialogo.activeInHierarchy) dialogo.SetActive(false);
            else dialogo.SetActive(true);
        }

    }

    public void SeleccionarNivel()
    {
        sonidoMenuPausa.Play();
        SceneManager.LoadScene("SelectorNiveles");
        Time.timeScale = 1;
    }

    public void AcabarJuego()
    {
        sonidoMenuPausa.Play();
        Application.Quit();
    }
    #endregion

    void CargarDatosGuardados()
    {
        nivelesCompletados = PlayerPrefs.GetInt("NivelesCompletados", 0);
        volumenMusica = PlayerPrefs.GetInt("VolumenMusica", 3);
        volumenEfectos = PlayerPrefs.GetInt("VolumenEfectos", 3);
        score = PlayerPrefs.GetInt("Score", 0);
        GameObject.Find("ScoreTexto").GetComponent<Text>().text = "Score: " + score;

        crearVida = PlayerPrefs.GetInt("Vida", 0);
        if (crearVida == 1)  GameObject.FindGameObjectWithTag("Vida").SetActive(true);
        else GameObject.FindGameObjectWithTag("Vida").SetActive(false);

        for (int i = 3; i > PlayerPrefs.GetInt("VidasJugador", 3); i--)
        {
            barraVida.transform.GetChild(i - 1).gameObject.SetActive(false);
        }
    }

    void AjustarSonidos()
    {
        musicaFondo.volume = volumenMusica / 10f;

        foreach (GameObject sonido in GameObject.FindGameObjectsWithTag("SonidoEfectos"))
            sonido.GetComponent<AudioSource>().volume = volumenEfectos / 10f;
    }

    void AbrirPuertasNivelesCompletados()
    {
        for (int i=0; i<=nivelesCompletados; i++)
        {
            if(i < 3) puertas[i+1].GetComponent<Animator>().SetBool("abrirPuerta", true);
        }
    }

    public void AddVidaJugador()
    {
        PlayerPrefs.SetInt("Vida", 0);
        GameObject.FindGameObjectWithTag("Vida").SetActive(false);

        int vidas = PlayerPrefs.GetInt("VidasJugador", 3);
        GameObject.Find("BarraVida").transform.GetChild(vidas).gameObject.SetActive(true);
        PlayerPrefs.SetInt("VidasJugador", vidas + 1);
        PlayerPrefs.Save();
    }
}
