using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    public float mouseSensitivity = 10.0f;
    public Transform CameraBase, Player;
    float rotX, rotY;
    float distance;
    private float _initialMagnitude;
    private Vector3 _initialDir;
    LayerMask mask;

    void Awake()
    {
        // initialization 
        _initialMagnitude = (transform.position - CameraBase.transform.position).magnitude;
        _initialDir = transform.localPosition.normalized;
    }

    // Start is called before the first frame update
    void Start()
    {
        // gets the rotation of the camera wrt parent 
        Vector3 rot = transform.localRotation.eulerAngles;
        rotX = rot.x;
        rotY = rot.y;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        mask = 1 << LayerMask.NameToLayer("Obstacle");
    }

    void Update()
    {
        // updates rotations based on input and clamps the pitch
        rotX += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotY = Mathf.Clamp(rotY, -30, -10);
    }

    void LateUpdate()
    {
        RotateCameraAndPlayer();
        CameraOcclusionCollisionAvoidance();
    }

    void RotateCameraAndPlayer()
    {
        // camera looks at parent, rotates parent and player 
        transform.LookAt(CameraBase);
        CameraBase.rotation = Quaternion.Euler(rotY, rotX, 0.0f);
        Player.rotation = Quaternion.Euler(0.0f, rotX, 0.0f);
    }

    void CameraOcclusionCollisionAvoidance()
    {
        // raycast from camera base to camera 
        RaycastHit hit;

        if (Physics.Raycast(CameraBase.transform.position, (transform.position - CameraBase.transform.position), out hit, 15.0f, mask))
        {
            distance = Mathf.Clamp(hit.distance * 0.9f, 1.0f, _initialMagnitude);
        }
        else
        {
            distance = _initialMagnitude;
        }
        //  based on hit distance, camera position is changed 
        transform.localPosition = Vector3.Lerp(transform.localPosition, _initialDir * distance, Time.deltaTime * 10.0f);
       
    }
}
