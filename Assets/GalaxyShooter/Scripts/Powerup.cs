using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float speed = 3.0f;

    // 0 = Tiro triplo, 1 = Velocidade, 2 = Escudo (futuro)
    [SerializeField]
    private int powerUpId;

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        // Destrói o power-up se sair da tela sem ser coletado
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                switch (powerUpId)
                {
                    case 0:
                        player.TripleShotPowerOn();
                        break;
                    case 1:
                        player.SpeedBoostOn();
                        break;
                    case 2:
                        // Escudo: ainda será implementado
                        break;
                }
            }

            Destroy(gameObject);
        }
    }
}