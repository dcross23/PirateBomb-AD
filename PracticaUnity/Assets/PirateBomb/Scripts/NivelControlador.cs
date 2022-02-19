using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NivelControlador : MonoBehaviour
{
    [Header("UI")]
    public GameObject barraVida;
    public GameObject menuPausa;
    public int numOros;

    [Header("Sonidos")]
    public AudioSource musicaFondo;
    public AudioSource sonidoMenuPausa;
    public AudioSource sonidoCogerOro;


    protected GameObject dialogo;
    protected int volumenMusica;
    protected int volumenEfectos;
    protected int nivelesCompletados;
    protected int score;
    protected int scoreAlComenzar;
    protected Text scoreTexto;

    protected int vidasAlComenzar;
    protected bool juegoActivo;

    void Start()
    {
        CargarDatosGuardados();
        AjustarSonidos();
        juegoActivo = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoActivo) MenuPausa();
            else ContinuarJuego();
        }
    }

    void CargarDatosGuardados()
    {
        nivelesCompletados = PlayerPrefs.GetInt("NivelesCompletados", 0);
        volumenMusica = PlayerPrefs.GetInt("VolumenMusica", 3);
        volumenEfectos = PlayerPrefs.GetInt("VolumenEfectos", 3);
        score = scoreAlComenzar = PlayerPrefs.GetInt("Score", 0);

        scoreTexto = GameObject.Find("ScoreTexto").GetComponent<Text>();
        scoreTexto.text = "Score: " + scoreAlComenzar;

        vidasAlComenzar = PlayerPrefs.GetInt("VidasJugador", 3);

        for (int i=3; i> vidasAlComenzar; i--)
        {
            GameObject.Find("BarraVida").transform.GetChild(i-1).transform.localScale = new Vector3(0, 0, 0);
        }
    }

    void AjustarSonidos()
    {
        musicaFondo.volume = volumenMusica / 10f;

        foreach (GameObject sonido in GameObject.FindGameObjectsWithTag("SonidoEfectos"))
            sonido.GetComponent<AudioSource>().volume = volumenEfectos / 10f;
    }

    #region MenuPausa
    void MenuPausa()
    {
        juegoActivo = false;
        Time.timeScale = 0;
        barraVida.SetActive(false);
        menuPausa.SetActive(true);
        
        if(dialogo == null) dialogo = GameObject.Find("DialogoControles");
        if (dialogo != null) dialogo.SetActive(false);
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
        if(dialogo != null)
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


    public void NivelCompletado(string nivel)
    {
        if (nivel == "Nivel1" && nivelesCompletados == 0) nivelesCompletados++;

        if (nivel == "Nivel2" && nivelesCompletados == 1) nivelesCompletados++;

        if (nivel == "Nivel3" && nivelesCompletados == 2) nivelesCompletados++;

        PlayerPrefs.SetInt("NivelesCompletados", nivelesCompletados);
        PlayerPrefs.Save();

        ActualizarScore(100);
    }

    public void QuitarVidaJugador()
    {
        int vidas = PlayerPrefs.GetInt("VidasJugador", 3);
        GameObject.Find("BarraVida").transform.GetChild(vidas-1).gameObject.SetActive(false);
        PlayerPrefs.SetInt("VidasJugador", vidas-1);
        PlayerPrefs.Save();
        ActualizarScore(-10);
    }

    public void RecargarEscenaMuerte()
    {
        PlayerPrefs.SetInt("VidasJugador", vidasAlComenzar);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        PlayerPrefs.SetInt("Score", scoreAlComenzar);
        PlayerPrefs.Save();
        scoreTexto.text = "Score: " + scoreAlComenzar;
    }

    public void DecrementarOrosRestantes()
    {
        sonidoCogerOro.Play();
        numOros--;
        if (numOros == 0)
        {
            GameObject.FindGameObjectWithTag("PuertaSalida").GetComponent<Animator>().SetBool("abrirPuerta", true);
            PlayerPrefs.SetInt("Vida", 1);
            PlayerPrefs.Save();
        }

        ActualizarScore(5);
    }

    public void ActualizarScore(int cantidad)
    {
        score += cantidad;
        if (score < 0) score = 0;
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.Save();
        scoreTexto.text = "Score: " + score;
    }
}
