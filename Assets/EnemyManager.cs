using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    private List<BaseEnemy> allEnemies = new List<BaseEnemy>();

    public List<string> enemyRoster = new List<string>();

    // imprisoned enemies and their release rounds
    private Dictionary<string, int> imprisonedRoster = new Dictionary<string, int>();

    public string baldiTag = "Baldi";
    public string uiiaTag = "UIIA";
    public string oggyTag = "Oggy";

    void Awake()
    {
        instance = this;
    }

    public void RegisterEnemy(BaseEnemy enemy)
    {
        if (!allEnemies.Contains(enemy))
            allEnemies.Add(enemy);
    }

    public void UnregisterEnemy(BaseEnemy enemy)
    {
        allEnemies.Remove(enemy);
    }

    public void AddToRoster(string tag)
    {
        enemyRoster.Add(tag);
    }

    public void RemoveFromRoster(string tag)
    {
        enemyRoster.Remove(tag);
    }

    public void ImprisonEnemy(BaseEnemy enemy, int currentRound)
    {
        string tag = GetPoolTag(enemy);
        if (tag == null) return;

        enemyRoster.Remove(tag);
        imprisonedRoster[tag] = currentRound + 3;

        ObjectPooler.instance.ReturnEnemyToPool(tag, enemy);

        Debug.Log(tag + " imprisoned until round " + (currentRound + 3));
    }

    public void CheckPrisonReleases(int currentRound)
    {
        List<string> toRelease = new List<string>();

        foreach (var kvp in imprisonedRoster)
            if (currentRound >= kvp.Value)
                toRelease.Add(kvp.Key);

        foreach (string tag in toRelease)
        {
            imprisonedRoster.Remove(tag);
            enemyRoster.Add(tag);
            Debug.Log(tag + " released, added back to roster for round " + currentRound);
        }
    }

    public void DespawnAllEnemies()
    {
        for (int i = allEnemies.Count - 1; i >= 0; i--)
        {
            BaseEnemy enemy = allEnemies[i];
            string tag = GetPoolTag(enemy);
            if (tag != null)
                ObjectPooler.instance.ReturnEnemyToPool(tag, enemy);
        }
        allEnemies.Clear();
    }

    public void SpawnDefaultEnemies()
    {
        foreach (string tag in enemyRoster)
            SpawnEnemyFromPool(tag);
    }

    public void RespawnAllEnemies()
    {
        DespawnAllEnemies();
        SpawnDefaultEnemies();
    }

    public BaseEnemy SpawnEnemyFromPool(string tag)
    {
        BaseEnemy enemy = ObjectPooler.instance.GetEnemy(tag, Vector3.zero, Quaternion.identity);
        if (enemy == null) return null;
        RegisterEnemy(enemy);
        return enemy;
    }

    public string GetPoolTag(BaseEnemy enemy)
    {
        if (enemy is BaldiEnemy) return baldiTag;
        if (enemy is UIIAController) return uiiaTag;
        if (enemy is EnemyController) return oggyTag;
        return null;
    }

    public void StunAllEnemies(float duration)
    {
        foreach (BaseEnemy enemy in allEnemies)
            enemy.Stun(duration);
    }

    public void ResetAllEnemies()
    {
        foreach (BaseEnemy enemy in allEnemies)
            enemy.ResetEnemy();
    }

    public void SetSpeedMultiplierAll(float multiplier)
    {
        foreach (BaseEnemy enemy in allEnemies)
            enemy.SetSpeedMultiplier(multiplier);
    }

    public T GetEnemy<T>() where T : BaseEnemy
    {
        foreach (BaseEnemy enemy in allEnemies)
            if (enemy is T found) return found;
        return null;
    }

    public List<T> GetAllEnemiesOfType<T>() where T : BaseEnemy
    {
        List<T> result = new List<T>();
        foreach (BaseEnemy enemy in allEnemies)
            if (enemy is T found) result.Add(found);
        return result;
    }

    public void PauseUIIAMusic()
    {
        foreach (UIIAController uiia in GetAllEnemiesOfType<UIIAController>())
            uiia.UiiaAudioSource.Pause();
    }

    public void UnpauseUIIAMusic()
    {
        foreach (UIIAController uiia in GetAllEnemiesOfType<UIIAController>())
            uiia.UiiaAudioSource.Play();
    }

    public bool IsImprisoned(string tag)
    {
        return imprisonedRoster.ContainsKey(tag);
    }

    public void EnrageAllEnemies(bool enraged)
    {
        foreach (BaseEnemy enemy in allEnemies)
        {
            if (enraged) enemy.Enrage();
            else enemy.UnEnrage();
        }
    }

    public void EnrageAllOfType<T>(bool enraged) where T : BaseEnemy
    {
        foreach (T enemy in GetAllEnemiesOfType<T>())
        {
            if (enraged) enemy.Enrage();
            else enemy.UnEnrage();
        }
    }

    public List<BaseEnemy> GetAllEnemies() => allEnemies;
}