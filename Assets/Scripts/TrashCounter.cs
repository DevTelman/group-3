using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TrashCounter : MonoBehaviour
{
    public static int Money;
    public static float Energy;
    public static float EarthCleanliness;

    [Header("Settings")]
    public int totalTrash = 0;
    public int maxTrash = 10;
    public Dictionary<TrashType, int> inventory = new Dictionary<TrashType, int>();

    [Header("UI Elements (Main HUD)")]
    public TextMeshProUGUI counterText;
    public TextMeshProUGUI moneyDisplay;
    public TextMeshProUGUI energyDisplay;

    [Header("Recycle Menu Info")]
    public GameObject recyclingMenuCanvas;
    [Tooltip("Այս մեկ տեքստի մեջ կլինի ամբողջ ինֆորմացիան")]
    public TextMeshProUGUI infoSummaryText;

    [Header("Eco Settings")]
    public Image energyFillImage;
    public TextMeshProUGUI ecoPercentText;

    void Start()
    {
        Money = PlayerPrefs.GetInt("Money", 0);
        Energy = PlayerPrefs.GetFloat("Energy", 0);
        EarthCleanliness = PlayerPrefs.GetFloat("EarthCleanliness", 100f);

        ResetInventory();
        UpdateUI();

        if (recyclingMenuCanvas != null)
            recyclingMenuCanvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trash"))
        {
            TrashItemData data = other.GetComponent<TrashItemData>();
            if (data != null)
            {
                if (!inventory.ContainsKey(data.type))
                    inventory[data.type] = 0;

                inventory[data.type]++;
                totalTrash++;
                UpdateUI();
                Destroy(other.gameObject);
                if (totalTrash >= maxTrash)
                    OpenMenu();
            }
        }

        if (other.CompareTag("ToxicTrash"))
        {
            Energy -= 100f;
            EarthCleanliness += 3f;
            UpdateUI();
            Destroy(other.gameObject);
        }
    }

    void OpenMenu()
    {
        if (recyclingMenuCanvas != null)
        {
            recyclingMenuCanvas.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Կազմում ենք ամբողջ ինֆորմացիան մեկ տեքստի մեջ
            if (infoSummaryText != null)
            {
                int pMoney = CalculatePotentialMoney();
                float pEnergy = totalTrash * 8f;

                infoSummaryText.text = $"<b>COLLECTED TRASH:</b>\n" +
                                      $"Metal: {inventory[TrashType.Metal]}\n" +
                                      $"Paper: {inventory[TrashType.Paper]}\n" +
                                      $"Glass: {inventory[TrashType.Glass]}\n" +
                                      $"Plastic: {inventory[TrashType.Plastic]}\n\n" +
                                      $"<b>IF YOU RECYCLE:</b>\n" +
                                      $"Potential Money: +${pMoney}\n" +
                                      $"Potential Energy: +{pEnergy} E";
            }
        }
    }

    public void RecycleForEnergy()
    {
        int mTotal = CalculatePotentialMoney();
        Money += Mathf.RoundToInt(mTotal * 0.3f);
        Energy += totalTrash * 8f;
        EarthCleanliness -= 2f;

        SaveAndClose();
    }

    public void RecycleForMoney()
    {
        Money += CalculatePotentialMoney();
        EarthCleanliness += 1f;

        SaveAndClose();
    }

    void SaveAndClose()
    {
        EarthCleanliness = Mathf.Clamp(EarthCleanliness, 0f, 100f);

        PlayerPrefs.SetInt("Money", Money);
        PlayerPrefs.SetFloat("Energy", Energy);
        PlayerPrefs.SetFloat("EarthCleanliness", EarthCleanliness);
        PlayerPrefs.Save();

        totalTrash = 0;
        ResetInventory();

        if (recyclingMenuCanvas != null)
            recyclingMenuCanvas.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        UpdateUI();
        SceneManager.LoadScene(2);
    }

    public void UpdateUI()
    {
        if (counterText != null)
            counterText.text = $"Trash: {totalTrash}/{maxTrash}";
        if (moneyDisplay != null)
            moneyDisplay.text = $"Money: {Money}";
        if (energyDisplay != null)
            energyDisplay.text = $"Energy: {Energy:F0}";
        if (ecoPercentText != null)
            ecoPercentText.text = EarthCleanliness.ToString("F0") + "%";

        if (energyFillImage != null)
        {
            energyFillImage.fillAmount = (100f + EarthCleanliness) / 100f;
        }
    }

    void ResetInventory()
    {
        inventory.Clear();
        inventory[TrashType.Metal] = 0;
        inventory[TrashType.Paper] = 0;
        inventory[TrashType.Glass] = 0;
        inventory[TrashType.Plastic] = 0;
    }

    int CalculatePotentialMoney()
    {
        int total = 0;
        if (inventory.ContainsKey(TrashType.Metal)) total += inventory[TrashType.Metal] * 25;
        if (inventory.ContainsKey(TrashType.Paper)) total += inventory[TrashType.Paper] * 20;
        if (inventory.ContainsKey(TrashType.Glass)) total += inventory[TrashType.Glass] * 25;
        if (inventory.ContainsKey(TrashType.Plastic)) total += inventory[TrashType.Plastic] * 15;
        return total;
    }
}