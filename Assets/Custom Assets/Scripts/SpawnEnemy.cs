using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    Level level;
    int wave;

    EnemyType[] typesOfEnemies;

    bool wolf;
    bool ghost;
    bool dragon;
    bool evilVine;

    EnemyType wolfET;
    EnemyType ghostET;
    EnemyType dragonET;
    EnemyType evilVineET;

    private void Start()
    {
        level = GameManager.instance.currentLevelData;
        wave = 0;
        typesOfEnemies = level.typesOfEnemies;

        for (int i = 0; i < typesOfEnemies.Length - 1; i++)
        {
            if (typesOfEnemies[i].name == "Wolf")
            {
                wolf = true;
                wolfET = typesOfEnemies[i];
            }
            else if (typesOfEnemies[i].name == "Ghost")
            {
                ghost = true;
                ghostET = typesOfEnemies[i];
            }
            else if (typesOfEnemies[i].name == "Dragon")
            {
                dragon = true;
                dragonET = typesOfEnemies[i];
            }
            else
            {
                evilVine = true;
                evilVineET = typesOfEnemies[i];
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            
            if (wave < level.spawnWaves)
            {
                if (wolf)
                {
                    //instantiate spawnRate number of wolves at location
                }
            }


        }
    }
}
