using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererController : MonoBehaviour {

    SteamVR_ControllerManager player;
    [SerializeField] GameObject Pointer; //移動位置のTarget
    [SerializeField] float GroundAngle = 30.0f; //角度

    [SerializeField] float initialVelocity = 10.0f; //力
    [SerializeField] float timeResolution = 0.02f;  //点と点の距離
    [SerializeField] float MaxTime = 10.0f;  //線の長さ
    [SerializeField] Vector3 PositionDiff;
    [SerializeField] LayerMask layerMask = -1;
    [SerializeField] GameObject ControllerRatation;

    bool ProjectileColor_judge = false; //放物線の色判断
    bool TargetSetActive = false;
    bool GroundAngle_judge = false; //地形角度の判断
    bool havePointer = false;
    bool isWarpInput = false;

    Vector3 Point;

    private GameObject PointerInstance;
    private LineRenderer lineRenderer;
    private SteamVR_TrackedObject TrackedObject;

    public Vector3 TargetPoint { get { return Point; } }
    public bool IsWarpInput { get { return isWarpInput; } }


    void Start() {
        lineRenderer = GetComponent<LineRenderer>();
        player = GameObject.FindObjectOfType<SteamVR_ControllerManager>( );
        TrackedObject = player.right.GetComponent<SteamVR_TrackedObject>( );
    }

    void Update() {
        //update毎にリセットする物はここに書く
        ResetState();
        Vector3 postion = (PositionDiff.magnitude * TrackedObject.transform.forward.normalized ) + TrackedObject.transform.position;

        //VRコントローラの処理
        var device = SteamVR_Controller.Input((int)TrackedObject.index);

        //放物線とTargetの処理
        int index = 0;

        Vector3 velocityVector = TrackedObject.transform.forward * initialVelocity;
        Vector3 currentPosition = postion;

        if (!havePointer) {
            PointerInstance = Instantiate( Pointer, Point, Quaternion.identity);
            havePointer = true;
        }

        PointerInstance.transform.position = Point;

        lineRenderer.positionCount = (int)(MaxTime / timeResolution);

        currentPosition.y = postion.y - 0.01f;

        for ( float t = 0.0f; t < MaxTime; t += timeResolution ) {

            if ( index < lineRenderer.positionCount ) {
                lineRenderer.SetPosition( index, currentPosition );
            }

            RaycastHit hit;

            if ( Physics.Raycast( currentPosition, velocityVector, out hit, velocityVector.magnitude * timeResolution, layerMask ) ) {

                lineRenderer.positionCount = index + 2;

                lineRenderer.SetPosition( index + 1, hit.point );

                //VRコントローラの処理
                if ( device.GetTouchDown( SteamVR_Controller.ButtonMask.Trigger ) && ProjectileColor_judge == false ) {
                    player.transform.position = hit.point;
                    isWarpInput = true;
                }

                //角度の判断
                PointerInstance.transform.rotation = Quaternion.LookRotation( hit.normal );
                if ( Vector3.Angle( hit.normal, Vector3.up ) >= GroundAngle ) {
                    GroundAngle_judge = true;
                } else {
                    GroundAngle_judge = false;
                }

                Point = hit.point;
                Point.y = hit.point.y + 0.05f;

                TargetSetActive = false;
                break;

            } else {
                TargetSetActive = true;
            }

            //物体を投げるの放物線重力シミュレーション
            currentPosition += velocityVector * timeResolution;
            velocityVector += Physics.gravity * timeResolution;
            if ( index >= 15 ) {
                velocityVector *= 0.9f;
            }
            index++;
            //Debug.Log( velocityVector );
            if ( index >= ( int )( MaxTime / timeResolution ) ) {
                index -= 2;
            }

        }

        //Targetの判断
        ProjectileColor_judge = ColliderTag(Point);

    }

    private void ResetState() {
        isWarpInput = false;
    }

    private bool ColliderTag(Vector3 point) {
        Ray ray = new Ray(point, Vector3.down);
        RaycastHit hit2;
        if (Physics.Raycast(ray, out hit2)) {
            if (hit2.distance > 1f || TargetSetActive == true || GroundAngle_judge == true) {
                PointerInstance.SetActive(false);
                return true;
            } else {
                PointerInstance.SetActive(true);
                return false;
            }
        }
        return false;
    }

    public void DeleteLine() {
        lineRenderer.positionCount = 0;
    }

    public void DeleteTarget() {
        Destroy(PointerInstance);
    }

}
