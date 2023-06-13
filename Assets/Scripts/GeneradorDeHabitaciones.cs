using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneradorDeHabitaciones : MonoBehaviour
{

    public GameObject[] habitacionesDisponibles;
    //con public GameObject pantalla;
        //Despu�s podremos hacer algo como
        //habitacion = habitacionesDisponibles[i];
        //habitacion.transform.position = new Vector2(x, y);
    public List<GameObject> habitacionesActuales;
    private float anchoDePantallaEnPuntos;

    // Start is called before the first frame update
    void Start()
    {
        float altura = 2.0f * Camera.main.orthographicSize;
        anchoDePantallaEnPuntos = altura * Camera.main.aspect;
        StartCoroutine(ComprobacionGenerador());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AgregarHabitacion(float ExtremoMasLejanoHabitacionX)
    {
        // Elige un �ndice aleatorio del tipo de habitaci�n (Prefab) para generar
        int indiceAleatorioDeHabitacion = Random.Range(0, habitacionesDisponibles.Length);
        // Crea un objeto habitaci�n aleatorio a partir del array de habitaciones disponibles
        GameObject habitacion = (GameObject)Instantiate(habitacionesDisponibles[indiceAleatorioDeHabitacion]);
        // Obtiene el tama�o del suelo dentro de la habitaci�n, que es igual a la anchura de la habitaci�n

        float anchoDeHabitacion = habitacion.transform.Find("suelo").localScale.x;

        // Para situar la nueva habitaci�n en su lugar correcto, hay que calcular d�nde debe estar su centro.
        // Cogemos el borde m�s alejado del nivel hasta ahora y a�adimos la mitad de la anchura de la nueva habitaci�n.
        // De este modo, la nueva habitaci�n empezar� exactamente donde termin� la anterior
        float centroHabitacion = ExtremoMasLejanoHabitacionX + anchoDeHabitacion * 0.5f;
        // Esto fija la posici�n de la habitaci�n. S�lo es necesario cambiar la coordenada x,
        // ya que todas las habitaciones tienen las mismas coordenadas y y z iguales a cero
        habitacion.transform.position = new Vector3(centroHabitacion, 0, 0);
        /*
         * habitacion.transform.position = new Vector3(centroDePantalla, habitacion.transform.position.y, 0);
         */

        // Por �ltimo, a�ade la sala a la lista de salas actuales. Se borrar� en el siguiente m�todo,
        // por lo que es necesario mantener esta lista
        habitacionesActuales.Add(habitacion);
    }

    private void GeneraHabitacionesSiNecesario()
    {
        // Crea una nueva lista para almacenar las habitaciones que necesitan ser eliminadas.
        // Se requiere una lista separada ya que no se pueden eliminar elementos de la lista mientras se itera a trav�s de ella
        List<GameObject> HabitacionesAEliminar = new List<GameObject>();

        // Booleano para si necesito a�adir m�s habitaciones.
        // Por defecto se establece en true, pero la mayor�a de las veces se establecer� en false dentro del primer bucle foreach
        bool necesitoAgregarHabitaciones = true;

        // Guarda la posici�n del jugador (Aunque la mayor�a de las veces s�lo se usa la x cuando se trabaja con la posici�n del rat�n)
        // �Necesario?
        float personajeX = transform.position.x;

        // Este es el punto despu�s del cual la habitaci�n ya pasada debe ser eliminada para no quedarnos sin memoria
        float quitarHabitacionX = personajeX - anchoDePantallaEnPuntos;

        // Si no hay ninguna habitaci�n despu�s del punto agregarPantallaX, entonces necesito a�adir una habitaci�n,
        // ya que el final del nivel est� m�s cerca que el ancho de la pantalla
        float agregarHabitacionX = personajeX + anchoDePantallaEnPuntos;

        // Punto donde el nivel termina en ese momento. Esta variable sirve para a�adir una nueva sala si es necesario
        float extremoMasLejanoPantallaX = 0;


        //Debug.Log("N�mero de habitaciones actuales: " + habitacionesActuales.Count);

        foreach (var habitacion in habitacionesActuales)
        {
            // Enumeraci�n de las habitaciones existentes actualmente.
            // Utiliza el suelo para obtener el ancho de la habitacion y calcula comienzoHabitacionX y finHabitacionX
            //Debug.Log(habitacion.name);            
            
            //if (habitacion == null) Debug.Log("�habitacion es null!");
            //Debug.Log("Ancho de la habitaci�n: " + habitacion.transform.Find("suelo").localScale.x);
            float anchoHabitacion = habitacion.transform.Find("suelo").localScale.x;                        
            float comienzoHabitacionX = habitacion.transform.position.x - (anchoHabitacion * 0.5f);
            float finHabitacionX = comienzoHabitacionX + anchoHabitacion;

            // Si hay una pantalla que comienza despu�s de agregarPantallaX entonces no es necesario a�adir pantallas en este momento.
            // Pero no hay ninguna instrucci�n de ruptura aqu�, ya que todav�a hay que comprobar si esta pantalla
            // necesita ser eliminada

            // Debug.Log(comienzoHabitacionX + " - " + agregarHabitacionX);
            if (comienzoHabitacionX > agregarHabitacionX)
            {
                necesitoAgregarHabitaciones = false;
            }
            // Si la pantalla termina a la izquierda del punto quitarPantallaX, entonces ya est� fuera de la pantalla
            // y hay que eliminarla
            if (finHabitacionX < quitarHabitacionX)
            {
                HabitacionesAEliminar.Add(habitacion);
            }
            // Este es el punto m�s a la derecha. Se usa s�lo si necesitas a�adir una pantalla
            extremoMasLejanoPantallaX = Mathf.Max(extremoMasLejanoPantallaX, finHabitacionX);
        }

        // Esto quita habitaciones que est�n marcadas para ser quitadas
        foreach (var pantalla in HabitacionesAEliminar)
        {
            habitacionesActuales.Remove(pantalla);
            Destroy(pantalla);
        }

        // Si en este punto agregarHabitaciones es todav�a true entonces el final del nivel est� cerca.
        // agregarHabitaciones ser� true si no se encuentra una habitaci�n que comience m�s lejos que el ancho de la habitaci�n.
        // Esto indica que es necesario a�adir una nueva
        if (necesitoAgregarHabitaciones)
        {
            AgregarHabitacion(extremoMasLejanoPantallaX);
        }
    }


    // El bucle while asegurar� que GeneraHabitacionesSiNecesario se ejecuta tantas veces como se necesite
    // mientras el juego est� corriendo y el GameObject est� activo.
    // Se utiliza una sentencia yield para a�adir una pausa de 0,25 segundos en la ejecuci�n entre cada iteraci�n del bucle
    // para evitar problemas de rendimiento, pues las operaciones con listas son costosas
    // 
    private IEnumerator ComprobacionGenerador()
    {
        while (true)
        {
            GeneraHabitacionesSiNecesario();
            yield return new WaitForSeconds(0.25f);
        }
    }

}
