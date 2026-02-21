using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrashSpawner : MonoBehaviour
{
    [Header("Настройки спавна")]
    public GameObject[] trashPrefabs;    // Массив разных видов мусора
    public Transform[] spawnPoints;     // Точки, где будет появляться мусор
    public float spawnInterval = 3f;    // Пауза между спавном (в секундах)
    public int maxTrashOnScene = 15;    // Максимум мусора на карте одновременно

    private List<GameObject> currentTrash = new List<GameObject>();

    void Start()
    {
        // Запускаем бесконечный цикл спавна
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Ждем указанное время
            yield return new WaitForSeconds(spawnInterval);

            // Очищаем список от удаленного мусора (который игрок уже собрал)
            currentTrash.RemoveAll(item => item == null);

            // Проверяем, не слишком ли много мусора уже навалило
            if (currentTrash.Count < maxTrashOnScene)
            {
                SpawnTrash();
            }
        }
    }

    void SpawnTrash()
    {
        if (spawnPoints.Length == 0 || trashPrefabs.Length == 0) return;

        // Выбираем случайную точку из твоего списка
        Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Выбираем случайный префаб мусора
        GameObject randomPrefab = trashPrefabs[Random.Range(0, trashPrefabs.Length)];

        // Создаем мусор в этой точке
        GameObject spawned = Instantiate(randomPrefab, randomPoint.position, randomPoint.rotation);

        // Добавляем в список, чтобы следить за количеством
        currentTrash.Add(spawned);
    }
}