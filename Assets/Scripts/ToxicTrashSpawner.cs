using UnityEngine;

public class ToxicPointSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject toxicTrashPrefab; // Այստեղ գցիր տոկսիկ աղբի Prefab-ը

    void Start()
    {
        SpawnHere();
    }

    void SpawnHere()
    {
        if (toxicTrashPrefab != null)
        {
            // Ստեղծում է աղբը հենց այս Empty Object-ի դիրքում
            Instantiate(toxicTrashPrefab, transform.position, transform.rotation);
        }
        else
        {
            Debug.LogError("Toxic Trash Prefab-ը կցված չէ " + gameObject.name + "-ի վրա:");
        }
    }
}