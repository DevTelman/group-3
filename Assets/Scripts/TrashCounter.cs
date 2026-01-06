using TMPro;
using UnityEngine;

public class TrashCounter : MonoBehaviour
{
   
    [Header("Настройки UI")]
    public TextMeshProUGUI counterText; 

    private int trashAmount = 0; 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trash"))
        {
            trashAmount++;

            
            if (counterText != null)
            {
                counterText.text = "Мусор "+trashAmount.ToString();
            }

            Destroy(other.gameObject);

            Debug.Log("Мусор собран! Всего: " + trashAmount);
        }
    }
}