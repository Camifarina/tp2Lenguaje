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
    private float[] ampHorizontales; // Amplitudes del movimiento horizontal
    public float maxAmpHor = 2f;

    private float[] horizontalOffsets; // Nueva variable para almacenar los offsets horizontales individuales

    public float rotationSpeed = 1.0f; // Velocidad de rotación 
    private float[] rotationAmplitudes; // Amplitudes de rotación individuales
    public float maxAmpRot = 30f;


    private void Start()
    {
        posicionInicial = new Vector2[elements.Length];
        horizontalOffsets = new float[transitionElements.Length]; // Inicializar la variable horizontalOffsets
        ampHorizontales = new float[transitionElements.Length];
        rotationAmplitudes = new float[transitionElements.Length];

        for (int i = 0; i < elements.Length; i++)
        {
            posicionInicial[i] = elements[i].position;
            transitionElements[i].gameObject.SetActive(false);
        }

        cameraInitialPosition = mainCamera.transform.position;

        for (int i = 0; i < transitionElements.Length; i++)
        {
            // Inicializar la variable horizontalOffsets con valores aleatorios
            horizontalOffsets[i] = Random.Range(0.0f, 30.0f);
            ampHorizontales[i] = Random.Range(-maxAmpHor, maxAmpHor);
            rotationAmplitudes[i] = Random.Range(-maxAmpRot, maxAmpRot);
        }
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

    private void Volar()
    {
        for (int i = 0; i < transitionElements.Length; i++)
        {
            Vector2 targetPosition = transitionElements[i].position;
            targetPosition.y += 1f * Time.deltaTime; // Movimiento hacia arriba

            // Movimiento horizontal utilizando una función senoidal con velocidad y amplitud controlables
            float xOffset = Mathf.Sin(Time.time * velHorizontal + horizontalOffsets[i]) * ampHorizontales[i];
            targetPosition.x += xOffset * Time.deltaTime;

            //Rotación de los barriletes
            float oscillation = Mathf.Sin(Time.time * rotationSpeed) * rotationAmplitudes[i];
            transitionElements[i].rotation = Quaternion.Euler(0f, 0f, oscillation);


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