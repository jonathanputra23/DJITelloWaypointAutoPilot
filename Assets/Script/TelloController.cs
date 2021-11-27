using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TelloLib;
using System.Linq;
using System;

public class TelloController : MonoBehaviour
{
    private UdpConnection connection;
    [HideInInspector]
    public MovePlayerDestination MPD;
    [HideInInspector]
    public telloState tState;
    private Vector3 rotation;
    public int speedSet=10;
    public int forwardSet = 50;
    public bool onSpeed;
    public bool setSpeed;
    public bool onTakeOff;
    public bool onForward;
    public bool onBack;
    public bool onLeft;
    public bool onRight;
    public bool onLand;
    public bool onEmergency;
    public bool onDown;
    private float angle;
    public int angleWhole;
    public bool onRotate;
    public int prevRot = 0;
    public bool onCalculatePos;

    private bool right=false;
    private float localRotY;
    //public float totX;
    //public int timeSince;
    void Start()
    {
        string sendIp = "192.168.10.1";
        int sendPort = 8889;
        int receivePort = 8889;

        connection = new UdpConnection();
        connection.StartConnection(sendIp, sendPort, receivePort);
        connection.Send("command");
        StartCoroutine(instantiateRotation());

    }

    void Update()
    {
        rotateTo();
        foreach (var message in connection.getMessages()) Debug.Log(message);
        if (onDown)
        {
            connection.Send("down 10");
            onDown = false;
        }
        if (onBack)
        {
            connection.Send("back " + forwardSet.ToString());
            onBack = false;
        }
        if (onTakeOff)
        {
            connection.Send("takeoff");
            onTakeOff = false;
        }
        if (onForward)
        {
            connection.Send("forward " + forwardSet.ToString());
            onForward = false;
        }
        if (onLeft)
        {
            connection.Send("left " + forwardSet.ToString());
            onLeft = false;
        }
        if (onRight)
        {
            connection.Send("right " + forwardSet.ToString());
            onRight = false;
        }
        if (onLand)
        {
            connection.Send("land");
            onLand = false;
        }
        if (onEmergency)
        {
            connection.Send("emergency");
            onEmergency = false;
        }
        if (onSpeed)
        {
            connection.Send("speed?");
            onSpeed = false;
        }
        if (setSpeed)
        {
            connection.Send("speed " + speedSet.ToString());
            setSpeed = false;
        }
        if (onRotate)
        {
            if (right)
            {
                connection.Send("cw " + (angleWhole).ToString());
            }
            else
            {
                connection.Send("ccw " + (angleWhole).ToString());
            }
            prevRot = angleWhole;
            onRotate = false;
        }
        if (onCalculatePos)
        {
            movePosition();
        }
        rotation = new Vector3(tState.roll, tState.yaw-localRotY, -tState.pitch);
        this.transform.eulerAngles = rotation;
    }

    IEnumerator instantiateRotation()
    {
        yield return new WaitForSeconds(1);
        localRotY = tState.yaw;
        Debug.Log(localRotY);
        Debug.Log(tState.yaw - localRotY);
    }
    void movePosition()
    {
        float tempx = transform.position.z + tState.sx2;
        float tempy = transform.position.x + tState.sy2;
        float tempz = transform.position.y + tState.sz2;
        //Vector3 waypointDir = wayPointTrans.position - transform.position;
        //Vector3 tempV = new Vector3(tempx, tempy, tempz);
        //Vector3 tempx = transform.forward * tState.sx * Time.deltaTime;
        //Vector3 tempy = transform.up * tState.sy * Time.deltaTime;
        //Vector3 tempz = transform.right * tState.sz * Time.deltaTime;
        //transform.Translate(this.transform.forward * tState.sx , Space.World);
        //transform.position += transform.forward * (tState.sx2) ;
        transform.position = new Vector3(tempy, 0, tempx);
        //transform.position += this.transform.right * tState.sz;
        //transform.position = new Vector3(tempx, tempy, tempz);
        //transform.Translate(tState.sx, tState.sy, tState.sz, Space.World);
        //transform.position = transform.TransformDirection(tempV);
        //transform.position = transform.TransformPoint(tempV);


    }
    void OnDestroy()
    {
        connection.Stop();
    }

    void rotateTo()
    {
        Vector3 direction = MPD.playerGoal.position - this.transform.position;
        Debug.DrawRay(this.transform.position, direction, Color.green);
        angle = Vector3.Angle(direction, transform.forward);
        angleWhole = (int)angle;
        Vector3 perp = Vector3.Cross(transform.forward, direction);
        float dir = Vector3.Dot(perp, transform.up);
        if (dir > 0f)
        {
            right = true;
        }
        else if (dir < 0f)
        {
            right = false;
        }
        else
        {
            right = true;
        }
        
        //angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg - 90;
        //angleWhole = (int)angle;
        //if (angleWhole < 0)
        //{
        //    System.Math.Abs(angleWhole);
        //    angleWhole += 180;
        //}
        //Debug.Log("Angle: " + angle);

        //Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.up);
        //transform.rotation = Quaternion.Slerp(transform.rotation, angleAxis, Time.deltaTime * 50);
    }
}
