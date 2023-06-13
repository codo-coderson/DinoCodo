using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Personaje : MonoBehaviour
{
    // public GameObject obstaculo;
    public float fuerza;
    public int velocidad;
    private Rigidbody2D rb;
    Animator anim;
    public Camera cam;
    public bool pantallaTocada;
    public Canvas canvasGameOver;
    public Button botonReiniciar;
    public AudioSource saltoFX;
    public AudioSource aterrizajeFX;
    public AudioSource muerteFX;
    private bool enElAire;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        velocidad = 2;
        fuerza = 10;
        anim = this.GetComponent<Animator>();
        anim.SetInteger("estado", 0);
        pantallaTocada = false;
        enElAire = true;
    }

    // Update is called once per frame
    private void Update()
    {
        cam.transform.position = new Vector3(this.transform.position.x + 8, cam.transform.position.y, cam.transform.position.z);
        
        if (Input.touchCount > 0  && anim.GetInteger("estado") == 1)
        {
            // El toque de pantalla debe procesarse en el update, pero no la física
            if (Input.GetTouch(Input.touchCount - 1).phase == TouchPhase.Began)
            {                
                pantallaTocada = true;
                Debug.Log("Pantalla tocada");
            }
        }
    }


    void FixedUpdate()
    {
        // Fijamos velocidad constante de valor "velocidad" en el eje horizontal
        // creo que hay que aplicarla desde que toca el suelo hasta el game over
        rb.velocity = new Vector2(velocidad, rb.velocity.y);
        if (pantallaTocada)
        {

            saltoFX.Play();
            rb.AddForce(new Vector2(0, fuerza), ForceMode2D.Impulse);
            enElAire = true;
            anim.SetInteger("estado", 2);
            Debug.Log("Saltando");
            pantallaTocada = false;
        }     
    }
        

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Colisión con: " + collision.gameObject.tag);
        // Hay que aplicar el impulso si "personaje" está colisionando con el suelo y se pulsa la pantalla        
        // Además, aplicar esta función provocará la llamada a OnCollisionExit2D
        if (collision.gameObject.tag.Equals("Objeto"))
        {
            canvasGameOver.gameObject.SetActive(true);
            muerteFX.Play();            
            Time.timeScale = 0.0f;
            botonReiniciar.onClick.AddListener(reiniciarEscena);
        }
        // si chocó con el suelo
        else if (collision.gameObject.tag.Equals("Suelo"))
        {            
            if (enElAire)
            { 
            aterrizajeFX.Play();
            Debug.Log("Andando");
            enElAire = false;
            }
            anim.SetInteger("estado", 1);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
        {
            //// Verificar si el objeto sigue colisionando con algún otro collider
            //Collider2D[] colliders = new Collider2D[1];
            //ContactFilter2D contactFilter = new ContactFilter2D();
            //int numColliders = collision.collider.OverlapCollider(contactFilter, colliders);
        }


    public void reiniciarEscena()
    {
        SceneManager.LoadScene("EscenaPpal");
    }
}
