using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player")]
    public PlayerController playerController;
    public Pet pet;

    [Header("Utilitis")]
    public ScenesManager SceneManager;
    public EnemyManager enemyManager;
    public CutSceneManager cutSceneManager;
    public CameraShake CameraShake;
    public PuzzleBase PuzzleBase;

    [Header("Level")]
    public AreaData AreaData;
    public float gateSpawnDistance;

    [Header("System")]
    public GameObject Info;
    bool infoToggle;

    private IDataService dataService = new JsonDataService();

    public void SerializeJson(GameData gameData)
    {
        if (dataService.SaveData("player-stats.json", gameData, false))
        {
            Debug.Log("Game data saved successfully");
        }
        else
        {
            Debug.LogError("Game data was not saved");
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        playerController = FindObjectOfType<PlayerController>();
        LoadData();
    }

    void LoadData()
    {
        bool dataExist;
        GameData gameData = new GameData();
        try
        {
            gameData = dataService.LoadData<GameData>("player-stats.json", false);
            dataExist = true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Data Loading Failed because {e.Message}");
            dataExist = false;
        }

        if (!dataExist)
        {
            playerController.transform.position = playerController.playerMovement.spawnPoint.transform.position;
            playerController.petAbality.aquiredPet = false;
            SceneManager.ChangeSectionConfinier(0);
            return;
        }

        foreach (Transform section in AreaData.transform)
        {
            section.gameObject.SetActive(false);
        }

        //Sets Last Section as Enabled (to assign and retrive data as well)
        var _activeSetion = AreaData.transform.GetChild(gameData.LastActiveSection);
        _activeSetion.gameObject.SetActive(true);


        #region Player Data Assignment

        pet.transform.parent = _activeSetion;
        playerController.transform.parent = _activeSetion;
        playerController.transform.SetAsFirstSibling();

        SceneManager.ChangeSectionConfinier(gameData.LastActiveSection);

        _activeSetion.GetComponent<SectionData>().playerInSection = true;
        //Load Player Health
        playerController.playerHealth.playerCurrentHealth = gameData.PlayerHealth != 0 ? gameData.PlayerHealth : 1f;

        //Load Player Position
        float playerPosX = gameData.PlayerPositionX;
        float playerPosY = gameData.PlayerPositionY;

        // Set the player's position to the saved position
        if (playerPosX == 0 && playerPosY == 0)
            playerController.transform.position = playerController.playerMovement.spawnPoint.transform.position;
        else
            playerController.transform.position = new Vector3(playerPosX, playerPosY, 0);

        //Weather Player has yet aquired pet or not
        playerController.petAbality.aquiredPet = gameData.PlayerHasPet;

        #endregion

        //print($"Loading {AreaData.transform.GetChild(gameData.LastActiveSection).GetComponent<SectionData>().name} : {AreaData.transform.GetChild(gameData.LastActiveSection).GetComponent<SectionData>().hasEnemies}");

        //if (AreaData.transform.GetChild(gameData.LastActiveSection).GetComponent<SectionData>().hasEnemies) print("Has Enemies");
        //else if (!AreaData.transform.GetChild(gameData.LastActiveSection).GetComponent<SectionData>().hasEnemies) print($"{gameData.LastActiveSection} Section has no enemies");

        //Set Enemy Data
        //if (AreaData.transform.GetChild(gameData.LastActiveSection).GetComponent<SectionData>().hasEnemies)
        //{
        foreach (var enemy in AreaData.transform.GetChild(gameData.LastActiveSection).GetComponent<SectionData>().Enemies)
        {
            Destroy(enemy);
        }
        //Respawn Enemies with saved stats
        for (int i = 0; i < gameData.EnemiesInActiveSection.Count; i++)
        {
            //GameObject Enemy = Instantiate(enemyManager.GetEnemyFromId(gameData.EnemiesInActiveSection[i].id), AreaData.transform.GetChild(gameData.LastActiveSection).GetComponent<SectionData>().Enemies[0].transform.parent);
            GameObject Enemy = Instantiate(enemyManager.GetEnemyFromId(gameData.EnemiesInActiveSection[i].id), AreaData.transform.GetChild(gameData.LastActiveSection).GetChild(AreaData.transform.GetChild(gameData.LastActiveSection).GetComponent<SectionData>().playerInSection ? 1 : 0));

            Enemy.transform.localPosition = new Vector3(gameData.EnemiesInActiveSection[i].enemyParentPosX, gameData.EnemiesInActiveSection[i].enemyParentPosY, 0);

            Enemy.transform.GetChild(0).localPosition = new Vector3(gameData.EnemiesInActiveSection[i].enemyPosX, gameData.EnemiesInActiveSection[i].enemyPosY, 0);

            Enemy.transform.GetChild(0).GetComponent<EnemyBase>().healthPoints = gameData.EnemiesInActiveSection[i].CurrentHealthPoints;

            StartCoroutine(SetEnemyData(gameData, Enemy));
        }

        //}

    }

    //Has to do it this way and not in the loop above the call because then Enemy.currentIndex will be assigned from EnemyBase script then we can override the valuse afterwards, if u have done otherwise it would have been overridden the desired value
    IEnumerator SetEnemyData(GameData gameData, GameObject Enemy)
    {
        //wait for one mirco sec
        //1 microsecond(µs) = 0.000001 second
        yield return new WaitForSeconds(0.000001f);
        for (int i = 0; i < gameData.EnemiesInActiveSection.Count; i++)
        {
            Enemy.transform.GetChild(0).GetComponent<EnemyBase>().currentIndex = gameData.EnemiesInActiveSection[i].wayPointAproching;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            infoToggle = !infoToggle;
            Info.SetActive(infoToggle);
        }
    }

    public void TimeSlow(int slowAmount = 100, float forSec = 1)
    {
        slowAmount = slowAmount < 0 ? 0 : slowAmount > 100 ? 100 : slowAmount;
        Time.timeScale = (100 - slowAmount) / 100.0f;
        StartCoroutine(TimeBackToNormal(forSec));
    }

    IEnumerator TimeBackToNormal(float sec)
    {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < sec)
        {
            yield return null;
        }
        Time.timeScale = 1.0f;
    }

    public void SaveGameData()
    {
        GameData gameData = new GameData();

        //Player Data Save
        gameData.PlayerHealth = playerController.playerHealth.playerCurrentHealth;
        gameData.PlayerHasPet = playerController.petAbality.aquiredPet;
        gameData.PlayerPositionX = playerController.transform.position.x;
        gameData.PlayerPositionY = playerController.transform.position.y;

        //Area Data Save
        gameData.LastActiveSection = AreaData.activeSectionId;

        //Section Enemy Data
        gameData.EnemiesInActiveSection.Clear();

        foreach (var enemy in AreaData.transform.GetChild(AreaData.activeSectionId).GetComponent<SectionData>().Enemies)
        {
            var _enemy = enemy.transform.GetChild(0).GetComponent<EnemyBase>();

            gameData.EnemiesInActiveSection.Add(
                new Enemy()
                {
                    id = _enemy.EnemyId,
                    CurrentHealthPoints = _enemy.healthPoints,

                    enemyParentPosX = enemy.transform.localPosition.x,
                    enemyParentPosY = enemy.transform.localPosition.y,

                    enemyPosX = _enemy.transform.localPosition.x,
                    enemyPosY = _enemy.transform.localPosition.y,

                    wayPointAproching = _enemy.currentIndex
                }
            );
        }

        SerializeJson(gameData);
    }
}
