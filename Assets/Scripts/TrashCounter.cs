using TMPro;
using UnityEngine;

public class TrashCounter : MonoBehaviour
{
    [Header("Настройки UI")]
    public TextMeshProUGUI counterText;
    public GameObject recyclingMenu; // Ссылка на твое меню переработки (Panel)

    [Header("Настройки баланса")]
    public int maxTrash = 10; // Сколько нужно собрать

    [Header("Звук")]
    public AudioClip trashCollectSound;
    private AudioSource audioSource;

    private int trashAmount = 0;

    void Start()
    {
        // Инициализируем звук
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Прячем меню переработки в начале игры
        if (recyclingMenu != null) recyclingMenu.SetActive(false);

        UpdateText(); // Устанавливаем начальное значение 0/10
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trash"))
        {
            trashAmount++;
            UpdateText();

            // Звук
            if (trashCollectSound != null) audioSource.PlayOneShot(trashCollectSound);

            Destroy(other.gameObject);
            Debug.Log("Мусор собран! Всего: " + trashAmount);

            // ПРОВЕРКА: Собрали ли мы 10 штук?
            if (trashAmount >= maxTrash)
            {
                OpenRecyclingMenu();
            }
        }
    }

    void UpdateText()
    {
        if (counterText != null)
        {
            // Формат 0 / 10
            counterText.text = "Trash: " + trashAmount.ToString() + " / " + maxTrash.ToString();
        }
    }

    void OpenRecyclingMenu()
    {
        if (recyclingMenu != null)
        {
            recyclingMenu.SetActive(true); // Показываем меню
            Time.timeScale = 0f;          // Останавливаем игру (пауза)

            // Включаем курсор, чтобы нажать на кнопки в меню
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    // Функция для кнопки "Переработать" (или продолжить игру)
    public void ProcessRecycling()
    {
        Debug.Log("Мусор переработан!");

        // 1. Скрываем меню
        if (recyclingMenu != null) recyclingMenu.SetActive(false);

        // 2. Запускаем время (снимаем паузу)
        Time.timeScale = 1f;

        // 3. Обнуляем счетчик для нового круга
        trashAmount = 0;
        UpdateText();

        // 4. Прячем курсор обратно
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Функция для кнопки "Выход в главное меню"
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Обязательно возвращаем время перед сменой сцены!
        UnityEngine.SceneManagement.SceneManager.LoadScene(0); // 0 - индекс сцены меню
    }
}