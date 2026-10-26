using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float speed = 3.0f;

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        if (transform.position.y >= 6)
        {
            // Se for do tiro triplo, destrói o pai (e os 3 filhos junto)
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(gameObject);
        }
    }
}