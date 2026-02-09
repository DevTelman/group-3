using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    [Header("��������� ������")]
    [SerializeField] private GameObject[] trashPrefabs; // ������ ������ ����� ������
    [SerializeField] private int trashCount = 20; // ���������� ������ ��� ������
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(50f, 50f); // ������ ������� ������
    [SerializeField] private float spawnHeight = 100f; // ������ ��� ��������
    [SerializeField] private LayerMask groundLayer; // ���� �����

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
                // �������� ��������� ������ �� �������
                GameObject randomTrash = trashPrefabs[Random.Range(0, trashPrefabs.Length)];

                // ������� �����
                GameObject trash = Instantiate(randomTrash, randomPos, Quaternion.Euler(0, Random.Range(0f, 360f), 0));

                // ��������� ������� ��� ������������
                float randomScale = Random.Range(0.8f, 1.2f);
                trash.transform.localScale *= randomScale;

                // ��������� ������ ���� � ���
                if (trash.GetComponent<Rigidbody>() == null)
                {
                    Rigidbody rb = trash.AddComponent<Rigidbody>();
                    rb.mass = 0.5f;
                    rb.linearDamping = 1f;
                    rb.useGravity = true;
                }

                // ��������� ��������� ���� ��� ���
                if (trash.GetComponent<Collider>() == null)
                {
                    trash.AddComponent<BoxCollider>();
                }
            }
        }
    }

    Vector3 GetRandomGroundPosition()
    {
        // ���������� ��������� ������� � ������� ������
        float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float randomZ = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);

        Vector3 rayStart = transform.position + new Vector3(randomX, spawnHeight, randomZ);

        // ������� ��� ���� ����� ����� �����
        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, spawnHeight * 2, groundLayer))
        {
            // ��������� ��� Ground
            if (hit.collider.CompareTag("Ground"))
            {
                return hit.point;
            }
        }

        return Vector3.zero;
    }

    // ������������ ������� ������ � ���������
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnAreaSize.x, 0.1f, spawnAreaSize.y));
    }
}