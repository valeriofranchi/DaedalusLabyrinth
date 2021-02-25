using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minotaur : MonoBehaviour
{
    [Header("Rigidbody physics")]   
    public float speed = 7.50f;
    public float angularSpeed = 0.5f;
    public float attackingSpeed = 15.0f;
    public float attackKnockBackForce = 0.5f;

    public Collider AgentCollider { get { return _agentCollider; } set { _agentCollider = value; } }
    public Vector3 Velocity { get { return _velocity; } set { _velocity = value; } }

    private Vector3 _velocity;
    private Collider _agentCollider;

    [Header("Path Planning")]
    public PathManager pathManager;    
    public float trackingRange = 10.0f;
    public float attackingRange = 2.0f;
    public GameObject player;
    public float distanceMovedThreshold = 5.0f;
    public Material material;

    public List<Vector3> Path { get { return _path; } set { _path = value; } }

    private List<Vector3> _path;
    private bool _trackingPlayer = false;
    private bool _attackPlayer = false;
    private float _trackingRangeSquared;
    private float _attackingRangeSquared;
    private float _distanceMovedThresholdSquared;
    private bool _followingWaypoints = false;
    private Vector3 _playerPosition;

    // Start is called before the first frame update
    void Start()
    {
        // initialization of variables 
        gameObject.layer = LayerMask.NameToLayer("Minotaur");

        gameObject.tag = "Minotaur";

        _path = new List<Vector3>();

        _trackingRangeSquared = trackingRange * trackingRange;
        _attackingRangeSquared = attackingRange * attackingRange;
        _distanceMovedThresholdSquared = distanceMovedThreshold * distanceMovedThreshold;

        StartCoroutine("LocateAttack");
        Debug.Log("started");
    }

    // Update is called once per frame
    void Update()
    {
    }

    void GetPath(Vector3 destination)
    {
        // retrieves and filters path 
        List<Vector3> generatedPath;
        int size;

        pathManager.findPath((Vector3)transform.position, destination, out generatedPath, out size);

        pathManager.FilterPath(generatedPath, size, out _path);

        //ColourTiles(Path);
    }

    void GetRandomPosition(out Vector3 generatedPosition)
    {
        // choose random point in map
        bool foundTargetPosition = false;
        int targetX = 0;
        int targetY = 0;

        while (!foundTargetPosition)
        {
            targetX = Random.Range(0, pathManager.Map[0].Length);
            targetY = Random.Range(0, pathManager.Map.Length);

            if (pathManager.Map[targetY][targetX] == 0)
                foundTargetPosition = true;
        }

        float targetXCartesian = 0.0f;
        float targetZCartesian = 0.0f;
        pathManager.mapManager.ConvertCellToCartesian(targetX, targetY, out targetXCartesian, out targetZCartesian);
        generatedPosition = new Vector3(targetXCartesian, 0.0f, targetZCartesian);
    }

    IEnumerator LocateAttack()
    {
        // check the tracking process
        StartCoroutine("TrackingPlayer");

        while (true)
        {
            // if not tracking player 
            if (!_trackingPlayer)
            {
                Debug.Log("Random position");               

                // choose random point in map
                if (!_followingWaypoints)
                {
                    yield return new WaitForSecondsRealtime(2);

                    if (_path != null)
                        _path.Clear();

                    Vector3 destination;
                    GetRandomPosition(out destination);

                    // go to it 
                    while (_path.Count == 0)
                    {
                        GetPath(destination);
                    }

                    StartCoroutine("FollowWaypoints");
                }
            }
            else
            {
                // if tracking player 
                Debug.Log("Tracking");

                Debug.Log("Go towards player");
                // if player in tracking range, go to him
                if (!_followingWaypoints)
                {
                    yield return new WaitForSecondsRealtime(2);

                    if (_path != null)
                        _path.Clear();

                    _playerPosition = player.transform.position;
                    GetPath(new Vector3(player.transform.position.x, 0.0f, player.transform.position.z));
                    StartCoroutine("FollowWaypoints");
                }

                // update path if player has moved by a certain amount 
                Vector3 difference = player.transform.position - _playerPosition;
                if (difference.sqrMagnitude > distanceMovedThreshold)
                {
                    Debug.Log("Recalculating");
                    if (_followingWaypoints)
                    {
                        StopCoroutine("FollowWaypoints");
                        if (_path != null)
                            _path.Clear();
                    }

                    _playerPosition = player.transform.position;
                    while (_path.Count == 0)
                        GetPath(new Vector3(player.transform.position.x, 0.0f, player.transform.position.z));
                    StartCoroutine("FollowWaypoints");
                }
            }  
            yield return null;
        }
    }

    IEnumerator TrackingPlayer()
    {
        // updates tracking status 
        while (true)
        {            
            // check if player inside tracking range 
            float distance = (player.transform.position - transform.position).sqrMagnitude;
            if (distance < _trackingRangeSquared)
            {
                _trackingPlayer = true;

                float timer = 0.0f;
                while ((player.transform.position - transform.position).sqrMagnitude > _trackingRangeSquared)
                {
                    timer += Time.deltaTime;

                    if (timer > 10.0f)
                    {
                        _trackingPlayer = false;
                        break;
                    }
                    yield return null;
                }

            }
            yield return null;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        // apply rigid body physics to player upon collision 
        if (collider.gameObject.tag == "Player")
        {
            Vector3 dir = transform.position - collider.transform.position;
            player.GetComponent<Rigidbody>().AddForce(dir.normalized * -100.0f, ForceMode.Impulse);

            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    // pathfinding implemented by myself, path smoothing and its integration with inspired by https://github.com/SebLague/Pathfinding 
    IEnumerator FollowWaypoints()
    {
        bool destinationReached = false;
        int currentWaypointIndex = 1;
        _followingWaypoints = true;

        Vector3 temp = _path[currentWaypointIndex];
        temp.y = transform.position.y; ;

        transform.LookAt(temp);

        while (!destinationReached)
        {
            // while turn point has not been reached 
            Vector2 currentPosition = new Vector2(transform.position.x, transform.position.z);
            Debug.Log(_path.Count);
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
                temp.y = transform.position.y; 
                Quaternion targetRotation = Quaternion.LookRotation(temp - transform.position);
                Debug.Log(targetRotation.eulerAngles);
                
                Quaternion rotationAction = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * angularSpeed);
                transform.rotation = rotationAction;
                Vector3 agentMovement = new Vector3(0.0f, 0.0f, 1.0f) * speed * Time.deltaTime;
                transform.Translate(agentMovement, Space.Self);
            }
            yield return null;
        }
        _followingWaypoints = false;
    }

    void ColourTiles(List<Vector3> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            GameObject tile;
            Vector3 waypoint = path[i];

            int x, z;
            pathManager.mapManager.ConvertCartesianToCell(waypoint.x, waypoint.z, out x, out z);

            string tileObject = "/Map/FloorSection " + x.ToString() + "," + z.ToString();

            Debug.Log(tileObject);

            tile = GameObject.Find(tileObject);

            if (tile == null)
            {
                Debug.Log(tileObject);
            }
            else
            {
                ;
            }

            MeshRenderer rend = tile.GetComponent<MeshRenderer>();
            rend.material = material;
        }
    }
}
