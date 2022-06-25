using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class Game : MonoBehaviour
{
    [SerializeField] Button playBtn;
    [SerializeField] Button claimBtn;
    [SerializeField] GameObject wheel;
    [SerializeField] public List<PrizeConfig> prizeList;
    [SerializeField] List<GameObject> prizeContainer;
    [SerializeField] GameObject prize;
    [SerializeField] ParticleSystem prizeEffect;

    float wheelSpinDuration = 4.5f;
    List<GameObject> destroyOnNewSpin = new List<GameObject>();
    ParticleSystem prizeEffectGameObj;
    int simulateSpinNum = 1000;
    string outPutFilePath = "Assets/simulationSpinResult.txt";
    List<int> simulationResultCounter = new List<int>(new int[8]);

    void Awake()
    {
        playBtn.onClick.AddListener(PlayButtonClickedWrap);
        claimBtn.onClick.AddListener(ResetGameWrap);
    }

    void Start()
    {
        SpwanPrizeOnBoard();
        SpwanPrizeEffect();

        //StartCoroutine(SimulateNSpins(simulateSpinNum));
    }

    void PlayButtonClickedWrap()
    {
        StartCoroutine(PlayButtonClicked());
    }

    IEnumerator PlayButtonClicked()        
    {
        gameObject.GetComponent<WheelSpin>().Spin();
        yield return new WaitForSeconds(wheelSpinDuration);

        claimBtn.gameObject.SetActive(true);
        wheel.gameObject.SetActive(false);

        SpwanPrize();
        prize.GetComponentInChildren<Animator>().SetTrigger("StartPrizeAnim");
        yield return new WaitForSeconds(1f);

        prizeEffectGameObj.Play();
    }

    void SpwanPrizeOnBoard()
    {
        for(int i = 0; i < prizeContainer.Count; i++)
        {
            Instantiate(prizeList[i].prizePrefab, prizeContainer[i].transform);
        }
    }

    void SpwanPrize()
    {   
        int prizeIndex = gameObject.GetComponent<WheelSpin>().prizeIndex;
        GameObject prizeGameObj = Instantiate(prizeList[prizeIndex].prizePrefab, prize.transform);
        destroyOnNewSpin.Add(prizeGameObj);
    }

    void SpwanPrizeEffect()
    {
        prizeEffectGameObj = Instantiate(prizeEffect, prize.transform);
    }

    IEnumerator ResetGame()
    {   
        claimBtn.gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);
        prizeEffectGameObj.Stop();
        foreach(GameObject destroyGameObject in destroyOnNewSpin)
        {
            Destroy(destroyGameObject);
        }
        gameObject.GetComponent<WheelSpin>().ResetWheelBoard();
        
        yield return new WaitForSeconds(1f);

        wheel.gameObject.SetActive(true);
        playBtn.gameObject.SetActive(true);
    }

    void ResetGameWrap()
    {
        StartCoroutine(ResetGame());
    }

    IEnumerator SimulateNSpins(int numOfSpin)
    {
        for(int i = 0; i < numOfSpin; i++)
        {   
            Debug.Log("Simulation: " + i.ToString());
            yield return PlayButtonClicked();
            yield return new WaitForSeconds(1f);
            int prizeIndex = gameObject.GetComponent<WheelSpin>().prizeIndex;
            simulationResultCounter[prizeIndex] += 1;
            yield return ResetGame();
            yield return new WaitForSeconds(1f);
        }

        WriteResultToFile();
    }

    void WriteResultToFile()
    {
        StreamWriter writer = new StreamWriter(outPutFilePath, true);
        writer.WriteLine(string.Format("Simulate {0} spins", simulateSpinNum));
        writer.WriteLine("");

        for(int i = 0; i < prizeList.Count; i++)
        {
            writer.WriteLine(string.Format("{0}: {1} times", prizeList[i].prizeName, simulationResultCounter[i]));
        }
        writer.Close();
    }
}
