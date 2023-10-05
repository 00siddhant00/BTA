using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace Assets.Scripts.Other
{
    public class Score : MonoBehaviour
    {
        public static int score;

        public TextMeshProUGUI scoreText;

        public List<Transform> CoinPos;
        public GameObject CoinPrefab;
        public GameObject Coins;

        // Use this for initialization
        void Start()
        {
            score = 0;
            SpawnCoins();
        }

        // Update is called once per frame
        void Update()
        {
            scoreText.text = score.ToString();
        }

        void SpawnCoins()
        {
            for (int i = 0; i < 10; i++)
            {
                var pos = CoinPos[Random.Range(0, CoinPos.Count)];
                var coin = Instantiate(CoinPrefab, pos.position, Quaternion.identity, Coins.transform);
                CoinPos.Remove(pos);
            }
        }
    }
}