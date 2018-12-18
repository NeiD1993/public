using Assets.Entities.FieldObjectsService.FieldStaticObjectsGenerator;
using Assets.Entities.FieldObjects;
using UnityEngine;
using Assets.Entities.FieldObjectsService.FieldGenerator.FieldStaticObjectsGenerator;
using Assets.Entities.FieldObjectsService.FieldDynamicObjectsMover;
using Assets.Entities.FieldObjectsService.FieldObjectsDestroyer;
using Assets.Entities.FieldObjectsService;
using Assets.Entities.FieldObjecsService.PlayerFieldObjectStatistics;
using System.Collections.Generic;

public class MainCameraBehaviour : MonoBehaviour
{
    private Field field;

    public int horizontalSize;

    public int verticalSize;

    public GameObject floorPrefab;

    public Material floorMaterial;

    public GameObject wallPrefab;

    public Material unbreakableWallMaterial;

    public Material breakableWallMaterial;

    public byte breakableWallsPercentage;

    public GameObject playerPrefab;

    public int playerMovingSpeed;

    public GameObject balloomPrefab;

    public int ballomKillPoints;

    public string ballomTag;

    public GameObject kondoriaPrefab;

    public int kondoriaKillPoints;

    public string kondoriaTag;

    public GameObject ovapiPrefab;

    public int ovapiKillPoints;

    public string ovapiTag;

    public int enemyMovingSpeed;

    public byte enemyDelay;

    public int enemiesCount;

    public GameObject bombPrefab;

    public byte bombDelay;

    public int maxPlacedBombsCount;

    public GameObject explosionWavePartPrefab;

    public float explosionWaveDelay;

    public float explosionWaveStep;

    public int explosionWaveDistance;

    public GameObject explosionWaveDistanceIncreasingPrefab;

    public GameObject maxPlacedBombsCountIncreasingPrefab;

    public GameObject speedIncreasingPrefab;

    public GameObject wallPassPrefab;

    public int explosionWaveDistanceIncreasingCount;

    public int maxPlacedBombsCountIncreasingCount;

    public int speedIncreasingCount;

    public int wallPassCount;

    public Vector3 powerUpsRotationAngle;

    public float powerUpDelay;

    public int maxClosedPathCellsPercentage;
    
    public GameObject helpTextPrefab;

    public GameObject statisticsPrefab;

    public GameObject lightPrefab;

    public GameObject textPrefab;

    public Material explosionWaveDistanceIncreasingMaterial;

    public Material maxBombPlaceCountIncreasingMaterial;

    public Material speedIncreasingMaterial;

    public Material wallPassMaterial;

    public int destroyableActionDelay;

    public Material killEnemyTextMaterial;

    public int objectSecondsLifetime;

    public AudioClip playerStepSound;

    public AudioClip enemyStepSound;

    public AudioClip setBombSound;

    public AudioClip bombExplosionSound;

    public AudioClip playerDeathSound;

    public AudioClip enemyDeathSound;

    public AudioClip enemyHitSound;

    public AudioClip explosionWaveDistanceIncreasingPowerUpPickUpSound;

    public AudioClip maxBombPlaceCountIncreasingPowerUpPickUpSound;

    public AudioClip speedIncreasingPowerUpPickUpSound;

    public AudioClip wallPassPowerUpPickUpSound;

    public AudioClip playerWinSound;

    public AudioClip enemyWinSound;

    public GameObject resultsTextPrefab;

    public Material loseResultsTextColor;

    public Material winResultsTextColor;

    void Start()
    {
        field = ScriptableObject.CreateInstance<Field>();
        field.HorizontalSize = horizontalSize;
        field.VerticalSize = verticalSize;
        field.WallPrefab = wallPrefab;
        field.LightPrefab = lightPrefab;
        field.ResultsTextPrefab = resultsTextPrefab;
        field.TextPrefab = textPrefab;
        field.DestroyableActionDelay = destroyableActionDelay;
        field.KillEnemyTextMaterial = killEnemyTextMaterial;
        field.LightMaterials = new Dictionary<string, Material>() { { "Wall Pass", wallPassMaterial },
                                                                    { "Moving Speed", speedIncreasingMaterial },
                                                                    { "Explosion Wave Distance", explosionWaveDistanceIncreasingMaterial },
                                                                    { "Remained Bombs", maxBombPlaceCountIncreasingMaterial },
                                                                    { "Kill Enemy", killEnemyTextMaterial }};
        field.FieldObjectsSounds = new Dictionary<string, AudioClip>() { { "Player Step", playerStepSound },
                                                                         { "Enemy Step", enemyStepSound },
                                                                         { "Set Bomb", setBombSound },
                                                                         { "Bomb Explosion", bombExplosionSound},
                                                                         { "Player Death", playerDeathSound },
                                                                         { "Enemy Death", enemyDeathSound }, 
                                                                         { "Enemy Hit", enemyHitSound },
                                                                         { "Explosion Wave Distance Increasing Power Up Pick Up", explosionWaveDistanceIncreasingPowerUpPickUpSound},
                                                                         { "Max Bomb Place Count Power Up Pick Up", maxBombPlaceCountIncreasingPowerUpPickUpSound},
                                                                         { "Speed Increasing Power Up Pick Up", speedIncreasingPowerUpPickUpSound},
                                                                         { "Wall Pass Power Up Pick Up", wallPassPowerUpPickUpSound},
                                                                         { "Player Win", playerWinSound},
                                                                         { "Enemy Win", enemyWinSound} };
        field.ResultsTextsMaterials = new Dictionary<string, Material>() { { "You Lose!", loseResultsTextColor },
                                                                           { "You Win!", winResultsTextColor } };
        field.FieldObjectsDestroyer = new FieldObjectsDestroyer(field, objectSecondsLifetime);
        field.FieldStaticObjectsGenerator = new FieldStaticObjectsGenerator(breakableWallsPercentage, field);
        field.FieldDynamicObjectsGenerator = new FieldDynamicObjectsGenerator(field);
        field.FieldDynamicObjectsMover = new FieldDynamicObjectsMover(field, maxClosedPathCellsPercentage);
        field.FieldObjectsComponentsGetter = new FieldObjectsComponentsGetter(field);
        field.FieldObjectsStatistics = new FieldObjectsStatistics(field, new Dictionary<string, int>() { { ballomTag, ballomKillPoints },
                                                                                                         { kondoriaTag, kondoriaKillPoints },
                                                                                                         { ovapiTag, ovapiKillPoints } });
        field.FieldStaticObjectsGenerator.GenerateField(floorPrefab, helpTextPrefab, statisticsPrefab, breakableWallMaterial, floorMaterial, unbreakableWallMaterial,
                                                        new int[] { explosionWaveDistanceIncreasingCount, maxPlacedBombsCountIncreasingCount, speedIncreasingCount, wallPassCount },
                                                        new GameObject[] { explosionWaveDistanceIncreasingPrefab, maxPlacedBombsCountIncreasingPrefab, speedIncreasingPrefab, wallPassPrefab },
                                                        powerUpsRotationAngle, powerUpDelay);
        field.FieldDynamicObjectsGenerator.CreatePlayer(playerPrefab, bombPrefab, explosionWavePartPrefab, playerMovingSpeed, bombDelay, explosionWaveDistance, explosionWaveDelay, explosionWaveStep,
                                                        maxPlacedBombsCount);
        field.FieldDynamicObjectsGenerator.CreateEnemy(enemiesCount, new GameObject[] { ovapiPrefab, kondoriaPrefab, balloomPrefab }, enemyMovingSpeed, enemyDelay);
    }
}
