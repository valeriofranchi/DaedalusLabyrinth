using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmAgent : MonoBehaviour
{
    private Collider _agentCollider;
    public Collider AgentCollider { get { return _agentCollider; } set { _agentCollider = value; } }

    private Vector3 _velocity;
    public Vector3 Velocity { get { return _velocity; } set { _velocity = value; } }

    private bool _playerHit;
    public bool PlayerHit {  get { return _playerHit;  } set { _playerHit = value; } }

    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        // get collider, layer and tag
        _agentCollider = GetComponent<Collider>();
        gameObject.layer = LayerMask.NameToLayer("Swarm Particle");
        gameObject.tag = "Swarm Particle";

        _playerHit = false;
    }

    // Update is called once per frame
    void Update()
    {
        _playerHit = false;
    }

    public void Move(Vector3 vel)
    {
        transform.position += vel * Time.deltaTime;
    }

    void onTriggerEnter(Collider collider)
    {
        // apply rigid body to player 
        if (collider.gameObject.tag == "Player")
        {
            Vector3 dir = transform.position - collider.transform.position;
            player.GetComponent<Rigidbody>().AddForce(dir.normalized * -100.0f, ForceMode.Impulse);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // make swarm agent inactive 
        if (collision.collider.tag == "Player")
        {
            _playerHit = true;

            // disappear
            gameObject.SetActive(false);
        }
    }
}
