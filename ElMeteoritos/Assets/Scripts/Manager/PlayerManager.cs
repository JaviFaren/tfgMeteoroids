using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [Header("Jugadores")]
    [SerializeField] private List<Player> Players = new();

    [Header("SpawnPoints")]
    [SerializeField] private List<SpawnPoint> playerSpawnPoints;

    [Header("Disparos")]
    [SerializeField] private GameObject shotsContainer;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    #region Lista de jugadores

    public void AddPlayerToPlayersList(Player player)
    {
        if (!Players.Contains(player))
        {
            Players.Add(player);
        }
    }

    public void RemovePlayerFromPlayersList(Player player)
    {
        if (Players.Contains(player))
        {
            Players.Remove(player);
        }
    }

    public List<Player> GetPlayersList() => Players;

    public int GetPlayersCount() => Players.Count;

    public Player GetRandomPlayer()
    {
        var randomPlayer = Players[Random.Range(0, Players.Count)];

        while (randomPlayer.IsDead)
        {
            randomPlayer = Players[Random.Range(0, Players.Count)];
        }

        return randomPlayer;
    }

    #endregion

    #region Spawn

    public IEnumerator SpawnPlayers()
    {
        int i = System.Array.IndexOf(PhotonNetwork.PlayerList, PhotonNetwork.LocalPlayer);
        Vector3 spawnPosition = GetSpawnPoint(i);

        PhotonNetwork.Instantiate("Player", spawnPosition, Quaternion.identity);

        yield return new WaitUntil(() =>
            Players.Count == PhotonNetwork.PlayerList.Length &&
            Players.TrueForAll(p => p.IsInitialized)
        );
    }

    public void ActivatePlayersScreenPositionCheck()
    {
        foreach (var player in Players)
        {
            if (player == null) continue;

            if (player.TryGetComponent<ScreenPositionCheck>(out var screenCheck))
            {
                screenCheck.enabled = true;
            }
            else
            {
                Debug.LogWarning($"Player '{player.name}' no tiene un componente ScreenPositionCheck.");
            }
        }
    }

    public Vector3 GetSpawnPoint(int index) => playerSpawnPoints[index].position;

    #endregion
    
    #region Jugador

    public Player GetPlayerByID(int playerID)
    {
        Player player = Players.Find(player => player.playerID == playerID);
        if (player != null)
        {
            return player;
        }
        else
        {
            Debug.LogWarning($"No se encontró el jugador con ID {playerID}");
            return null;
        }
    }

    public Player GetMasterPlayer() => Players.Find(p => p.photonView.Owner.IsMasterClient);

    public Vector3 GetRandomPlayerPosition()
    {
        if (Players == null || Players.Count == 0) return Vector3.zero;

        int randomIndex = Random.Range(0, Players.Count);
        Debug.Log($"Objetivo -> {Players[randomIndex].GetComponent<Player>().username}");
        return Players[randomIndex].transform.position;
    }

    public void SetPlayersCombatState(bool value)
    {
        foreach (var player in Players)
        {
            player.CanShoot = player.CanGetDamaged = value;
        }
    }

    #endregion
}

[System.Serializable]
public class SpawnPoint
{
    public Vector3 position;
}
