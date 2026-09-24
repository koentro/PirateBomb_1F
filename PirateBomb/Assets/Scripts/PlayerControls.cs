using UnityEngine;

public class PlayerControls : MonoBehaviour
{

    [Header("Movimentação horizontal")]
    [SerializeField] private float moveSpeed = 4.0f;
    [SerializeField] private float acceleration = 50.0f;
    [SerializeField] private float deceleration = 50.0f;


    [Header("Configuração do pulo")]
    [SerializeField] private float jumpForce = 12.0f;
    [SerializeField] private float falMultiplier = 2.5f;
    [SerializeField] private float lowMultiplier = 2.0f;


    [Header("Verificação de chão")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groudCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;


    private Rigidbody2D rb;
    private Animator animator;

    private float moveInput;
    private bool isGrounded;
    private bool jumpRequested;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();   
        animator = GetComponent<Animator>();    
    }

    private void Update()
    {
        // Leitura dos eixos de movimento (-1 para esquerda, 1 para direita)
        moveInput = Input.GetAxisRaw("Horizontal");

        // Verifica se o personagem está pisando no chão
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groudCheckRadius, groundLayer);

        // Detecta o pressionar da barra de espaço para executar o pulo
        if (Input.GetButtonDown("Jump") && isGrounded == true)
        {
            jumpRequested = true;   
        }

        // Vira o sprite conforme a direção 
        if(moveInput > 0)
        {
            transform.localScale = new Vector2(1.0f, 1.0f);
        }
        else if(moveInput < 0)
        {
            transform.localScale = new Vector2(-1.0f, 1.0f);
        }

        // Modifica a gravidade para um pulo mais dinâmico (não linear)
        ApplyBetterGravity();


        // Animações
        animator.SetInteger("pMove", (int)moveInput);
        animator.SetBool("pGrounded", isGrounded);

    }

    private void FixedUpdate()
    {
        // Processa o movimento horizontal suave
        float targetSpeed = moveInput * moveSpeed;
        float speedDif = targetSpeed - rb.linearVelocity.x;

        // Ternário (if) para verifica a velocidade e fazer a aceleração e desaceleração
        float acceRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;

        float movement = speedDif * acceRate;
        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);


        // Aplica a força de pulo na taxa de atualização da fisica
        if(jumpRequested == true)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
            jumpRequested = false;  
        }
    }

    private void ApplyBetterGravity()
    {
        // Se estiver caindo, aumenta a gravidade para uma queda mais rápida
        if(rb.linearVelocityY < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (falMultiplier - 1) * Time.deltaTime;
        }
        // Se soltar o botão de pulo no ar, realiza um mais baixo
        else if(rb.linearVelocityY > 0 && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowMultiplier - 1) * Time.deltaTime;
        }
    }


    private void OnDrawGizmosSelected()
    {
        if(groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(groundCheck.position, groudCheckRadius);
        }
    }


}
