using UnityEngine;

public enum TrashType { Plastic, Glass, Paper, Metal } // Пластик = пакеты

public class TrashItemData : MonoBehaviour
{
    public TrashType type;
}