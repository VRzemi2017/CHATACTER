using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class line2 : MonoBehaviour {

    [SerializeField] GameObject player;
    [SerializeField] GameObject Pointer; //移動位置のTarget
    [SerializeField] float GroundAngle = 30.0f; //角度

    [SerializeField] float initialVelocity = 10.0f;
    [SerializeField] float timeResolution = 0.02f;
    [SerializeField] float MaxTime = 10.0f;
    [SerializeField] float LineLegth = 5.0f;
    [SerializeField] Vector3 PositionDiff;
    [SerializeField] SteamVR_TrackedObject TrackedObject;

    [SerializeField] LayerMask layerMask = -1;

    bool ProjectileColor_judge = false; //放物線の色判断
    bool NG2 = false;
    bool GroundAngle_judge = false; //地形角度の判断
    bool havePointer = false;
    bool isWarpInput = false;

    Vector3 Point;

    private GameObject PointerInstance;

    private LineRenderer lineRenderer;

    public Vector3 TargetPoint { get { return Point; } }
    public bool IsWarpInput { get { return isWarpInput; } }


    void Start() {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update() {
        //update毎にリセットする物はここに書く
        ResetState();
        Vector3 postion = (PositionDiff.magnitude * TrackedObject.transform.forward.normalized) + TrackedObject.transform.position;

        //VRコントローラの処理
        var device = SteamVR_Controller.Input((int)TrackedObject.index);

        //放物線とTargetの処理
        int index = 0;

        Vector3 velocityVector = TrackedObject.transform.forward * initialVelocity;
        Vector3 currentPosition = postion;

        if (!havePointer) {
            PointerInstance = Instantiate(Pointer, Point, Quaternion.Euler(-90, 0, 0));
            havePointer = true;
        }

        PointerInstance.transform.position = Point;

        lineRenderer.SetVertexCount((int)(MaxTime / timeResolution));

        currentPosition.y = postion.y - 0.01f;

        float nowLineLength = 0.0f;
        for (float t = 0.0f; t < MaxTime; t += timeResolution) {

            lineRenderer.SetPosition(index, currentPosition);
           /* if (nowLineLength > LineLegth) {
                lineRenderer.SetVertexCount(index + 2);
                lineRenderer.SetPosition(index + 1, currentPosition);
                break;
            }*/
            RaycastHit hit;

            if (Physics.Raycast(currentPosition, velocityVector, out hit, velocityVector.magnitude * timeResolution, layerMask)) {

                lineRenderer.SetVertexCount(index + 2);

                lineRenderer.SetPosition(index + 1, hit.point);

                //VRコントローラの処理
                if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger) && ProjectileColor_judge == false) {
                    player.transform.position = hit.point;
                    isWarpInput = true;
                }

                //角度の判断
                PointerInstance.transform.rotation = Quaternion.LookRotation(hit.normal);
                if (Vector3.Angle(hit.normal, Vector3.up) >= GroundAngle) {
                    GroundAngle_judge = true;
                } else {
                    GroundAngle_judge = false;
                }

                Point = hit.point;
                Point.y = hit.point.y + 0.01f;

                NG2 = false;
                break;

            } else {
                NG2 = true;
            }

            //物体を投げるの放物線重力シミュレーション
            currentPosition += velocityVector * timeResolution;
            velocityVector += Physics.gravity * timeResolution;
            index++;
            //nowLineLength += ;


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
            Debug.Log(hit2.point);
            Debug.DrawLine(Point, hit2.point, Color.red);
            if (hit2.distance > 1f || hit2.collider.tag == "unstand" || NG2 == true || GroundAngle_judge == true) {
                PointerInstance.SetActive(false);
                return true;
            } else {
                PointerInstance.SetActive(true);
                return false;
            }
        }
        return false;
    }

}
