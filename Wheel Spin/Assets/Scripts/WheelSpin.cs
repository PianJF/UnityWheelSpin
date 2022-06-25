using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class WheelSpin : MonoBehaviour
{   
    [SerializeField] GameObject wheelBoard;
    [SerializeField] float fastSpinDuration;
    [SerializeField] float slowSpinDuration;
    [SerializeField] float xSlowSpinDuration;

    public int prizeIndex;
    List<PrizeConfig> prizeList;
    List<PrizeConfig> prizePool = new List<PrizeConfig>();
    float fastSpinDegrees;
    float slowSpinDegrees;
    float extraDegrees;


    void Start()
    {
        prizeList = gameObject.GetComponent<Game>().prizeList;
        GeneratePrizePool();
    }

    // int CalculatePrizeIndex(float randomNum)
    // {   
    //     if(randomNum < 0.2) return 0;
    //     else if(0.2 <= randomNum && randomNum < 0.3) return 1;
    //     else if(0.3 <= randomNum && randomNum < 0.4) return 2;
    //     else if(0.4 <= randomNum && randomNum < 0.5) return 3;
    //     else if(0.5 <= randomNum && randomNum < 0.55) return 4;
    //     else if(0.55 <= randomNum && randomNum < 0.75) return 5;
    //     else if(0.75 <= randomNum && randomNum < 0.8) return 6;
    //     else return 7;
    // }

    void GeneratePrizePool()
    {
        foreach(PrizeConfig prizeConfig in prizeList)
        {   
            prizePool.AddRange(Enumerable.Repeat(prizeConfig, prizeConfig.percentage));
        }
    }

    int LookupPrizeIndex(int num)
    {
        return prizePool[num].index;
    }

    public void Spin()
    {   
        // The wheel will fast spin fisrt then slow spin the last half circle;
        int randomNum = Random.Range(0, prizePool.Count);
        prizeIndex = LookupPrizeIndex(randomNum);
        Debug.Log(prizeIndex);
        
        extraDegrees = Mathf.Clamp(randomNum/prizePool.Count, 0.1f, 0.9f)  * -45f; 
        fastSpinDegrees = -1 * CalculateFastSpinDegrees(prizeIndex);
        slowSpinDegrees = -180f;

        StartCoroutine(RotateWheel());
    }

    float CalculateFastSpinDegrees(int index)
    {   
        // Total fast spin degress
        /// <summary>
        /// The pointer pointing at edge of prize 0 and 7, prize 0 - 3 will spin 2 full circle
        /// plus (3 - x) * 45, prize 4 - 7 will spin into another round becuase theres less 
        /// than half circle to slow spin, thus those needs finish current round (7 - x) then
        /// plus another half circie 4 * 45, so (7 - x + 4) * 45
        /// </summary>

        if(index <= 3) return (3 - index) * 45f + 2 * 360f;
        else return (11 - index) * 45f + 2 * 360f;
    }

    IEnumerator RotateOverTime(float degress, float duration)
    {
        float startRotation = wheelBoard.transform.eulerAngles.z;
        float endRotation = startRotation + degress;
        float time = 0f;

        while ( time  < duration )
        {
            time += Time.deltaTime;
            float zRotation = Mathf.Lerp(startRotation, endRotation, time / duration) % 360.0f;
            wheelBoard.transform.eulerAngles = new Vector3(0f, 0f, zRotation);
            yield return null;
        }
    }

    IEnumerator RotateWheel()
    {
        yield return RotateOverTime(fastSpinDegrees, fastSpinDuration);
        yield return RotateOverTime(slowSpinDegrees, slowSpinDuration);
        yield return RotateOverTime(extraDegrees, xSlowSpinDuration);
    }

    public void ResetWheelBoard()
    {
        wheelBoard.transform.eulerAngles = new Vector3(0f, 0f, 0f);
    }
}
