using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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


    public int commandSent = 0;
    public int responseRec = -1; //termasuk send command pertama

    public bool onAuto=false;
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
        commandSent += 1;
        StartCoroutine(instantiateRotation());

    }

    void Update()
    {
        rotateTo(MPD.guide.transform.position);
        foreach (var message in connection.getMessages()) responseRec += 1;
        if (onAuto)
        {
            StartCoroutine(startAuto());
            onAuto = false;
        }
        if (onDown)
        {
            connection.Send("down 10");
            commandSent += 1;
            onDown = false;
        }
        if (onBack)
        {
            connection.Send("back " + forwardSet.ToString());
            commandSent += 1;
            onBack = false;
        }
        if (onTakeOff)
        {
            connection.Send("takeoff");
            commandSent += 1;
            onTakeOff = false;
        }
        if (onForward)
        {
            connection.Send("forward " + forwardSet.ToString());
            commandSent += 1;
            onForward = false;
        }
        if (onLeft)
        {
            connection.Send("left " + forwardSet.ToString());
            commandSent += 1;
            onLeft = false;
        }
        if (onRight)
        {
            connection.Send("right " + forwardSet.ToString());
            commandSent += 1;
            onRight = false;
        }
        if (onLand)
        {
            onCalculatePos = false; 
            connection.Send("land");
            commandSent += 1;
            onLand = false;
        }
        if (onEmergency)
        {
            connection.Send("emergency");
            commandSent += 1;
            onEmergency = false;
        }
        if (onSpeed)
        {
            connection.Send("speed?");
            commandSent += 1;
            onSpeed = false;
        }
        if (setSpeed)
        {
            connection.Send("speed " + speedSet.ToString());
            commandSent += 1;
            setSpeed = false;
        }
        if (onRotate)
        {
            if (right)
            {
                connection.Send("cw " + (angleWhole).ToString());
                commandSent += 1;
            }
            else
            {
                connection.Send("ccw " + (angleWhole).ToString());
                commandSent += 1;
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
        //Debug.Log(localRotY);
        //Debug.Log(tState.yaw - localRotY);
    }
    void movePosition()
    {
        float tempx = transform.position.z + tState.sx2;
        float tempy = transform.position.x + tState.sy2;
        float tempz = transform.position.y + tState.sz2;

        float tempInt;
        if (rotation.y < 0)
        {
            tempInt = Math.Abs(rotation.y);
        }
        else
        {
            tempInt = 180 + rotation.y;
        }
        //Debug.Log("degree: " + tempInt.ToString());
        double a = (tempInt * Math.PI) / 180;
        //Debug.Log("a = " + a.ToString());

        //y' = y*cos(a) + x*sin(a)
        //x' = x*cos(a) - y*sin(a) 
        //double newY = tState.sy2 * Math.Cos(a) + tState.sx2 * Math.Sin(a);
        //double newX = tState.sx2 * Math.Cos(a) - tState.sy2 * Math.Sin(a);
        //clockwise
        //y' = y*cos(a) - x*sin(a)
        //x' = y*sin(a) + x*cos(a)
        double newY = tState.sy2 * Math.Cos(a) + tState.sx2 * Math.Sin(a);
        double newX = tState.sx2 * Math.Cos(a) - tState.sy2 * Math.Sin(a);
        transform.position = new Vector3(transform.position.x - (float)newY, 0, transform.position.z - (float)newX);
        //if (Math.Abs(rotation.y)>0) //< 25 || Math.Abs(rotation.y)> 65)
        //{
        //    float tempInt;
        //    if (rotation.y < 0)
        //    {
        //        tempInt = 180 + Math.Abs(rotation.y);
        //    }
        //    else
        //    {
        //        tempInt = rotation.y;
        //    }
        //    double a = (tempInt * Math.PI) / 180;
        //    double newY = tState.sy2 * Math.Cos(a) + tState.sx2 * Math.Sin(a);
        //    double newX = tState.sx2 * Math.Cos(a) - tState.sy2 * Math.Sin(a);
        //    //y' = y*cos(a) + x*sin(a)
        //    //x' = x*cos(a) - y*sin(a) 
        //    transform.position = new Vector3(transform.position.x + (float)newY, 0, transform.position.z + (float)newX);

        //}
        //else
        //{
        //    transform.position = new Vector3(tempy, 0, tempx);
        //}
        //Vector3 waypointDir = wayPointTrans.position - transform.position;
        //Vector3 tempV = new Vector3(tempx, tempy, tempz);
        //Vector3 tempx = transform.forward * tState.sx * Time.deltaTime;
        //Vector3 tempy = transform.up * tState.sy * Time.deltaTime;
        //Vector3 tempz = transform.right * tState.sz * Time.deltaTime;
        //transform.Translate(this.transform.forward * tState.sx , Space.World);
        //transform.position += transform.forward * (tState.sx2) ;
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

    void rotateTo(Vector3 pos)
    {
        Vector3 direction = pos - this.transform.position;
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

    IEnumerator startAuto()
    {
        yield return new WaitForSeconds(1);
        setSpeed = true;
        yield return new WaitForSeconds(1);
        Debug.Log("Set Speed " + speedSet.ToString());
        onTakeOff = true;
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => responseRec == commandSent);
        Debug.Log("TakeOff Finish");
        onCalculatePos = true;

        Vector3 lastDist = this.transform.position;
        for (int i = 0; i < MPD.data.objects.Count(); i++)
        {
            rotateTo(MPD.data.objects[i]);
            onRotate = true;
            yield return new WaitForSeconds(1);
            //Debug.Log(responseRec.ToString() + ",command sent: " + commandSent.ToString());
            yield return new WaitUntil(() => responseRec == commandSent);
            Debug.Log("Rotate " + angleWhole.ToString()+" Finish");
            forwardSet = ((int)Vector3.Distance(MPD.data.objects[i], lastDist)) * 10; 
            onForward = true;
            yield return new WaitForSeconds(1);
            yield return new WaitUntil(() => responseRec == commandSent);
            Debug.Log("Forward " + forwardSet.ToString() + " Finish");
            lastDist = MPD.data.objects[i];
        }
        Debug.Log("Finish Navigating");
    }
}
