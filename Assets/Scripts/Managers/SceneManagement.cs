using UnityEngine.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.TimeZoneInfo;
using TMPro;

public class SceneManagement : MonoBehaviour
{
    public TMP_InputField name;

    public static SceneManagement Instance;
    void Start()
    {
        Time.timeScale = 1f;
        Instance = this;
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextScene()
    {
        PlayerPrefs.SetString("Name", name.text);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}