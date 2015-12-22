using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CameraControl : MonoBehaviour {
    public static CameraControl Instance;

    public Transform TargetLookAt;
    public float Distance = 5f;
    public float DistanceMin = 3f;
    public float DistanceMax = 10f;
    public float DistanceSmooth = 0.05f;
    public float DistanceResumeSmooth = 1f;
    public float XMouseSensitivity = 5f;
    public float YMouseSensitivity = 5f;
    public float MouseWheelSensitivity = 5f;
    public float XSmooth = 0.05f;
    public float YSmooth = 0.1f;
    public float YMinLimit = -40f;
    public float YMaxLimit = 80f;
    public float OcclusionDistanceStep = 0.5f;
    public int MaxOcclusionChecks = 10;

    private float _mouseX ;
    private float _mouseY;
    private float _velX;
    private float _velY;
    private float _velZ;
    private float _velDistance;
    private float _startDistance;
    private Vector3 _position = Vector3.zero;
    private Vector3 _desiredPosition = Vector3.zero;
    private float _desiredDistance;
    private float _distanceSmooth;
    private float _preOccludedDistance;
    private float _nearClipPlane;

    void Awake() {
        Instance = this;
    }

    void Start() {
        Distance = Mathf.Clamp(Distance, DistanceMin, DistanceMax);
        _startDistance = Distance;
        _nearClipPlane = GetComponent<Camera>().nearClipPlane;
        Reset();
    }
    void LateUpdate() {
        if(TargetLookAt == null)
            return;

        HandlePlayerInput();

        var count = 0;
        do {
            CalculateDesiredPosition();
            count++;
        } while(CheckIfOccluded(count));

        UpdatePosition();
    }

    void HandlePlayerInput() {
        const float deadZone = 0.01f;

        _mouseX += Input.GetAxis("Mouse X") * XMouseSensitivity;
        _mouseY -= Input.GetAxis("Mouse Y") * YMouseSensitivity;


        _mouseY = Helper.ClampAngle(_mouseY, YMinLimit, YMaxLimit);

        if(Input.GetAxis("Mouse ScrollWheel") < -deadZone || Input.GetAxis("Mouse ScrollWheel") > deadZone) {
            _desiredDistance = Mathf.Clamp(Distance - Input.GetAxis("Mouse ScrollWheel") * MouseWheelSensitivity,
                DistanceMin, DistanceMax);

            _preOccludedDistance = _desiredDistance;
            _distanceSmooth = DistanceSmooth;
        }
    }

    void CalculateDesiredPosition() {
        ResetDesiredDistance();
        Distance = Mathf.SmoothDamp(Distance, _desiredDistance, ref _velDistance, _distanceSmooth);
        _desiredPosition = CalculatePosition(_mouseY, _mouseX, Distance);

    }

    Vector3 CalculatePosition(float rotationX, float rotationY, float distance) {
        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
        return TargetLookAt.position + rotation * direction;
    }

    bool CheckIfOccluded(int count) {
        var isOccluded = false;

        var nearestDistance = CheckCameraPoints(TargetLookAt.position, _desiredPosition);

        if(nearestDistance != -1) {
            if(count < MaxOcclusionChecks) {
                isOccluded = true;
                Distance -= OcclusionDistanceStep;

                if(Distance < 0.25f)
                    Distance = 0.25f;
            } else
                Distance = nearestDistance - _nearClipPlane;

            _desiredDistance = Distance;
            _distanceSmooth = DistanceResumeSmooth;
        }

        return isOccluded;
    }

    float CheckCameraPoints(Vector3 from, Vector3 to) {
        var nearestDistance = -1f;

        RaycastHit hitInfo;

        Helper.ClipPlanePoints clipPlanePoints = Helper.ClipPlaneAtNear(to);

        Debug.DrawLine(from, to + transform.forward * -_nearClipPlane, Color.red);
        Debug.DrawLine(from, clipPlanePoints.UpperLeft);
        Debug.DrawLine(from, clipPlanePoints.UpperRight);
        Debug.DrawLine(from, clipPlanePoints.LowerLeft);
        Debug.DrawLine(from, clipPlanePoints.LowerRight);

        Debug.DrawLine(clipPlanePoints.UpperLeft, clipPlanePoints.UpperRight);
        Debug.DrawLine(clipPlanePoints.UpperRight, clipPlanePoints.LowerRight);
        Debug.DrawLine(clipPlanePoints.LowerRight, clipPlanePoints.LowerLeft);
        Debug.DrawLine(clipPlanePoints.LowerLeft, clipPlanePoints.UpperLeft);

        if(Physics.Linecast(from, clipPlanePoints.UpperLeft, out hitInfo) && !hitInfo.collider.CompareTag("Player"))
            nearestDistance = hitInfo.distance;

        if(Physics.Linecast(from, clipPlanePoints.LowerLeft, out hitInfo) && !hitInfo.collider.CompareTag("Player"))
            if(hitInfo.distance < nearestDistance || nearestDistance == -1)
                nearestDistance = hitInfo.distance;

        if(Physics.Linecast(from, clipPlanePoints.UpperRight, out hitInfo) && !hitInfo.collider.CompareTag("Player"))
            if(hitInfo.distance < nearestDistance || nearestDistance == -1)
                nearestDistance = hitInfo.distance;
        if(Physics.Linecast(from, clipPlanePoints.LowerRight, out hitInfo) && !hitInfo.collider.CompareTag("Player"))
            if(hitInfo.distance < nearestDistance || nearestDistance == -1)
                nearestDistance = hitInfo.distance;

        if(Physics.Linecast(from, to + transform.forward * -_nearClipPlane, out hitInfo) && !hitInfo.collider.CompareTag("Player"))
            if(hitInfo.distance < nearestDistance || nearestDistance == -1)
                nearestDistance = hitInfo.distance;

        return nearestDistance;
    }

    void ResetDesiredDistance() {
        if(_desiredDistance < _preOccludedDistance) {
            var pos = CalculatePosition(_mouseY, _mouseX, _preOccludedDistance);

            var nearestDistance = CheckCameraPoints(TargetLookAt.position, pos);

            if(nearestDistance == -1 || nearestDistance > _preOccludedDistance) {
                _desiredDistance = _preOccludedDistance;
            }
        }
    }

    void UpdatePosition() {

        var posX = Mathf.SmoothDamp(_position.x, _desiredPosition.x, ref _velX, XSmooth);
        var posY = Mathf.SmoothDamp(_position.y, _desiredPosition.y, ref _velY, YSmooth);
        var posZ = Mathf.SmoothDamp(_position.z, _desiredPosition.z, ref _velZ, XSmooth);
        _position = new Vector3(posX, posY, posZ);

        transform.position = _position;

        transform.LookAt(TargetLookAt);

    }

    public void Reset() {
        _mouseX = 0;
        _mouseY = 10;
        Distance = _startDistance;
        _desiredDistance = Distance;
        _preOccludedDistance = Distance;
    }

    public static void UseExistingOnCreateNewMainCamera() {
        GameObject tempCamera;

        if(Camera.main != null) {
            tempCamera = Camera.main.gameObject;
        } else {
            tempCamera = new GameObject("Main Camera");
            tempCamera.AddComponent<Camera>();
            tempCamera.tag = "MainCamera";
        }

        tempCamera.AddComponent<CameraControl>();
        var myCamera = tempCamera.GetComponent<CameraControl>();

        var targetLookAt = GameObject.Find("targetLookAt");

        if(targetLookAt == null) {
            targetLookAt = new GameObject("targetLookAt");
            targetLookAt.transform.position = Vector3.zero;
        }

        myCamera.TargetLookAt = targetLookAt.transform;
    }
}
