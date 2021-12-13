using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
//using System.Threading;

public class TelloController : MonoBehaviour
{
    private UdpConnection connection;
    [HideInInspector]
    public MovePlayerDestination MPD;
    [HideInInspector]
    public telloState tState;
    private Vector3 rotation;
    public int speedSet=35;
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
    public bool onRcCon = false;

    //Thread calculatePosThread;
    public int commandSent = 0;
    public int responseRec = -1; //termasuk send command pertama

    public int a, b, c, d;
    /*
    a: left/right (-100~100)
    b: forward/backward (-100~100)
    c: up/down (-100~100)
    d: yaw (-100~100)
    */
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
        //StartCoroutine(instantiateRotation());

    }

    void Update()
    {
        //rotateTo(MPD.guide.transform.position);
        foreach (var message in connection.getMessages()) responseRec += 1;
        if (onRcCon)
        {
            connection.Send("rc " + a.ToString() + " " + b.ToString() + " " + c.ToString() + " " + d.ToString());
            onRcCon = false;
        }
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
            //calculatePosThread = new Thread(() => movePosition());
            //calculatePosThread.IsBackground = true;
            //calculatePosThread.Start();
            //onCalculatePos = false;
        }
        rotation = new Vector3(tState.roll, tState.yaw, -tState.pitch);
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
        //double tempx2=0;
        //double tempy2=0;
        float tempx = transform.position.z + tState.sx2;
        float tempy = transform.position.x + tState.sy2;
        float tempz = transform.position.y + tState.sz2;

        float tempInt;
        double newX, newY;
        //x didrone = z diunity = y di rumus matematika
        //y didrone = x diunity = x di rumus matematika
        if ((tState.vgx == 0) || (tState.vgy == 0)) //kalo data dari drone hanya mengembalikan satu data
        {
            if (rotation.y < 0)
            {
                //
                //tempInt = Math.Abs(rotation.y);
                tempInt = 181 + 180 + rotation.y;
            }
            else
            {
                //tempInt = 180 + rotation.y;
                tempInt = rotation.y;
            }
            double a = (tempInt * Math.PI) / 180;
            //ccw
            //x' = x*cos(a) - y*sin(a) 
            //y' = y*cos(a) + x*sin(a)
            newX = Math.Abs(tState.sy2) * Math.Cos(a) - Math.Abs(tState.sx2) * Math.Sin(a);
            newY = Math.Abs(tState.sx2) * Math.Cos(a) + Math.Abs(tState.sy2) * Math.Sin(a);

            if (tState.vgy == 0)//normal
            {
                transform.position = new Vector3(transform.position.x - (float)newX, 0, transform.position.z + (float)newY);
            }
            else//kalo data yang didapet cuma y, y diubah menjadi x dan x diubah menjadi y
            {
                transform.position = new Vector3(transform.position.x + (float)newY, 0, transform.position.z + (float)newX);

            }  
        }
        else
        {
            transform.position = new Vector3(tempy, 0, tempx);
        }
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
            if (forwardSet < 20)
            {
                b = 15;
                onRcCon = true;
                yield return new WaitForSeconds(3);
                Debug.Log("RC Con " + b.ToString() + " Start");

                b = 0;
                onRcCon = true;
                yield return new WaitForSeconds(1);
                Debug.Log("RC Con " + b.ToString() + " Finish");

            }
            else
            {
                onForward = true;
                yield return new WaitForSeconds(1);
                yield return new WaitUntil(() => responseRec == commandSent);
                Debug.Log("Forward " + forwardSet.ToString() + " Finish");

            }

            lastDist = MPD.data.objects[i];
        }
        Debug.Log("Finish Navigating");
    }
}
