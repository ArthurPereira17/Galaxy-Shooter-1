using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public bool canTripleShot = false;
    public bool isSpeedBoostActive = false;

    [SerializeField]
    private GameObject laserPrefab;
    [SerializeField]
    private GameObject tripleShotPrefab;

    [SerializeField]
    private float fireRate = 0.25f;
    private float canFire = 0.0f;

    [SerializeField]
    private float tripleShotDuration = 5.0f;
    private Coroutine tripleShotRoutine;

    [SerializeField]
    private float speedBoostDuration = 5.0f;
    [SerializeField]
    private float speedMultiplier = 2.0f;
    private Coroutine speedBoostRoutine;

    public float speed = 5.0f;

    private void Start()
    {
        ResetPosition();
    }

    private void Update()
    {
        Move();
        LimitBounds();
        HandleShootInput();
    }

    // Posição inicial da nave
    private void ResetPosition()
    {
        transform.position = new Vector3(0, 0, 0);
    }

    // Lê espaço (um clique por tiro) ou botão esquerdo do mouse (segurando)
    private void HandleShootInput()
    {
        var kb = Keyboard.current;
        var mouse = Mouse.current;

        bool pressedSpace = kb != null && kb.spaceKey.wasPressedThisFrame;
        bool pressedMouse = mouse != null && mouse.leftButton.isPressed;

        if (pressedSpace || pressedMouse)
        {
            Shoot();
        }
    }

    // Atira respeitando o fireRate
    private void Shoot()
    {
        if (Time.time > canFire)
        {
            Vector3 spawnPos = transform.position + new Vector3(0, 0.88f, 0);

            if (canTripleShot)
            {
                Instantiate(tripleShotPrefab, spawnPos, Quaternion.identity);
            }
            else
            {
                Instantiate(laserPrefab, spawnPos, Quaternion.identity);
            }

            canFire = Time.time + fireRate;
        }
    }

    // Lê o teclado e devolve a direção (-1, 0 ou 1 em cada eixo)
    private Vector2 GetInput()
    {
        Vector2 move = Vector2.zero;
        var kb = Keyboard.current;

        if (kb != null)
        {
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) move.x -= 1;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) move.x += 1;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed) move.y -= 1;
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed) move.y += 1;
        }

        return move;
    }

    // Move a nave; com o boost ativo, a velocidade é multiplicada
    private void Move()
    {
        Vector2 move = GetInput();

        float currentSpeed = speed;
        if (isSpeedBoostActive)
        {
            currentSpeed = speed * speedMultiplier;
        }

        transform.Translate(new Vector3(move.x, move.y, 0) * currentSpeed * Time.deltaTime);
    }

    // Aplica os limites do mapa
    private void LimitBounds()
    {
        LimitVertical();
        WrapHorizontal();
    }

    // Trava a nave entre -4.2 e 0 no eixo Y
    private void LimitVertical()
    {
        if (transform.position.y > 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        else if (transform.position.y < -4.2f)
        {
            transform.position = new Vector3(transform.position.x, -4.2f, 0);
        }
    }

    // Atravessa a tela: sai de um lado e entra no outro no eixo X
    private void WrapHorizontal()
    {
        if (transform.position.x > 9.5f)
        {
            transform.position = new Vector3(-9.5f, transform.position.y, 0);
        }
        else if (transform.position.x < -9.5f)
        {
            transform.position = new Vector3(9.5f, transform.position.y, 0);
        }
    }

    // Chamado pelo Powerup quando o jogador coleta o tiro triplo
    public void TripleShotPowerOn()
    {
        canTripleShot = true;

        if (tripleShotRoutine != null)
        {
            StopCoroutine(tripleShotRoutine);
        }
        tripleShotRoutine = StartCoroutine(TripleShotPowerDownRoutine());
    }

    private IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(tripleShotDuration);
        canTripleShot = false;
        tripleShotRoutine = null;
    }

    // Chamado pelo Powerup quando o jogador coleta o boost de velocidade
    public void SpeedBoostOn()
    {
        isSpeedBoostActive = true;

        // Se já havia um boost ativo, reinicia o tempo
        if (speedBoostRoutine != null)
        {
            StopCoroutine(speedBoostRoutine);
        }
        speedBoostRoutine = StartCoroutine(SpeedBoostDownRoutine());
    }

    private IEnumerator SpeedBoostDownRoutine()
    {
        yield return new WaitForSeconds(speedBoostDuration);
        isSpeedBoostActive = false;
        speedBoostRoutine = null;
    }
}