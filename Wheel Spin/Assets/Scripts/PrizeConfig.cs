using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Prize", menuName = "WheelSpinPrize", order = 0)]
public class PrizeConfig : ScriptableObject
{
    [SerializeField] public GameObject prizePrefab;
    [SerializeField] public string prizeName;
}
