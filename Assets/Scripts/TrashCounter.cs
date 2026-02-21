using UnityEngine;
using TMPro;
using UnityEngine.UI; // Обязательно добавь это для работы со слайдером
using System.Collections.Generic;

public class TrashCounter : MonoBehaviour
{
    [Header("Ресурсы")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI energyText;

    [Header("Шкала загрязнения")]
    public Slider pollutionSlider; // Перетащи сюда Slider из инспектора
    public float globalPollution = 100f;

    [Header("Текущий сбор")]
    public int totalTrash = 0;
    public int maxTrash = 10;
    public Dictionary<TrashType, int> inventory = new Dictionary<TrashType, int>();

    [Header("UI Меню")]
    public TextMeshProUGUI counterText;
    public TextMeshProUGUI statsText;
    public GameObject recyclingMenu;

    private int money;
    private float energy;

    void Start()
    {
        money = PlayerPrefs.GetInt("Money", 0);
        energy = PlayerPrefs.GetFloat("Energy", 0);
        globalPollution = PlayerPrefs.GetFloat("GlobalPollution", 100f);

        // Инициализация инвентаря
        inventory[TrashType.Metal] = 0;
        inventory[TrashType.Paper] = 0;
        inventory[TrashType.Glass] = 0;
        inventory[TrashType.Plastic] = 0;

        // Настройка слайдера
        if (pollutionSlider != null)
        {
            pollutionSlider.maxValue = 100f;
            pollutionSlider.value = globalPollution;
        }

        UpdateUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trash"))
        {
            TrashItemData data = other.GetComponent<TrashItemData>();
            if (data != null)
            {
                inventory[data.type]++;
                totalTrash++;
                UpdateUI();
                Destroy(other.gameObject);
                if (totalTrash >= maxTrash) OpenMenu();
            }
        }
    }

    void OpenMenu()
    {
        recyclingMenu.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        statsText.text = $"СОБРАНО:\nМеталл: {inventory[TrashType.Metal]}\nБумага: {inventory[TrashType.Paper]}\nСтекло: {inventory[TrashType.Glass]}\nПакеты: {inventory[TrashType.Plastic]}";
    }

    public void RecycleForMoney()
    {
        int reward = (inventory[TrashType.Metal] * 25) + (inventory[TrashType.Paper] * 20) +
                     (inventory[TrashType.Glass] * 25) + (inventory[TrashType.Plastic] * 15);
        money += reward;
        globalPollution -= 5f; // Уменьшаем загрязнение на 5%
        SaveAndClose();
    }

    public void RecycleForEnergy()
    {
        energy += totalTrash * 5;
        globalPollution -= 2f; // Энергия чистит мир меньше
        SaveAndClose();
    }

    void SaveAndClose()
    {
        if (globalPollution < 0) globalPollution = 0;
        PlayerPrefs.SetInt("Money", money);
        PlayerPrefs.SetFloat("Energy", energy);
        PlayerPrefs.SetFloat("GlobalPollution", globalPollution);
        PlayerPrefs.Save();

        totalTrash = 0;
        foreach (TrashType type in System.Enum.GetValues(typeof(TrashType))) inventory[type] = 0;

        recyclingMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UpdateUI();
    }

    void UpdateUI()
    {
        counterText.text = $"Мусор: {totalTrash} / {maxTrash}";
        moneyText.text = $"{money}"; // Пишем только число, так как иконка будет рядом
        energyText.text = $"{energy}";
        if (pollutionSlider != null) pollutionSlider.value = globalPollution;
    }
    public void ResetDataForTesting()
    {
        PlayerPrefs.DeleteAll(); // Стирает всё из памяти
        money = 0;
        energy = 0;
        globalPollution = 100f;
        totalTrash = 0;

        // Обнуляем инвентарь
        inventory[TrashType.Metal] = 0;
        inventory[TrashType.Paper] = 0;
        inventory[TrashType.Glass] = 0;
        inventory[TrashType.Plastic] = 0;

        UpdateUI();
        Debug.Log("Данные полностью обнулены для теста!");
    }
}