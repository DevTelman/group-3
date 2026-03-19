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

    // Փոփոխական՝ ընթացիկ ծախսը հիշելու համար
    private int currentUpgradeCost;

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
    public TextMeshProUGUI infoSummaryText;

    [Header("Upgrade/Cleaning Menu")]
    public GameObject upgradeMenuCanvas;
    public TextMeshProUGUI upgradeInfoText;
    public Button payMoneyButton;
    public Button payEnergyButton;

    [Header("Eco Settings")]
    public Image energyFillImage;
    public TextMeshProUGUI ecoPercentText;

    void Start()
    {
        Money = PlayerPrefs.GetInt("Money", 0);
        Energy = PlayerPrefs.GetFloat("Energy", 0);

        // Աղտոտվածությունը սկսվում է 100%-ից
        EarthCleanliness = PlayerPrefs.GetFloat("EarthCleanliness", 100f);

        // Վերցնում ենք մաքրման արժեքը (եթե առաջին անգամ է՝ 50)
        currentUpgradeCost = PlayerPrefs.GetInt("UpgradeCost", 50);

        ResetInventory();
        UpdateUI();

        if (recyclingMenuCanvas != null) recyclingMenuCanvas.SetActive(false);
        if (upgradeMenuCanvas != null) upgradeMenuCanvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Սովորական աղբի հավաքում
        if (other.CompareTag("Trash"))
        {
            TrashItemData data = other.GetComponent<TrashItemData>();
            if (data != null)
            {
                if (!inventory.ContainsKey(data.type)) inventory[data.type] = 0;
                inventory[data.type]++;
                totalTrash++;
                UpdateUI();
                Destroy(other.gameObject);
                if (totalTrash >= maxTrash) OpenMenu();
            }
        }

        // ՏՈՔՍԻԿ ԱՂԲ. Միայն աղտոտում է երկիրը 3%-ով
        if (other.CompareTag("ToxicTrash"))
        {
            // Ավելացնում ենք աղտոտվածությունը 3%-ով (քանի որ 100-ը առավելագույն աղտոտվածությունն է)
            EarthCleanliness = Mathf.Clamp(EarthCleanliness + 3f, 0f, 100f);

            UpdateUI(); // Թարմացնում ենք էկրանի տոկոսը և գիծը
            Destroy(other.gameObject); // Ջնջում ենք տոքսիկ աղբը
        }
    }

    void OpenMenu()
    {
        recyclingMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (infoSummaryText != null)
        {
            int pMoney = CalculatePotentialMoney();
            infoSummaryText.text = $"<b>COLLECTED TRASH:</b>\n" +
                                   $"Metal: {inventory[TrashType.Metal]}, Plastic: {inventory[TrashType.Plastic]}\n\n" +
                                   $"<b>REWARD:</b>\nMoney: +${pMoney} | Energy: +{totalTrash * 8} E";
        }
    }

    public void RecycleForEnergy()
    {
        Money += Mathf.RoundToInt(CalculatePotentialMoney() * 0.3f);
        Energy += totalTrash * 8f;
        // Վերամշակելիս աղտոտվածությունը մի փոքր ավելանում է (եթե 100-ից ցածր է)
        EarthCleanliness = Mathf.Clamp(EarthCleanliness + 2f, 0f, 100f);
        ShowUpgradeMenu();
    }

    public void RecycleForMoney()
    {
        Money += CalculatePotentialMoney();
        // Փողի համար վերամշակելիս աղտոտվածությունը իջնում է 1%-ով
        EarthCleanliness = Mathf.Clamp(EarthCleanliness - 1f, 0f, 100f);
        ShowUpgradeMenu();
    }

    void ShowUpgradeMenu()
    {
        recyclingMenuCanvas.SetActive(false);
        upgradeMenuCanvas.SetActive(true);

        if (upgradeInfoText != null)
            upgradeInfoText.text = $"Spend {currentUpgradeCost} Money or {currentUpgradeCost} Energy to clean 5%?";

        if (payMoneyButton != null) payMoneyButton.interactable = (Money >= currentUpgradeCost);
        if (payEnergyButton != null) payEnergyButton.interactable = (Energy >= currentUpgradeCost);
    }

    public void PayUpgradeMoney()
    {
        if (Money >= currentUpgradeCost)
        {
            Money -= currentUpgradeCost;
            ApplyCleaningEffect();
        }
    }

    public void PayUpgradeEnergy()
    {
        if (Energy >= currentUpgradeCost)
        {
            Energy -= currentUpgradeCost;
            ApplyCleaningEffect();
        }
    }

    void ApplyCleaningEffect()
    {
        // Աղտոտվածությունը իջեցնում ենք 5%-ով
        EarthCleanliness -= 90f;
        EarthCleanliness -= 90f;
        if (EarthCleanliness < 0) EarthCleanliness = 0;

        // Հաջորդ անգամվա համար ծախսը ավելացնում ենք 50%-ով (x 1.5)
        currentUpgradeCost = Mathf.RoundToInt(currentUpgradeCost * 1.5f);

        PlayerPrefs.SetInt("UpgradeCost", currentUpgradeCost);

        SaveAndNextLevel();
    }

    public void SkipUpgrade()
    {
        SaveAndNextLevel();
    }

    void SaveAndNextLevel()
    {
        PlayerPrefs.SetInt("Money", Money);
        PlayerPrefs.SetFloat("Energy", Energy);
        PlayerPrefs.SetFloat("EarthCleanliness", EarthCleanliness);
        PlayerPrefs.Save();

        Time.timeScale = 1f;

        if (EarthCleanliness <= 0f)
        {
            SceneManager.LoadScene("WinScene");
        }
        else
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                SceneManager.LoadScene(1);
            }
        }
    }

    public void UpdateUI()
    {
        if (counterText != null) counterText.text = $"Trash: {totalTrash}/{maxTrash}";
        if (moneyDisplay != null) moneyDisplay.text = $"Money: {Money}";
        if (energyDisplay != null) energyDisplay.text = $"Energy: {Energy:F0}";
        if (ecoPercentText != null) ecoPercentText.text = EarthCleanliness.ToString("F0") + "%";

        if (energyFillImage != null)
            energyFillImage.fillAmount = EarthCleanliness / 100f;
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
    