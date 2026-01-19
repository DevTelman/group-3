using TMPro;
using UnityEngine;

public class TrashCounter : MonoBehaviour
{
    [Header("Настройки UI")]
    public TextMeshProUGUI counterText;

    [Header("Звук")]
    public AudioClip trashCollectSound; // Звук сбора мусора
    private AudioSource audioSource;

    private int trashAmount = 0;

    void Start()
    {
        // Создаем AudioSource если его нет
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Настраиваем AudioSource
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trash"))
        {
            trashAmount++;

            // Обновляем текст
            if (counterText != null)
            {
                counterText.text = "Trash " + trashAmount.ToString();
            }

            // Воспроизводим звук
            if (trashCollectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(trashCollectSound);
            }

            Destroy(other.gameObject);
            Debug.Log("Мусор собран! Всего: " + trashAmount);
        }
    }
}