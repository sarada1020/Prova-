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

    // �udio
    public AudioClip coletaAudio;  // Som de coleta da bola
    public AudioClip musicaFundo;  // M�sica de fundo
    private AudioSource audioSource;  // Fonte de �udio para efeitos (coleta)
    private AudioSource musicaFundoSource;  // Fonte de �udio para a m�sica de fundo

    private bool coletaAudioTocando = false;  // Flag para garantir que o som de coleta n�o sobreponha

    void Start()
    {
        rb = GetComponent<Rigidbody>();  // Obt�m o Rigidbody para controlar a f�sica
        controlador = GetComponent<Animator>();  // Obt�m o Animator para controlar anima��es

        // Inicializando o AudioSource para os efeitos (coleta)
        audioSource = GetComponent<AudioSource>();  // Fonte de �udio para efeitos (coleta)
        musicaFundoSource = gameObject.AddComponent<AudioSource>();  // Fonte de �udio para a m�sica de fundo

        // Tocar m�sica de fundo continuamente
        musicaFundoSource.clip = musicaFundo;
        musicaFundoSource.loop = true;  // Faz a m�sica de fundo tocar em loop
        musicaFundoSource.Play();  // Inicia a m�sica de fundo
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

            controlador.SetTrigger("Anda");  // Ativa a anima��o de "anda" se o personagem estiver se movendo
        }
        else
        {
            controlador.SetTrigger("Parado");  // Ativa a anima��o de "parado"
        }
    }

    // Esse m�todo ser� chamado quando o player colidir com a bola
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Diamante"))
        {
            Destroy(other.gameObject);  // Destr�i a bola coletada
            Spawnar();  // Gera uma nova bola em uma posi��o aleat�ria dentro da �rea

            // Toca o som da coleta da bola
            if (!coletaAudioTocando)  // Verifica se o som de coleta j� est� tocando
            {
                audioSource.PlayOneShot(coletaAudio);  // Toca o �udio de coleta
                coletaAudioTocando = true;
                StartCoroutine(ResetarColetaAudio());  // Reseta a flag depois que o som terminar
            }
        }
    }

    // Fun��o para resetar a flag de som de coleta ap�s o som terminar
    private IEnumerator ResetarColetaAudio()
    {
        yield return new WaitForSeconds(coletaAudio.length);  // Espera o tempo do �udio
        coletaAudioTocando = false;  // Permite que o som de coleta seja tocado novamente
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
