using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    [Header("Настройки спавна")]
    [SerializeField] private GameObject[] trashPrefabs; // Массив разных типов мусора
    [SerializeField] private int trashCount = 20; // Количество мусора для спавна
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(50f, 50f); // Размер области спавна
    [SerializeField] private float spawnHeight = 100f; // Высота для рейкаста
    [SerializeField] private LayerMask groundLayer; // Слой земли

    void Start()
    {
        SpawnTrash();
    }

    void SpawnTrash()
    {
        for (int i = 0; i < trashCount; i++)
        {
            Vector3 randomPos = GetRandomGroundPosition();

            if (randomPos != Vector3.zero)
            {
                // Выбираем случайный префаб из массива
                GameObject randomTrash = trashPrefabs[Random.Range(0, trashPrefabs.Length)];

                // Спавним мусор
                GameObject trash = Instantiate(randomTrash, randomPos, Quaternion.Euler(0, Random.Range(0f, 360f), 0));

                // Случайный масштаб для разнообразия
                float randomScale = Random.Range(0.8f, 1.2f);
                trash.transform.localScale *= randomScale;
            }
        }
    }

    Vector3 GetRandomGroundPosition()
    {
        // Генерируем случайную позицию в области спавна
        float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float randomZ = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);

        Vector3 rayStart = transform.position + new Vector3(randomX, spawnHeight, randomZ);

        // Пускаем луч вниз чтобы найти землю
        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, spawnHeight * 2, groundLayer))
        {
            // Проверяем тег Ground
            if (hit.collider.CompareTag("Ground"))
            {
                return hit.point;
            }
        }

        return Vector3.zero;
    }

    // Визуализация области спавна в редакторе
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnAreaSize.x, 0.1f, spawnAreaSize.y));
    }
}