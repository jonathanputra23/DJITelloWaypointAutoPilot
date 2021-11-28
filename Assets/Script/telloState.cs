using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

public class telloState : MonoBehaviour
{
    public float yaw, pitch, roll; //degrees
    public float vgx, vgy, vgz; //decimeter/s
    private float templ, temph;
    public float bat;
    private float tof, h, baro, time; //barometer value (m) 
    public float agx, agy, agz; //(0.001g)
    private UdpClient udpClient, udpState;
    private IPEndPoint RemoteIpEndPoint;
    public static List<string> statePerData = new List<string>();
    public static List<string> singleStatePerData = new List<string>();
    private Byte[] sendBytes, receiveBytes, receiveBytesState;
    private string returnData, returnDataState;
    Thread receiveStateThread;
    [HideInInspector]
    public bool threadRunning;
    private float sx = 0, sy = 0, sz = 0;
    public float sx2 = 0, sy2 = 0, sz2 = 0;

    public float totalX = 0, totalY = 0, totalZ = 0;

    //public float totalXfromList;
    //private List<float> listOfCalculation = new List<float>();

    public void Start()
    {
        //startSDK();

        receiveStateThread = new Thread(() => updateState());
        receiveStateThread.IsBackground = true;
        threadRunning = true;
        receiveStateThread.Start();
    }

    void Update()
    {
        //totalXfromList = listOfCalculation.Sum();
        //updateState();
        //if(returnData == "ok")
        //{
        //    udpClient.Close();
        //    updateState();
        //}
        //else
        //{
        //    startSDK();
        //}

    }

    void updateState()
    {
        udpState = new UdpClient(8890);
        RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8890);
        DateTime firstTime = DateTime.Now;
        while (threadRunning)
        {
            try
            {
                receiveBytesState = udpState.Receive(ref RemoteIpEndPoint);
                returnDataState = Encoding.ASCII.GetString(receiveBytesState);
                char[] delimiter = { ';' };
                char[] delimiter2 = { ':' };
                List<string> statePerData = new List<string>(returnDataState.Split(delimiter));
                pitch = float.Parse(statePerData[0].Split(delimiter2)[1]);
                roll = float.Parse(statePerData[1].Split(delimiter2)[1]);
                yaw = float.Parse(statePerData[2].Split(delimiter2)[1]);

                vgx = float.Parse(statePerData[3].Split(delimiter2)[1]);
                vgy = float.Parse(statePerData[4].Split(delimiter2)[1]);
                vgz = float.Parse(statePerData[5].Split(delimiter2)[1]);

                templ = float.Parse(statePerData[6].Split(delimiter2)[1]);
                temph = float.Parse(statePerData[7].Split(delimiter2)[1]);

                tof = float.Parse(statePerData[8].Split(delimiter2)[1]);
                h = float.Parse(statePerData[9].Split(delimiter2)[1]);
                bat = float.Parse(statePerData[10].Split(delimiter2)[1]);
                baro = float.Parse(statePerData[11].Split(delimiter2)[1]);
                time = float.Parse(statePerData[12].Split(delimiter2)[1]);

                agx = float.Parse(statePerData[13].Split(delimiter2)[1]);
                agy = float.Parse(statePerData[14].Split(delimiter2)[1]);
                agz = float.Parse(statePerData[15].Split(delimiter2)[1]);
                DateTime lastTime = DateTime.Now;
                TimeSpan diff = lastTime - firstTime;
                int timeSince = (int)diff.TotalMilliseconds;
                //calculateJarakWithout(timeSince);
                //sx = vgx / 10 * timeSince / 1000;//vg < dcm/s, time < ms
                //sy = vgy / 10 * timeSince / 1000;
                //sz = vgz / 10 * timeSince / 1000;
                sx2 = vgx / 10 * timeSince / 1000 + (1 / 2) * ((agx / 1000) * (98 / 10)) * ((timeSince / 1000) * (timeSince / 1000));//vg < dcm/s, time < ms
                sy2 = vgy / 10 * timeSince / 1000 + (1 / 2) * ((agy / 1000) * (98 / 10)) * ((timeSince / 1000) * (timeSince / 1000));
                sz2 = vgz / 10 * timeSince / 1000 + (1 / 2) * ((agz / 1000) * (98 / 10)) * ((timeSince / 1000) * (timeSince / 1000));
                totalX += sx2;
                totalY += sy2;
                totalZ += sz2;
                firstTime = lastTime;
                //listOfCalculation.Add(sx2);
                //calculateJarakWith(timeSince);
                //Debug.Log(timeSince);
            }
            catch (SocketException e)
            {
                // 10004 thrown when socket is closed
                if (e.ErrorCode != 10004) Debug.Log("Socket exception while receiving data from udp client: " + e.Message);
            }
            catch (Exception e)
            {
                Debug.Log("Error receiving data from udp client: " + e.Message);
            }
            Thread.Sleep(1);
        }

    }

    void calculateJarakWithout(int timez)
    {
        //s = v*t <- without acceleration
        sx = vgx / 10 * timez / 1000;//vg < dcm/s, time < ms
        sy = vgy / 10 * timez / 1000;
        sz = vgz / 10 * timez / 1000;
        //sx2 = vgx / 10 * timez / 1000 + 1 / 2 * agx / 1000 * 98 / 10 * (timez / 1000) * (timez / 1000);//vg < dcm/s, time < ms
        //sy2 = vgy / 10 * timez / 1000 + 1 / 2 * agy / 1000 * 98 / 10 * (timez / 1000) * (timez / 1000);
        //sz2 = vgz / 10 * timez / 1000 + 1 / 2 * agz / 1000 * 98 / 10 * (timez / 1000) * (timez / 1000);
        //listOfCalculation.Add(sx);
        //totalX += sx;
        //totalY += sy;
        //totalZ += sz;
    }

    void calculateJarakWith(int timez)
    {
        //s = v0 t + 1/2 a t^2 <- with constant acceleration, a 0.001g = 0.00980665m/s / 0.01m/s, 1g = 9.80665
        sx2 = vgx / 10 * timez / 1000 + 1 / 2 * agx / 1000 * 98 / 10 * (timez / 1000) * (timez / 1000);//vg < dcm/s, time < ms
        sy2 = vgy / 10 * timez / 1000 + 1 / 2 * agy / 1000 * 98 / 10 * (timez / 1000) * (timez / 1000); 
        sz2 = vgz / 10 * timez / 1000 + 1 / 2 * agz / 1000 * 98 / 10 * (timez / 1000) * (timez / 1000);
        //listOfCalculation.Add(sx);
        //totalX += sx;
        //totalY += sy;
        //totalZ += sz;
    }
    void startSDK()
    {
        //var server = new UDPServer();
        //server.Initialize();
        //server.StartMessageLoop();
        //Console.WriteLine("Response Listening!");
        Debug.Log("Test!");
        udpClient = new UdpClient(8889);
        
        try
        {
            udpClient.Connect("192.168.10.1", 8889);

            // Sends a message to the host to which you have connected.
            //Byte[] sendBytes = Encoding.ASCII.GetBytes("command");

            //udpClient.Send(sendBytes, sendBytes.Length);

            //IPEndPoint object will allow us to read datagrams sent from any source.
            RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8889);
            String msg = "command";
            sendBytes = Encoding.ASCII.GetBytes(msg);

            udpClient.Send(sendBytes, sendBytes.Length);


            receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
            returnData = Encoding.ASCII.GetString(receiveBytes);

            // Uses the IPEndPoint object to determine which of these two hosts responded.
            Debug.Log(returnData.ToString());

        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            udpClient.Close();
            udpState.Close();
        }
    }
}
