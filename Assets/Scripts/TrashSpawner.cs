using UnityEngine;
public class TrashSpawner : MonoBehaviour
{
    public GameObject trashPrefab;
    public int trashCount = 50;

    void Start()
    {
        SpawnTrash();
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
                    Instantiate(
                        trashPrefab,
                        hit.point + Vector3.up * 0.1f,
                        Quaternion.identity
                    );
                    spawned++;
                }
            }
        }
    }
}