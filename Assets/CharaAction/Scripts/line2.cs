using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class line2 : MonoBehaviour {

    public GameObject player;
    public GameObject Pointer; //移動位置のTarget
    public float GroundAngle = 30.0f; //角度

    public float initialVelocity = 10.0f;
    public float timeResolution = 0.02f;
    public float MaxTime = 10.0f;

    public LayerMask layerMask = -1;

    bool ProjectileColor_judge = false; //放物線の色判断
    bool NG2 = false;
    bool GroundAngle_judge = false; //地形角度の判断
    bool havePointer = false;

    Vector3 Point;

    private GameObject PointerInstance;

    private LineRenderer lineRenderer;

    void Start () {
        lineRenderer = GetComponent<LineRenderer>( );
	}
	
	void Update () {

        //VRコントローラの処理
        SteamVR_TrackedObject trackedObject = GetComponent<SteamVR_TrackedObject>( );
        var device = SteamVR_Controller.Input( ( int )trackedObject.index );

        //放物線とTargetの処理
        int index = 0;

        Vector3 velocityVector = transform.forward * initialVelocity;
        Vector3 currentPosition = transform.position;

        if ( !havePointer ) {
            PointerInstance = Instantiate( Pointer, Point, Quaternion.Euler( -90, 0, 0 ) );
            havePointer = true;
        }

        PointerInstance.transform.position = Point;

        lineRenderer.SetVertexCount( ( int ) ( MaxTime / timeResolution ) );
        
        currentPosition.y = transform.position.y - 0.01f;

        for ( float t = 0.0f; t < MaxTime; t += timeResolution ) {

            lineRenderer.SetPosition( index, currentPosition);

            RaycastHit hit;

            if ( Physics.Raycast( currentPosition, velocityVector, out hit, velocityVector.magnitude, layerMask ) ) {

                lineRenderer.SetVertexCount( index + 2);

                lineRenderer.SetPosition( index + 1, hit.point);

                //VRコントローラの処理
                if ( device.GetTouchDown( SteamVR_Controller.ButtonMask.Trigger ) && ProjectileColor_judge == false ) {
                    player.transform.position = hit.point;
                }

                //角度の判断
                PointerInstance.transform.rotation = Quaternion.LookRotation( hit.normal );
                if ( Vector3.Angle( hit.normal, Vector3.up ) >= GroundAngle ) {
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
        }

        //放物線の色とTargetの表示
        if ( ProjectileColor_judge == true ) {
            GetComponent<Renderer>( ).material.color = Color.red;
            PointerInstance.SetActive( false );
        } else {
            GetComponent<Renderer>( ).material.color = Color.green;
            PointerInstance.SetActive( true );
        }

        //Tagの判断
        Ray ray = new Ray(Point, Vector3.down);
        RaycastHit hit2;
        if (Physics.Raycast( ray, out hit2 ) ) {
            Debug.Log( hit2.point );
            Debug.DrawLine( Point, hit2.point, Color.red );
            if ( hit2.distance > 1f || hit2.collider.tag == "unstand" || NG2 == true || GroundAngle_judge == true ) {
                ProjectileColor_judge = true;
            } else {
                ProjectileColor_judge = false;
            }
        }
    }
}
