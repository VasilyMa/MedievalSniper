using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InterfaceConfig", menuName = "Data/Interface")]
public class InterfaceConfig : ScriptableObject
{
    public GameObject prefabIndicator;
    public GameObject prefabProduct;
    public GameObject Bow;

    public GameObject levelPrefab;
    public Sprite CompleteLevel;
    public Sprite CurrentLevel;
    public Sprite ClosedLevel;

    public Sprite EmptyStar;
    public Sprite HalfStarRight;
    public Sprite HalfStarLeft;
    public Sprite FullStar;

    public Sprite EmptyLevel;
    public Sprite FirstLevel;
    public Sprite SecondLevel;
    public Sprite ThirdLevel;

    public Sprite SoundOff;
    public Sprite SoundOn;
    public Sprite VibroOn;
    public Sprite VibroOff;


    public ScorePrefabMB prefabScore;
}
