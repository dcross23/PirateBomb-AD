using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraJugador : MonoBehaviour
{
    public GameObject jugadorSeguir;
    public float vistaCompleta;
    public float vistaJugador;

    protected float margenX;
    protected float margenY;
    protected float currentVelocityX;
    protected float currentVelocityY;
    protected bool isSeguirJugador = false;


    // Start is called before the first frame update
    void Start()
    {
        Vector2 vector2min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 vector2max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        margenX = (vector2max.x - vector2min.x) / 2.3f;
        margenY = (vector2max.y - vector2min.y) / 2.3f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("c")) 
            isSeguirJugador = !isSeguirJugador;

        if (isSeguirJugador)
        {
            Camera.main.orthographicSize = vistaJugador;
            seguirJugador();
        }
        else 
        {
            Camera.main.transform.position = new Vector3(1,2, Camera.main.transform.position.z);
            Camera.main.orthographicSize = vistaCompleta;
        }
    }

    private void seguirJugador()
    {
        Vector2 v2min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 v2max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        Vector3 nuevaPos = new Vector3();

        float despJugadorX = Mathf.Clamp(jugadorSeguir.transform.position.x, v2min.x + margenX, v2max.x - margenX);
        despJugadorX = jugadorSeguir.transform.position.x - despJugadorX;


        float despJugadorY = Mathf.Clamp(jugadorSeguir.transform.position.y, v2min.y + margenY, v2max.y - margenY);
        despJugadorY = jugadorSeguir.transform.position.y - despJugadorY;


        //para hacer que el movimiento de la camara sea suave
        nuevaPos.x = Mathf.SmoothDamp(transform.position.x, transform.position.x + despJugadorX, ref currentVelocityX, 0.1f);
        nuevaPos.y = Mathf.SmoothDamp(transform.position.y, transform.position.y + despJugadorY, ref currentVelocityY, 0.1f);
        nuevaPos.z = Camera.main.transform.position.z;
        
        Camera.main.transform.position = nuevaPos;
    }
}
