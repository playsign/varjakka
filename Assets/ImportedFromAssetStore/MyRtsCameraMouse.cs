using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Encapsulates mouse movement for RtsCamera.
/// </summary>
[AddComponentMenu("Camera-Control/RtsCamera-Mouse")]
public class MyRtsCameraMouse : MonoBehaviour
{
    // public KeyCode MouseOrbitButton;

    public bool AllowScreenEdgeMove;
    public bool ScreenEdgeMoveBreaksFollow;
    public int ScreenEdgeBorderWidth;
    public float MoveSpeed;

    public bool AllowPan;
    public bool PanBreaksFollow;
    public float PanSpeed, panSpeedTouch;

    public bool AllowRotate;
    public float RotateSpeed, rotateSpeedTouch;

    public bool AllowTilt;
    public float TiltSpeed, tiltSpeedTouch;

    public bool AllowZoom;
    public float ZoomSpeed, zoomSpeedTouch;

    //public SelectionManager scm;

    //because the 3D cam pos is stored & handled here for the 2D button (from before)
    public Text GUIText2D3D;

    float birdEyeDistance = 1250f;
    Vector3 birdEyeTarget = new Vector3(1383.492f, 55, 389.2245f);

    float birdEyeRotation = -208.3843f;
    float birdEyeTilt = 85;

    float lastDistance, lastRotation, lastTilt;
    Vector3 lastTarget;
    bool lastSettingsSaved = false;

    Vector3 initPos;

    // public string RotateInputAxis = "Mouse X";
    // public string TiltInputAxis = "Mouse Y";
    public string ZoomInputAxis = "Mouse ScrollWheel";
    // public KeyCode PanKey1 = KeyCode.LeftShift;
    // public KeyCode PanKey2 = KeyCode.RightShift;

    //

    private RtsCamera _rtsCamera;

    //

    Vector3 previousMousePosition;

    protected void Reset()
    {
        // MouseOrbitButton = KeyCode.Mouse2;    // middle mouse by default (probably should not use right mouse since it doesn't work well in browsers)

        AllowScreenEdgeMove = true;
        ScreenEdgeMoveBreaksFollow = true;
        ScreenEdgeBorderWidth = 4;
        MoveSpeed = 30f;

        AllowPan = true;
        PanBreaksFollow = true;
        PanSpeed = 50f;
        // PanKey1 = KeyCode.LeftShift;
        // PanKey2 = KeyCode.RightShift;

        AllowRotate = true;
        RotateSpeed = 360f;

        AllowTilt = true;
        TiltSpeed = 200f;

        AllowZoom = true;
        ZoomSpeed = 500f;

        // RotateInputAxis = "Mouse X";
        // TiltInputAxis = "Mouse Y";
        ZoomInputAxis = "Mouse ScrollWheel";
    }

    protected void Start()
    {

        _rtsCamera = gameObject.GetComponent<RtsCamera>();

        if (!Input.mousePresent)
        {

            PanSpeed = panSpeedTouch;
            RotateSpeed = rotateSpeedTouch;
            TiltSpeed = tiltSpeedTouch;
            ZoomSpeed = zoomSpeedTouch;

        }

    }

    void PinchZoom()
    {

        // Store both touches.
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        // Find the position in the previous frame of each touch.
        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        // Find the magnitude of the vector (the distance) between the touches in each frame.
        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        // Find the difference in the distances between each frame.
        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        _rtsCamera.Distance += deltaMagnitudeDiff * ZoomSpeed * Time.deltaTime;

    }

    protected void Update()
    {
        if (false) ; //scm.selectionMode != SelectionManager.SelectionMode.FreeMode) AllowPan = false;
        else AllowPan = true;
        
        if (_rtsCamera == null)
            return; // no camera, bail!

        if (AllowZoom)
        {
            var scroll = Input.GetAxisRaw(ZoomInputAxis);
            _rtsCamera.Distance -= scroll * ZoomSpeed * Time.deltaTime;
        }

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            previousMousePosition = Input.mousePosition;

        if (Input.GetMouseButton(1))
        {

            if (AreFingersMovingInOppositeDirections())
                PinchZoom();
            else
                Orbit();

        }
        else if (AllowPan && Input.GetMouseButton(0))// && !EventSystem.current.currentSelectedGameObject)
        {
            initPos = Input.mousePosition;
            Pan();
        }

        if (AllowScreenEdgeMove && (!_rtsCamera.IsFollowing || ScreenEdgeMoveBreaksFollow))
        {
            var hasMovement = false;

            if (Input.mousePosition.y > (Screen.height - ScreenEdgeBorderWidth))
            {
                hasMovement = true;
                _rtsCamera.AddToPosition(0, 0, MoveSpeed * Time.deltaTime);
            }
            else if (Input.mousePosition.y < ScreenEdgeBorderWidth)
            {
                hasMovement = true;
                _rtsCamera.AddToPosition(0, 0, -1 * MoveSpeed * Time.deltaTime);
            }

            if (Input.mousePosition.x > (Screen.width - ScreenEdgeBorderWidth))
            {
                hasMovement = true;
                _rtsCamera.AddToPosition(MoveSpeed * Time.deltaTime, 0, 0);
            }
            else if (Input.mousePosition.x < ScreenEdgeBorderWidth)
            {
                hasMovement = true;
                _rtsCamera.AddToPosition(-1 * MoveSpeed * Time.deltaTime, 0, 0);
            }

            if (hasMovement && _rtsCamera.IsFollowing && ScreenEdgeMoveBreaksFollow)
            {
                _rtsCamera.EndFollow();
            }
        }

        if (Input.GetKeyUp(KeyCode.F1))
        {
            Toggle2D(); //now also from GUI button
        }

        //ESC. could/should be for many other things too (closing menus and such)
        //if (Input.GetKeyUp(KeyCode.Escape)) scm.areaEditPanel.transform.Find("PolygonEdit").GetComponent<Toggle>().isOn = false;
            
    }

    public void Toggle2D()
    {
        if (!lastSettingsSaved)
        {
            lastDistance = GetComponent<RtsCamera>().Distance;
            lastTarget = GetComponent<RtsCamera>().LookAt;
            lastTilt = GetComponent<RtsCamera>().Tilt;
            lastRotation = GetComponent<RtsCamera>().Rotation;
            lastSettingsSaved = true;

            GetComponent<RtsCamera>().Distance = birdEyeDistance;
            GetComponent<RtsCamera>().LookAt = birdEyeTarget;
            GetComponent<RtsCamera>().Tilt = birdEyeTilt;
            GetComponent<RtsCamera>().Rotation = birdEyeRotation;

            //TODO: would be nice to show this state in the GUI button!
            //.. this code tries to do that but fails, doesn't update to UI
            GUIText2D3D.text = "3D";
        }

        else
        {
            lastSettingsSaved = false;

            GetComponent<RtsCamera>().Distance = lastDistance;
            GetComponent<RtsCamera>().LookAt = lastTarget;
            GetComponent<RtsCamera>().Tilt = lastTilt;
            GetComponent<RtsCamera>().Rotation = lastRotation;

            GUIText2D3D.text = "2D";
        }
    }

    void Orbit()
    {

        Vector3 orbit = GetMousePositionChange();

        // orbit
        if (AllowTilt)
        {

            var tilt = orbit.y;
            _rtsCamera.Tilt -= tilt * TiltSpeed * Time.deltaTime;

        }

        if (AllowRotate)
        {

            var rot = orbit.x;
            _rtsCamera.Rotation += rot * RotateSpeed * Time.deltaTime;

        }

    }

    void Pan()
    {
        Vector3 pan = GetMousePositionChange();

        // pan
        var panX = -1 * pan.x * PanSpeed * Time.deltaTime;
        var panZ = -1 * pan.y * PanSpeed * Time.deltaTime;

        //Debug.LogFormat("MyRtsCameraMouse - Pan: {0} {1}", panX, panZ);

        _rtsCamera.AddToPosition(panX, 0, panZ);

        if (PanBreaksFollow && (Mathf.Abs(panX) > 0.001f || Mathf.Abs(panZ) > 0.001f))
        {
            _rtsCamera.EndFollow();
        }
    }

    Vector3 GetMousePositionChange()
    {

        Vector3 currentMousePosition = Input.mousePosition;
        Vector3 change = GetComponent<Camera>().ScreenToViewportPoint(currentMousePosition - previousMousePosition);

        if (change.magnitude > 0.005f)
        {

            //print(change.magnitude);
            //GetComponent<SelectionManager>().cameraPanned = true;

        }


        previousMousePosition = currentMousePosition;

        
        return change;

    }

    bool AreFingersMovingInOppositeDirections()
    {

        if (Input.touchCount == 2)
        {

            // fingers data
            Touch f0 = Input.GetTouch(0);
            Touch f1 = Input.GetTouch(1);

            // fingers movements
            Vector3 f0Delta = new Vector3(f0.deltaPosition.x, f0.deltaPosition.y, 0);
            Vector3 f1Delta = new Vector3(f1.deltaPosition.x, f1.deltaPosition.y, 0);

            // fingers moving direction
            Vector3 f0Dir = f0Delta.normalized;
            Vector3 f1Dir = f1Delta.normalized;

            // dot product of directions
            float dot = Vector3.Dot(f0Dir, f1Dir);

            if (dot < -0.7f)
                return true;
            else
                return false;

        }
        else
            return false;

    }

}
