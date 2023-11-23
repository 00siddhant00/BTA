using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public UnityEngine.UI.Image fadeImg;
    public float fadeDuration;
    public Color fadeColor;
    public float waitForFadeOut;

    public void Fade()
    {
        fadeImg.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
        StartCoroutine(FadeInAndOut());
    }

    private IEnumerator FadeInAndOut()
    {
        // Fade In
        float elapsedTime = 0f;
        Color startColor = new Color(fadeImg.color.r, fadeImg.color.g, fadeImg.color.b, 0f);
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f);

        while (elapsedTime < fadeDuration)
        {
            fadeImg.color = Color.Lerp(startColor, targetColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeImg.color = targetColor;

        // Wait for a specified duration
        yield return new WaitForSeconds(waitForFadeOut);

        // Fade Out
        elapsedTime = 0f;
        startColor = fadeImg.color;
        targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsedTime < fadeDuration)
        {
            fadeImg.color = Color.Lerp(startColor, targetColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeImg.color = targetColor;
    }

    void Start()
    {
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    /// <summary>
    /// Sets the confinier boundry to current active section
    /// </summary>
    public void ChangeSectionConfinier(int activeSectionId)
    {
        GameManager.Instance.CameraShake.Confiner.m_BoundingShape2D = GameManager.Instance.AreaData.transform.GetChild(activeSectionId).GetComponent<SectionData>().confinerBoundry;
    }


    /// <summary>
    /// Gives the next gate's position from input Gate Id
    /// </summary>
    /// <param name="id">Go To Gate Id</param>
    /// <param name="direction">Spawn Direction from the gate</param>
    /// <returns>Returns Vector3 position of given gate</returns>
    public Vector3 GetGatePoseFromID(Vector2Int id, Gate.SpawnDirection direction)
    {
        SectionData data = GameManager.Instance.AreaData.transform.GetChild(id.x).GetComponent<SectionData>();

        Vector3 offset = new Vector3(direction == Gate.SpawnDirection.left ? (GameManager.Instance.gateSpawnDistance * -1) : GameManager.Instance.gateSpawnDistance, 0, 0);

        foreach (var gate in data.Gates)
        {
            if (gate.GetComponent<Gate>().gateId == id)
                return gate.transform.position + offset;
        }

        print($"{id.x}-{id.y} Gate Not found");
        return Vector3.zero;
    }


    #region Scene System Functions

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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    #endregion
}