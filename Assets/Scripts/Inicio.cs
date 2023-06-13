using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inicio : MonoBehaviour
{
    public Canvas canvasComenzar;
    private bool juegoActivo = false;
    public AudioSource musica;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0.0f;
        juegoActivo = false;
    }

    void Update()
    {

        if (Input.touchCount > 0 && juegoActivo == false)
        {


            // El toque de pantalla debe procesarse en el update
            if (Input.GetTouch(Input.touchCount - 1).phase == TouchPhase.Began)
            {
                Time.timeScale = 1.0f;
                canvasComenzar.gameObject.SetActive(false);
                juegoActivo = true;
                musica.Play();
            }
        }
    }

}
