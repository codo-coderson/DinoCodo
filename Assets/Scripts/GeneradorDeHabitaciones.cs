using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneradorDeHabitaciones : MonoBehaviour
{

    public GameObject[] habitacionesDisponibles;
    //con public GameObject pantalla;
        //Después podremos hacer algo como
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
        // Elige un índice aleatorio del tipo de habitación (Prefab) para generar
        int indiceAleatorioDeHabitacion = Random.Range(0, habitacionesDisponibles.Length);
        // Crea un objeto habitación aleatorio a partir del array de habitaciones disponibles
        GameObject habitacion = (GameObject)Instantiate(habitacionesDisponibles[indiceAleatorioDeHabitacion]);
        // Obtiene el tamaño del suelo dentro de la habitación, que es igual a la anchura de la habitación

        float anchoDeHabitacion = habitacion.transform.Find("suelo").localScale.x;

        // Para situar la nueva habitación en su lugar correcto, hay que calcular dónde debe estar su centro.
        // Cogemos el borde más alejado del nivel hasta ahora y añadimos la mitad de la anchura de la nueva habitación.
        // De este modo, la nueva habitación empezará exactamente donde terminó la anterior
        float centroHabitacion = ExtremoMasLejanoHabitacionX + anchoDeHabitacion * 0.5f;
        // Esto fija la posición de la habitación. Sólo es necesario cambiar la coordenada x,
        // ya que todas las habitaciones tienen las mismas coordenadas y y z iguales a cero
        habitacion.transform.position = new Vector3(centroHabitacion, 0, 0);
        /*
         * habitacion.transform.position = new Vector3(centroDePantalla, habitacion.transform.position.y, 0);
         */

        // Por último, añade la sala a la lista de salas actuales. Se borrará en el siguiente método,
        // por lo que es necesario mantener esta lista
        habitacionesActuales.Add(habitacion);
    }

    private void GeneraHabitacionesSiNecesario()
    {
        // Crea una nueva lista para almacenar las habitaciones que necesitan ser eliminadas.
        // Se requiere una lista separada ya que no se pueden eliminar elementos de la lista mientras se itera a través de ella
        List<GameObject> HabitacionesAEliminar = new List<GameObject>();

        // Booleano para si necesito añadir más habitaciones.
        // Por defecto se establece en true, pero la mayoría de las veces se establecerá en false dentro del primer bucle foreach
        bool necesitoAgregarHabitaciones = true;

        // Guarda la posición del jugador (Aunque la mayoría de las veces sólo se usa la x cuando se trabaja con la posición del ratón)
        // ¿Necesario?
        float personajeX = transform.position.x;

        // Este es el punto después del cual la habitación ya pasada debe ser eliminada para no quedarnos sin memoria
        float quitarHabitacionX = personajeX - anchoDePantallaEnPuntos;

        // Si no hay ninguna habitación después del punto agregarPantallaX, entonces necesito añadir una habitación,
        // ya que el final del nivel está más cerca que el ancho de la pantalla
        float agregarHabitacionX = personajeX + anchoDePantallaEnPuntos;

        // Punto donde el nivel termina en ese momento. Esta variable sirve para añadir una nueva sala si es necesario
        float extremoMasLejanoPantallaX = 0;


        //Debug.Log("Número de habitaciones actuales: " + habitacionesActuales.Count);

        foreach (var habitacion in habitacionesActuales)
        {
            // Enumeración de las habitaciones existentes actualmente.
            // Utiliza el suelo para obtener el ancho de la habitacion y calcula comienzoHabitacionX y finHabitacionX
            //Debug.Log(habitacion.name);            
            
            //if (habitacion == null) Debug.Log("¡habitacion es null!");
            //Debug.Log("Ancho de la habitación: " + habitacion.transform.Find("suelo").localScale.x);
            float anchoHabitacion = habitacion.transform.Find("suelo").localScale.x;                        
            float comienzoHabitacionX = habitacion.transform.position.x - (anchoHabitacion * 0.5f);
            float finHabitacionX = comienzoHabitacionX + anchoHabitacion;

            // Si hay una pantalla que comienza después de agregarPantallaX entonces no es necesario añadir pantallas en este momento.
            // Pero no hay ninguna instrucción de ruptura aquí, ya que todavía hay que comprobar si esta pantalla
            // necesita ser eliminada

            // Debug.Log(comienzoHabitacionX + " - " + agregarHabitacionX);
            if (comienzoHabitacionX > agregarHabitacionX)
            {
                necesitoAgregarHabitaciones = false;
            }
            // Si la pantalla termina a la izquierda del punto quitarPantallaX, entonces ya está fuera de la pantalla
            // y hay que eliminarla
            if (finHabitacionX < quitarHabitacionX)
            {
                HabitacionesAEliminar.Add(habitacion);
            }
            // Este es el punto más a la derecha. Se usa sólo si necesitas añadir una pantalla
            extremoMasLejanoPantallaX = Mathf.Max(extremoMasLejanoPantallaX, finHabitacionX);
        }

        // Esto quita habitaciones que están marcadas para ser quitadas
        foreach (var pantalla in HabitacionesAEliminar)
        {
            habitacionesActuales.Remove(pantalla);
            Destroy(pantalla);
        }

        // Si en este punto agregarHabitaciones es todavía true entonces el final del nivel está cerca.
        // agregarHabitaciones será true si no se encuentra una habitación que comience más lejos que el ancho de la habitación.
        // Esto indica que es necesario añadir una nueva
        if (necesitoAgregarHabitaciones)
        {
            AgregarHabitacion(extremoMasLejanoPantallaX);
        }
    }


    // El bucle while asegurará que GeneraHabitacionesSiNecesario se ejecuta tantas veces como se necesite
    // mientras el juego esté corriendo y el GameObject esté activo.
    // Se utiliza una sentencia yield para añadir una pausa de 0,25 segundos en la ejecución entre cada iteración del bucle
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
