using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditosControlador : MonoBehaviour
{
    protected int volumenMusica;
    protected int volumenEfectos;
    protected int score;

    void Start()
    {
        CargarDatosGuardados();
        AjustarSonidos();

        GameObject.Find("SonidoVictoria").GetComponent<AudioSource>().Play();
        GameObject.Find("MusicaFondo").GetComponent<AudioSource>().Stop();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("SelectorNiveles");
        }
    }

    void CargarDatosGuardados()
    {
        volumenMusica = PlayerPrefs.GetInt("VolumenMusica", 3);
        volumenEfectos = PlayerPrefs.GetInt("VolumenEfectos", 3);
        score = PlayerPrefs.GetInt("Score", 0);
        GameObject.Find("ScoreTexto").GetComponent<Text>().text = "Score: " + score;
    }

    void AjustarSonidos()
    {
        foreach (GameObject sonido in GameObject.FindGameObjectsWithTag("SonidoEfectos"))
            sonido.GetComponent<AudioSource>().volume = volumenEfectos / 10f;
    }
}
