using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Player Health")] 
    public LifeBar health;
    public int playerMaxHealth = 100;
    private int _playerHealth;

    [SerializeField]
    private TextMeshProUGUI _gameOver;

    [SerializeField]
    private TextMeshProUGUI _victory;

    [Header("Tags")]
    private string _minotaurTag;
    private string _swarmAgentTag;
    private string _floorTag;

    [Header("Map")]
    public MapManager mapManager;
    public Material stringMaterial;

    [Header("Collision")]
    private bool _hit = false;

    void Awake()
    {
        // initialization
        _minotaurTag = "Minotaur";
        _swarmAgentTag = "Swarm Particle";
        _floorTag = "Floor";
        
        _gameOver.enabled = false;
        _victory.enabled = false;
    }

    void Start()
    {
        // initialization of player health 
        _playerHealth = playerMaxHealth;
        health.InitializeHealth(playerMaxHealth);
    }

    void RemoveHealth(int healthToSubtract)
    {
        // removes health 
        _playerHealth -= healthToSubtract;
        health.ModifyHealth(_playerHealth);
    }

    void FixedUpdate()
    {
        // end game if player health drops below 0 
        if (_playerHealth < 0)
        {
            _gameOver.enabled = true;
            Time.timeScale = 0;
        }
    }

    void AriadneString()
    {
        // mark on map where player is 
        int x, z;
        mapManager.ConvertCartesianToCell(transform.position.x, transform.position.z, out x, out z);

        string tileObject = "/Map/FloorSection " + x.ToString() + "," + z.ToString();
        GameObject tile = GameObject.Find(tileObject);

        try
        {
            MeshRenderer rend = tile.GetComponent<MeshRenderer>();
            rend.material = stringMaterial;
        }
        catch(NullReferenceException e)
        {
            //if outside map, stop game 
            _victory.enabled = true;
            Time.timeScale = 0;
            Debug.LogError("Tile was not found");
        }
    }

    void Update()
    {
        // helps with duplicate collisions 
        if (_hit)
            _hit = false;

        AriadneString();
    }

    void OnCollisionEnter(Collision collision)
    {
        // remove 50 health if hit by minotaur, 10 if by swarm agent 
        if ((collision.collider.gameObject.tag != _floorTag) & (!_hit || _hit))
        {
            if (collision.collider.tag == _minotaurTag)
            {
                RemoveHealth(50);
            }
            else if (collision.collider.tag == _swarmAgentTag)
            {
                RemoveHealth(10);
            }
            else
            {
                ;
            }
            _hit = true;
        }
    }

}
