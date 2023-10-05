using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public CameraShake CameraShake;
    public PlayerController playerController;
    public Pet pet;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        //if (Instance == null)
        //{
        //    DontDestroyOnLoad(gameObject);
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}

        playerController = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
