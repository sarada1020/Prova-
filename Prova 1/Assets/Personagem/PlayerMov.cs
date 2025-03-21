using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMov : MonoBehaviour
{
    public Animator controlador;
    private Rigidbody rb;
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 700.0f;
    public GameObject bola;  // Referência ao prefab da bola
    public Vector3 spawnAreaMin; // Posição mínima (inferior) da área de spawn
    public Vector3 spawnAreaMax; // Posição máxima (superior) da área de spawn

    // Áudio
    public AudioClip coletaAudio;  // Som de coleta da bola
    public AudioClip musicaFundo;  // Música de fundo
    private AudioSource audioSource;  // Fonte de áudio para efeitos (coleta)
    private AudioSource musicaFundoSource;  // Fonte de áudio para a música de fundo

    private bool coletaAudioTocando = false;  // Flag para garantir que o som de coleta não sobreponha

    void Start()
    {
        rb = GetComponent<Rigidbody>();  // Obtém o Rigidbody para controlar a física
        controlador = GetComponent<Animator>();  // Obtém o Animator para controlar animações

        // Inicializando o AudioSource para os efeitos (coleta)
        audioSource = GetComponent<AudioSource>();  // Fonte de áudio para efeitos (coleta)
        musicaFundoSource = gameObject.AddComponent<AudioSource>();  // Fonte de áudio para a música de fundo

        // Tocar música de fundo continuamente
        musicaFundoSource.clip = musicaFundo;
        musicaFundoSource.loop = true;  // Faz a música de fundo tocar em loop
        musicaFundoSource.Play();  // Inicia a música de fundo
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
            // Cria uma rotação para o personagem com base na direção do movimento
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            // Faz a rotação do personagem de forma suave
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotateSpeed * Time.deltaTime);

            controlador.SetTrigger("Anda");  // Ativa a animação de "anda" se o personagem estiver se movendo
        }
        else
        {
            controlador.SetTrigger("Parado");  // Ativa a animação de "parado"
        }
    }

    // Esse método será chamado quando o player colidir com a bola
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Diamante"))
        {
            Destroy(other.gameObject);  // Destrói a bola coletada
            Spawnar();  // Gera uma nova bola em uma posição aleatória dentro da área

            // Toca o som da coleta da bola
            if (!coletaAudioTocando)  // Verifica se o som de coleta já está tocando
            {
                audioSource.PlayOneShot(coletaAudio);  // Toca o áudio de coleta
                coletaAudioTocando = true;
                StartCoroutine(ResetarColetaAudio());  // Reseta a flag depois que o som terminar
            }
        }
    }

    // Função para resetar a flag de som de coleta após o som terminar
    private IEnumerator ResetarColetaAudio()
    {
        yield return new WaitForSeconds(coletaAudio.length);  // Espera o tempo do áudio
        coletaAudioTocando = false;  // Permite que o som de coleta seja tocado novamente
    }

    // Função para spawnar uma nova bola em uma área aleatória
    public void Spawnar()
    {
        float spawnX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float spawnY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        float spawnZ = Random.Range(spawnAreaMin.z, spawnAreaMax.z);

        // Cria a bola em uma posição aleatória dentro do intervalo definido
        Vector3 spawnPos = new Vector3(spawnX, spawnY, spawnZ);
        Instantiate(bola, spawnPos, Quaternion.identity);
    }
}
