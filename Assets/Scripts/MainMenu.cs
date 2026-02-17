using UnityEngine;
using UnityEngine.SceneManagement; // Обязательно для работы со сценами

public class MainMenu : MonoBehaviour
{
    // Эта функция будет запускать игру
    public void PlayGame()
    {
        // Загружаем сцену под номером 1 (твой игровой уровень)
        SceneManager.LoadScene(1);
    }

    // Эта функция будет закрывать игру
    public void ExitGame()
    {
        Debug.Log("Выход из игры..."); // Сообщение для проверки в консоли
        Application.Quit(); // Закрывает приложение
    }
}