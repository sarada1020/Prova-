using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMov : MonoBehaviour
{
    public Animator controlador;
    private Rigidbody rb;
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 700.0f;
    public GameObject bola;  // Refer�ncia ao prefab da bola
    public Vector3 spawnAreaMin; // Posi��o m�nima (inferior) da �rea de spawn
    public Vector3 spawnAreaMax; // Posi��o m�xima (superior) da �rea de spawn

    void Start()
    {
        rb = GetComponent<Rigidbody>();  // Obt�m o Rigidbody para controlar a f�sica
        controlador = GetComponent<Animator>();  // Obt�m o Animator para controlar anima��es
    }

    void Update()
    {
        // Captura a entrada horizontal (A/D ou as setas)
        float moveHorizontal = Input.GetAxis("Horizontal");
        // Captura a entrada vertical (W/S ou as setas)
        float moveVertical = Input.GetAxis("Vertical");

        // Cria um vetor de movimento com base na entrada
        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical).normalized * moveSpeed * Time.deltaTime;

        // Aplica o movimento no Rigidbody para mover o personagem
        rb.MovePosition(transform.position + movement);

        // Rotaciona o personagem se houver movimento
        if (movement.magnitude > 0)
        {
            // Cria uma rota��o para o personagem com base na dire��o do movimento
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            // Faz a rota��o do personagem de forma suave
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotateSpeed * Time.deltaTime);
        }

        // Controle de anima��es
        if (movement.magnitude > 0)
        {
            controlador.SetTrigger("Anda");  // Ativa a anima��o de "anda" se o personagem estiver se movendo
        }
        else
        {
            controlador.SetTrigger("Parado");  // Ativa a anima��o de "parado" se o personagem estiver parado
        }
    }

// Esse m�todo ser� chamado quando o player colidir com a bola
void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Diamante"))
        {
            Destroy(other.gameObject);  // Destr�i a bola coletada
            Spawnar();  // Gera uma nova bola em uma posi��o aleat�ria dentro da �rea
        }
    }

    // Fun��o para spawnar uma nova bola em uma �rea aleat�ria
    public void Spawnar()
    {
        float spawnX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float spawnY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        float spawnZ = Random.Range(spawnAreaMin.z, spawnAreaMax.z);

        // Cria a bola em uma posi��o aleat�ria dentro do intervalo definido
        Vector3 spawnPos = new Vector3(spawnX, spawnY, spawnZ);
        Instantiate(bola, spawnPos, Quaternion.identity);
    }
}

