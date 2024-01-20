using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu()]
public class Level : ScriptableObject
{
    public string sceneName;
    public int levelNumber;
    public int sheepCount;
    public bool unlocked;
    public bool completed;
    public EnemyType[] typesOfEnemies;
    public float spawnRate;
    public float spawnWaves;
    public int minSheepToCompleteLevel;
    public int pointsPerSheep;


    //map/terrain/layout
    //UI Display Info variable (to display timer or no timer or any level-specific info)
}
