using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrileteMovement : MonoBehaviour
{
    public Transform[] elements;
    public Transform[] transitionElements;
    
    public Camera mainCamera;
    public float transitionDuration = 10.0f; // Duracion de la transicion en segundos

    private int contadordeClicks = 0;
    public int clicks = 10;
    private bool subiendo = false;
    private Vector2[] posicionInicial;
    private Vector3 cameraInitialPosition;
    

    public float velHorizontal = 1.0f; // Velocidad horizontal de los elementos
    public float ampHorizontal = 0.5f; // Amplitud del movimiento horizontal

    public float minSpeed = 1.0f;
    public float maxSpeed = 5.0f;

    private float[] verticalOffsets; // Nueva variable para almacenar los offsets verticales individuales
    private float[] horizontalOffsets; // Nueva variable para almacenar los offsets horizontales individuales
    public float verticalAmplitude = 1.0f; // Amplitud del movimiento vertical

    //Sonido
    private AudioSource SonidoArpa;


    private void Start()
    {
        posicionInicial = new Vector2[elements.Length];
        horizontalOffsets = new float[transitionElements.Length]; // Inicializar la variable horizontalOffsets


        for (int i = 0; i < elements.Length; i++)
        {
            posicionInicial[i] = elements[i].position;
            transitionElements[i].gameObject.SetActive(false);
        }

        cameraInitialPosition = mainCamera.transform.position;

        // Inicializar el arreglo verticalOffsets con valores aleatorios
        verticalOffsets = new float[transitionElements.Length];
        for (int i = 0; i < transitionElements.Length; i++)
        {
            verticalOffsets[i] = Random.Range(0f, 2f * Mathf.PI);
            // Inicializar la variable horizontalOffsets con valores aleatorios
            horizontalOffsets[i] = Random.Range(0f, 2f * Mathf.PI);
        }

        // Declaración de sonido
        SonidoArpa = GetComponent <AudioSource>();
    }

    private void Update()
    {
        if (subiendo)
        {
            MoveCameraUp();
            Volar();
        }
    }

    private void OnMouseDown()
    {
        contadordeClicks++;

        if (contadordeClicks <= clicks)
        {
            MovHorizontal();
        }
        else
        {
            comenzarTransicion();
        }
    }

    private void comenzarTransicion()
    {
        subiendo = true;

        SonidoArpa.Play();

        // Desactivar elementos originales y activar elementos de transicion
        for (int i = 0; i < elements.Length; i++)
        {
            elements[i].gameObject.SetActive(false);
            transitionElements[i].gameObject.SetActive(true);
            transitionElements[i].position = elements[i].position;
        }

        Invoke("finTransicion", transitionDuration);
    }

    private void finTransicion()
    {
        subiendo = false;
        ResetElements();
        contadordeClicks = 0;
    }

    private void MovHorizontal()
    {
        for (int i = 0; i < elements.Length; i++)
        {
            elements[i].position = posicionInicial[i];
            Vector2 targetPosition = elements[i].position;
            targetPosition.x += Random.Range(-0.3f, 0.3f); // Movimiento aleatorio en el eje X
            elements[i].position = targetPosition;
        }
    }

    //---- FUNCION VOLAR ORIGINAL
    /* private void Volar()
    {
        for (int i = 0; i < transitionElements.Length; i++)
        {
            Vector2 targetPosition = transitionElements[i].position;
            targetPosition.y += 1f * Time.deltaTime; // Movimiento hacia arriba

            // Movimiento horizontal utilizando una funcion senoidal
            float xOffset = Mathf.Sin(Time.time * velHorizontal) * ampHorizontal;
            targetPosition.x += xOffset * Time.deltaTime;

            Vector3 viewportPosition = mainCamera.WorldToViewportPoint(targetPosition); // Convertir a coordenadas de ventana de vista

            // Limitar los movimientos en el eje X dentro del rango de la pantalla
            viewportPosition.x = Mathf.Clamp01(viewportPosition.x);

            targetPosition = mainCamera.ViewportToWorldPoint(viewportPosition); // Convertir de vuelta a coordenadas del mundo

            transitionElements[i].position = targetPosition;
        }
    } */

    //---- FUNCION VOLAR MODIFICADA
    private void Volar()
    {
        for (int i = 0; i < transitionElements.Length; i++)
        {
            Vector2 targetPosition = transitionElements[i].position;

            // Movimiento vertical hacia arriba utilizando una función senoidal con velocidad individual y amplitud controlables
            float verticalSpeed = Random.Range(minSpeed, maxSpeed);
            float yOffset = Mathf.Sin(Time.time * verticalSpeed + verticalOffsets[i]) * verticalAmplitude;
            targetPosition.y += yOffset * Time.deltaTime;

            // Movimiento horizontal utilizando una función senoidal con velocidad y amplitud controlables
            float xOffset = Mathf.Sin(Time.time * velHorizontal + horizontalOffsets[i]) * ampHorizontal;
            targetPosition.x += xOffset * Time.deltaTime;

            // Restringir el movimiento horizontal dentro del rango de la pantalla
            Vector3 viewportPosition = mainCamera.WorldToViewportPoint(targetPosition);
            viewportPosition.x = Mathf.Clamp01(viewportPosition.x);
            targetPosition = mainCamera.ViewportToWorldPoint(viewportPosition);

            transitionElements[i].position = targetPosition;
        }
    }

    
    private void MoveCameraUp()
    {
        Vector3 targetPosition = mainCamera.transform.position;
        targetPosition.y += 1.0f * Time.deltaTime; // Movimiento de la camara hacia arriba a una velocidad constante
        mainCamera.transform.position = targetPosition;
    } 

    private void ResetElements()
    {

        for (int i = 0; i < elements.Length; i++)
        {
            elements[i].gameObject.SetActive(true);
            transitionElements[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < elements.Length; i++)
        {
            elements[i].position = posicionInicial[i];
        }

        mainCamera.transform.position = cameraInitialPosition;
    }
}