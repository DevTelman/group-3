using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    public GameObject[] trashPrefabs;
    public int trashCount = 50;

    void Start()
    {
        if (trashPrefabs != null && trashPrefabs.Length > 0)
        {
            SpawnTrash();
        }
        else
        {
            Debug.LogError("Список Trash Prefabs пуст! Перетащите префабы в инспекторе.");
        }
    }

    void SpawnTrash()
    {
        BoxCollider area = GetComponent<BoxCollider>();
        int spawned = 0;
        int attempts = 0;

        while (spawned < trashCount && attempts < trashCount * 10)
        {
            attempts++;

            Vector3 randomPos = new Vector3(
                Random.Range(area.bounds.min.x, area.bounds.max.x),
                area.bounds.max.y,
                Random.Range(area.bounds.min.z, area.bounds.max.z)
            );

            if (Physics.Raycast(randomPos, Vector3.down, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    int randomIndex = Random.Range(0, trashPrefabs.Length);
                    GameObject selectedTrash = trashPrefabs[randomIndex];

                    Instantiate(
                        selectedTrash,
                        hit.point + Vector3.up * 0.1f,
                        Quaternion.identity
                    );
                    spawned++;
                }
            }
        }
    }
}