using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Swarm : MonoBehaviour
{
    [Header("Swarm")]
    public SwarmAgent agentPrefab;
    public Transform spawn;
    public Material leaderMaterial;
    public Material followerMaterial;
    public int agentPopulation = 12;

    const float agentDensity = 1.75f;
    List<SwarmAgent> agents = new List<SwarmAgent>();
    private int _leaderIndex;
    List<Vector3> spawnPositions = new List<Vector3>();

    [Space(15)]

    [Header("Swarm Behaviour")]
    public float[] behaviourWeights;
    public float neighbourRadius = 1.5f;
    public float avoidanceRadius = 1.0f;
    public float neighbourAngle = 90.0f;
    public bool smoothCohesion = true;
    public float smoothTimeCohesion = 0.5f;

    private SwarmBehaviour _agentBehaviour;
    private float _neighbourRadiusSquared;
    private float _avoidanceRadiusSquared;

    public float NeighbourRadiusSquared { get { return _neighbourRadiusSquared; } }
    public float AvoidanceRadiusSquared { get { return _avoidanceRadiusSquared; } }

    private bool _stop = false;
    private bool _start = false;

    [Space(15)]

    [Header("Physical and Kinematic properties")]
    public float angularSpeed = 1.0f;
    public float speed = 3.50f;
    public float attackingSpeed = 4.00f;
    public float mass = 3.0f;
    public float maxForce = 5.0f;
    public float maxSpeed = 5f;

    private bool _attackingPlayer = false;
    public float attackDuration = 10.0f;
    private float _attackingElapsedTime = 0.0f;

    private float _maxSpeedSquared;
    private float _maxForceSquared;

    public float MaxSpeedSquared { get { return _maxSpeedSquared; } }
    public float MaxForceSquared { get { return _maxForceSquared; } }

    [Space(15)]

    [Header("Path Planning")]
    public PathManager pathManager;
    public Material material;
    public float distanceMovedThreshold = 5.0f;
    public float attackingRange = 1.0f;

    private List<Vector3> _path;
    private int _sizeOfPath;

    private float _distanceMovedThresholdSquared;
    private float _attackingRangeSquared;
    private bool _findingTarget;
    private Vector3 _playerPosition;

    public List<Vector3> Path { get { return _path; } set { _path = value; } }
    public int SizeOfPath { get { return _sizeOfPath; } set { _sizeOfPath = value; } }

    [Space(15)]

    [Header("Related objects")]
    public GameObject player;
    private int _minotaurLayerMask;
    private int _agentLayerMask;
    private int _playerLayerMask;
    private int _wallLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        // initialize path
        _path = new List<Vector3>();

        // initialize minotaur layer mask 
        _minotaurLayerMask = 1 << LayerMask.NameToLayer("Minotaur");
        _agentLayerMask = 1 << LayerMask.NameToLayer("Swarm Particle");
        _playerLayerMask = 1 << player.layer;
        _wallLayerMask = 1 << LayerMask.NameToLayer("Obstacle");

        // initialize squared variables for quicker computations 
        _distanceMovedThresholdSquared = distanceMovedThreshold * distanceMovedThreshold;
        _attackingRangeSquared = attackingRange * attackingRange;
        _maxSpeedSquared = maxSpeed * maxSpeed;
        _maxForceSquared = maxForce * maxForce;
        _avoidanceRadiusSquared = avoidanceRadius * avoidanceRadius;
        _neighbourRadiusSquared = neighbourRadius * neighbourRadius;

        // initialize swarm behaviour constructor 
        _agentBehaviour = new SwarmBehaviour(smoothCohesion, smoothTimeCohesion);

        // initialize agents
        InitializeAgents();

        // start actions for swarm 
        _findingTarget = false;
        //StartCoroutine("LocateAttackRespawn");

    }

    void InitializeAgents()
    {
        // initialize swarm leader

        SwarmAgent swarmLeader = Instantiate(agentPrefab,
            spawn.position,
            spawn.rotation,
            transform);

        // set gameobject name 
        swarmLeader.name = "LeaderAgent";

        // set material 
        MeshRenderer rend1 = swarmLeader.GetComponent<MeshRenderer>();
        rend1.material = leaderMaterial;

        // add to swarm 
        agents.Add(swarmLeader);
        _leaderIndex = 0;
        spawnPositions.Add(spawn.position);

        Debug.Log(spawn.position);

        Transform[] children = spawn.GetComponentsInChildren<Transform>();
        
        agentPopulation = children.Length;
        
        // add followers to swarm 
        for (int i = 1; i < agentPopulation; i++)
        {
            Vector3 followerPosition = Vector3.zero;
            bool behindLeader = false;

            Transform childTransform = children[i];

            followerPosition = childTransform.position;
            Debug.Log(childTransform.position);
            Debug.Log(childTransform.gameObject.name);
                
            followerPosition.y = 0.5f;
          
             spawnPositions.Add(followerPosition);

            // initialize follower 
            SwarmAgent swarmFollower = Instantiate(agentPrefab,
                followerPosition,
                spawn.rotation,
                transform
                );

            // set material
            MeshRenderer rend2 = swarmFollower.GetComponent<MeshRenderer>();
            rend2.material = followerMaterial;

            // set gameobject name 
            swarmFollower.name = "FollowerAgent " + i;

            // add to swarm
            agents.Add(swarmFollower);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_stop)
            StopAllCoroutines();

        Vector3 dist = player.transform.position - agents[_leaderIndex].transform.position;

        if ((dist.sqrMagnitude < 15.0f * 15.0f) && !_start)
        {
            StartCoroutine("LocateAttackRespawn");
            _start = true;
        }
    }

    void CalculateAgentMovement(SwarmAgent agent, Vector3 desiredPosition, bool leader, out Vector3 vel)
    {
        // get desired velocity based on current and target point 
        Vector3 desiredVel = (desiredPosition - agent.transform.position).normalized;

        // get all transforms of all objects in range 
        int layerMask = leader ? (_minotaurLayerMask | _wallLayerMask) : 
            (_minotaurLayerMask | _agentLayerMask | _wallLayerMask);
        List<Transform> objectsInRange = GetEverythingInRange(agent, layerMask);

        if (agent == null)
            Debug.Log("Agent is null");

        if (objectsInRange == null)
            Debug.Log("Objects are null");

        if (this == null)
            Debug.Log("Swarm is null");

        // calculate move based on objects in surroundings and normalize it 
        float[] modifiedWeights = behaviourWeights;
        if (leader)
        {
            modifiedWeights[0] = 0.0f;
            modifiedWeights[1] = 0.0f;
        }

        vel = desiredVel + _agentBehaviour.CalculateMove(agent, objectsInRange, modifiedWeights, this);
            
        if (vel.sqrMagnitude > _maxSpeedSquared)
            vel = vel.normalized * maxSpeed;
    }

    List<Transform> GetEverythingInRange(SwarmAgent agent, int layerMask)
    {
        // retrieve every collider in a circular neighbourhood around the agent 
        List<Transform> everythingInRange = new List<Transform>();
        Collider[] colliders = Physics.OverlapSphere(agent.transform.position, neighbourRadius, layerMask);

        // problem, if there are walls or other players, agents will not do 3 behaviours properly
        foreach(Collider collider in colliders)
        {
            // if the collider does not correspond to the same agent 
            if (collider != agent.AgentCollider)
            {
                // if the angle between the current agent and the detected agent is above a threshold, add it 
                Vector3 distanceBetweenAgents = collider.transform.position - agent.transform.position;
                if (Vector3.Dot(distanceBetweenAgents, agent.transform.forward) > Mathf.Cos(neighbourAngle * Mathf.PI / 180.0f))
                    everythingInRange.Add(collider.transform);
            }
        }
        return everythingInRange;
    }

    void GetPath(Vector3 currentPosition, Vector3 destination, bool visualize)
    {   
        // find path to target 
        int size;
        List<Vector3> generatedPath;
        pathManager.findPath(currentPosition, destination, out generatedPath, out size);

        // remove redundant nodes inside path and colour waypoint tiles in map 
        pathManager.FilterPath(generatedPath, size, out _path);

        if (visualize)
            ColourTiles(Path);
    }

    void ResetLeader()
    {
        // get indices of all alive followers 
        List<int> aliveIndices = new List<int>();
        for (int i = 0; i < agents.Count; i++)
        {
            if (agents[i].gameObject.activeSelf == true)
            {
                Debug.Log("agent alive: " + agents[i].gameObject.name);
                aliveIndices.Add(i);
            }
        }

        // get random integer 
        int randomInt = Random.Range(0, aliveIndices.Count - 1);

        // copy agent to temporary variable, modify name and re-assign it
        SwarmAgent temp = agents[aliveIndices[randomInt]];
        SwarmAgent copy = agents[aliveIndices[randomInt]];
        temp.name = "LeaderAgent";
        agents[aliveIndices[randomInt]] = temp;
        agents[_leaderIndex] = copy;
        MeshRenderer rend = agents[aliveIndices[randomInt]].GetComponent<MeshRenderer>();
        rend.material = leaderMaterial;

        // set global leader index
        _leaderIndex = aliveIndices[randomInt];  
    }

    // pathfinding implemented by myself, path smoothing and its integration with inspired by https://github.com/SebLague/Pathfinding 
    IEnumerator LocateAttackRespawn()
    {
        yield return new WaitForSecondsRealtime(4);

        StartCoroutine("RefactorSwarm");

        while (true)
        {

            // if not attacking, check if inside range 
            if (!_attackingPlayer)
            {
                // if the player target is found, stop following waypoints and attack
                Vector3 distanceToTarget = player.transform.position - agents[_leaderIndex].transform.position;

                if (!_findingTarget)
                {
                    Debug.Log("Tracking");

                    // get current position of player inside the map
                    _playerPosition = player.transform.position;

                    // find path to player and find him 
                    while (_path.Count == 0)
                        GetPath(agents[_leaderIndex].gameObject.transform.position, _playerPosition, false);

                    StartCoroutine("MoveSwarm");
                }
                // update path if player has moved by a certain amount 
                Vector3 difference = player.transform.position - _playerPosition;
                if (difference.sqrMagnitude > distanceMovedThreshold)
                {
                    if (_findingTarget)
                    {
                        Debug.Log("Recalculating...");

                        StopCoroutine("MoveSwarm");
                        _findingTarget = false;

                        if (_path != null)
                            _path.Clear();
                    }
                    // get current position of player inside the map
                    _playerPosition = player.transform.position;

                    while (_path.Count == 0)
                        GetPath(agents[_leaderIndex].transform.position, player.transform.position, false);
                    StartCoroutine("MoveSwarm");

                }
            }
            yield return null;
        }
        
    }

    IEnumerator RefactorSwarm()
    {
        while (true)
        {
            // check if leader dead 
            bool leaderDead = (agents[_leaderIndex].gameObject.activeSelf == false) ? true : false;

            // if leader not dead, do nothing 
            if (!leaderDead)
            {
                ;

            }
            // if leader dead, decide depending on followers 
            else
            {
                bool allDead = true;
                foreach (SwarmAgent agent in agents)
                {
                    if (agent.gameObject.activeSelf == true)
                    {
                        allDead = false;
                        break;
                    }
                }

                // if all followers dead, stop everything 
                if (allDead)
                {
                    _stop = true;
                }

                // if only part are dead, remove dead ones, the leader and reset leader 
                else
                {
                    ResetLeader();
                }

                // recalculate 
                StopCoroutine("MoveSwarm");
                _findingTarget = false;

                if (_path != null)
                    _path.Clear();

                Debug.Log(agents[_leaderIndex].gameObject.name);

                // get current position of player inside the map
                _playerPosition = player.transform.position;

                while (_path.Count == 0)
                    GetPath(agents[_leaderIndex].transform.position, player.transform.position, true);
                StartCoroutine("MoveSwarm");
            }
            yield return null;
        }

}

    bool HasMoved(Vector3 initial, Vector3 current, float distanceMoved)
    {
        if ((current - initial).sqrMagnitude > (distanceMoved * distanceMoved))
            return true;
        else
            return false;
    }

    IEnumerator Attack()
    {
        _findingTarget = false;
        _attackingPlayer = true;

        // store initial and final positions 
        List<Vector3> initialPositions = new List<Vector3>(agents.Count);
        List<Vector3> finalPositions = new List<Vector3>(agents.Count);

        for (int i = 0; i < agents.Count; i++)
        {
            if (agents[i].gameObject.activeSelf == true)
            {
                Quaternion targetRotation = Quaternion.LookRotation(player.transform.position - agents[i].transform.position);
                Quaternion rotationAction = Quaternion.Lerp(agents[i].transform.rotation, targetRotation,
                    Time.deltaTime * angularSpeed);
                agents[i].transform.rotation = rotationAction;

                initialPositions.Insert(i, agents[i].transform.position);
                Vector3 finalPosition = agents[i].transform.position + agents[i].transform.forward * 10.0f;
                finalPosition.y = agents[i].transform.position.y;
                finalPositions.Insert(i, finalPosition);
            }
        }

        _attackingElapsedTime = 0.0f;

        while (_attackingElapsedTime < attackDuration)
        {
            for (int i = 0; i < agents.Count; i++)
            {
                if (agents[i].gameObject.activeSelf == true)
                {
                    agents[i].transform.position = Vector3.Lerp(initialPositions[i], finalPositions[i],
                        (_attackingElapsedTime / attackDuration));   
                }
            }

            _attackingElapsedTime += Time.deltaTime;
            Debug.Log(_attackingElapsedTime);
            yield return null;
        }

        _attackingPlayer = false;
        
        yield return new WaitForSecondsRealtime(2.50f);
    }

    IEnumerator MoveSwarm()
    {
        _findingTarget = true;
        _attackingPlayer = false;

        bool destinationReached = false;
        int currentWaypointIndex = 1;

        Vector3 temp = _path[currentWaypointIndex];
        temp.y = agents[_leaderIndex].transform.position.y; ;

        agents[_leaderIndex].transform.LookAt(temp);

        while (!destinationReached)
        {
            // while turn point has not been reached 
            Vector2 currentPosition = new Vector2(agents[_leaderIndex].transform.position.x, agents[_leaderIndex].transform.position.z);
            while (pathManager.PassedTurningPoint(_path, currentWaypointIndex, currentPosition))
            {
                if (currentWaypointIndex == _path.Count - 1)
                {
                    destinationReached = true;
                    break;
                }
                else
                    currentWaypointIndex++;
            }

            // turn and translate agent 
            if (!destinationReached)
            {
                temp = _path[currentWaypointIndex];
                temp.y = agents[_leaderIndex].transform.position.y;
                Quaternion targetRotation = Quaternion.LookRotation(temp - agents[_leaderIndex].transform.position);

                Quaternion rotationAction = Quaternion.Lerp(agents[_leaderIndex].transform.rotation, targetRotation,
                    Time.deltaTime * angularSpeed);
                agents[_leaderIndex].transform.rotation = rotationAction;
                Vector3 agentMovement = new Vector3(0.0f, 0.0f, 1.0f) * speed * Time.deltaTime;
                agents[_leaderIndex].transform.Translate(agentMovement, Space.Self);

                // make leader avoid minotaur if in range 
                Vector3 modifiedVel;
                Vector3 targetL = temp;
                CalculateAgentMovement(agents[_leaderIndex], targetL, true, out modifiedVel);
                agents[_leaderIndex].Move(modifiedVel*0.75f);
                
                Vector3 target = agents[_leaderIndex].transform.position;

                // update other agents
                Vector3 outputVel;
                foreach (SwarmAgent agent in agents)
                {
                    if (agent != agents[_leaderIndex])
                    {
                        CalculateAgentMovement(agent, target, false, out outputVel);
                        agent.Move(outputVel);
                    }
                }
            }
            yield return null;
        }
        _findingTarget = false;
    }

    void ColourTiles(List<Vector3> path)
    {
        // colour the tiles of the path 
        for (int i = 0; i < path.Count; i++)
        {
            GameObject tile;
            Vector3 waypoint = path[i];

            int x, z;
            pathManager.mapManager.ConvertCartesianToCell(waypoint.x, waypoint.z, out x, out z);

            string tileObject = "/Map/FloorSection " + x.ToString() + "," + z.ToString();
            tile = GameObject.Find(tileObject);

            MeshRenderer rend = tile.GetComponent<MeshRenderer>();
            rend.material = material;
        }
    }

    [System.Serializable]
    public class SwarmBehaviour
    {
        private Vector3 _velocity;
        private float _smoothTime;

        public SwarmBehaviour(bool smoothCohesion, float smoothTime)
        {
            if (smoothCohesion)
                this._smoothTime = smoothTime;
            else
                this._smoothTime = 0.0f;
        }

        // inspired from https://github.com/boardtobits/flocking-algorithm
        public Vector3 CalculateMove(SwarmAgent agent,
            List<Transform> everythingInRange, float[] behaviourWeights, Swarm swarm)
        {
            LayerMask mask = LayerMask.GetMask("Swarm Particle");
            LayerMask wallMask = LayerMask.GetMask("Obstacle");
            LayerMask minotaurMask = LayerMask.GetMask("Minotaur");

            int agentsInRange = 0;

            if (behaviourWeights.Length != 3)
            {
                Debug.LogError("There should be 3 weights for each behaviour: cohesion, " +
                    "alignment, and avoidance.");
                return Vector3.zero;
            }

            Vector3 finalMovement = Vector3.zero;

            Vector3 cohesion = Vector3.zero;
            Vector3 alignment = Vector3.zero;
            Vector3 avoidance = Vector3.zero;

            if (everythingInRange.Count == 0)
            {
                alignment = agent.transform.forward;
            }

            int avoidanceCounter = 0;
            int wallCounter = 0;

            foreach (Transform t in everythingInRange)
            {
                if (mask == (mask | 1 << t.gameObject.layer))
                {                    
                    cohesion += (t.position);
                    alignment += t.transform.forward;
                   
                    agentsInRange++;
                }

                Vector3 difference = t.position - agent.transform.position;

                if (difference.sqrMagnitude < swarm.AvoidanceRadiusSquared)
                {
                        avoidanceCounter++;
                        avoidance += agent.transform.position - t.position;
                }       
            }

            // avoidance
            if (avoidanceCounter > 0)
                avoidance /= avoidanceCounter;

            // alignment
            if (agentsInRange > 0)
                alignment /= agentsInRange;

            // cohesion  
            if (agentsInRange > 0)
            {
                cohesion /= agentsInRange;
                cohesion -= agent.transform.position;
                cohesion = Vector3.SmoothDamp(agent.transform.forward, cohesion, ref _velocity, _smoothTime);
            }

            // put individual movements in vector
            Vector3[] behaviourMovements = new[] { cohesion, alignment, avoidance };            

            for (int i = 0; i < behaviourMovements.Length; i++)
            {
                Vector3 individualMovement = behaviourMovements[i];
                if (individualMovement != Vector3.zero)
                {
                    if (individualMovement.sqrMagnitude >
                        (behaviourWeights[i] * behaviourWeights[i]))
                    {
                        individualMovement.Normalize();
                        individualMovement *= behaviourWeights[i];
                    }
                }
                finalMovement += individualMovement;
            }
            finalMovement.y = 0.0f;
            return finalMovement;
        }
    }

}
