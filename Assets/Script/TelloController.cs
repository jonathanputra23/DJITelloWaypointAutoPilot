using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TelloLib;

public class TelloController : MonoBehaviour
{
    private UdpConnection connection;
    public MovePlayerDestination MPD;
    public telloState tState;
    private Vector3 rotation;
    public int speedSet=10;
    public int forwardSet = 50;
    public bool onBattery;
    public bool onSpeed;
    public bool setSpeed;
    public bool onTakeOff;
    public bool onForward;
    public bool onLand;
    public bool onEmergency;
    private float angle;
    public int angleWhole;
    public bool onRotate;

    void Start()
    {
        string sendIp = "192.168.10.1";
        int sendPort = 8889;
        int receivePort = 8889;

        connection = new UdpConnection();
        connection.StartConnection(sendIp, sendPort, receivePort);
        connection.Send("command");
    }

    void Update()
    {
        rotateTo();
        foreach (var message in connection.getMessages()) Debug.Log(message);
        if (onBattery)
        {
            connection.Send("battery?");
            onBattery = false;
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
            connection.Send("cw " + angleWhole.ToString());
            onRotate = false;
        }
        rotation = new Vector3(tState.roll, tState.yaw, -tState.pitch);
        this.transform.eulerAngles = rotation;
    }

    void OnDestroy()
    {
        connection.Stop();
    }

    void rotateTo()
    {
        Vector3 direction = MPD.playerGoal.position - this.transform.position;
        Debug.DrawRay(this.transform.position, direction, Color.green);

        angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg - 90;
        angleWhole = (int)angle;
        if (angleWhole < 0)
        {
            System.Math.Abs(angleWhole);
            angleWhole += 180;
        }
        //Debug.Log("Angle: " + angle);

        //Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.up);
        //transform.rotation = Quaternion.Slerp(transform.rotation, angleAxis, Time.deltaTime * 50);
    }
}
