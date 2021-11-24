using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovePlayerDestination : MonoBehaviour
{
    public MoveGuideDestination guide;

    public Transform playerGoal;

    private LineRenderer line;

    private NavMeshAgent agent;


    public ObjectData data = new ObjectData();
    public List<GameObject> targetLoc;

    public GameObject playerPrefabs;


    public int count = 0;


    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        line = GetComponent<LineRenderer>();
        line.startWidth = 0.15f;
        line.endWidth = 0.15f;
        line.positionCount = 0;
        Mulai();
    }

    public void Mulai()
    {
        StartCoroutine(getPath(agent, playerPrefabs, playerGoal));
    }

    void Update()
    {
        //this.transform.LookAt(playerGoal.position);
    }

    IEnumerator getPath(NavMeshAgent nav, GameObject prefabs, Transform tPos)
    {
        yield return new WaitForEndOfFrame();
        //line.SetPosition(0, agent.path.corners[0]);

        nav.SetDestination(tPos.position);

        StartCoroutine(DrawPath(nav.path, prefabs));
        //nav.Stop();
    }

    IEnumerator DrawPath(NavMeshPath path, GameObject prefab)
    {
        yield return new WaitForEndOfFrame();
        path = agent.path;

        int count = path.corners.Length;
        line.SetVertexCount(path.corners.Length);

        Vector3 previousCorner = path.corners[0];
        for (int i = 0; i < count; i++)
        {
            Vector3 currentCorner = path.corners[i];
            //line.SetPosition(i, path.corners[i]);
            //Vector3 middle = Vector3.Lerp(previousCorner, currentCorner, i * 0.25f);
            //GameObject go = Instantiate(prefab, middle, Quaternion.identity) as GameObject;

            Vector3 middle = Vector3.Lerp(previousCorner, currentCorner, i);
            GameObject go = Instantiate(prefab, middle, Quaternion.identity) as GameObject;

            go.transform.LookAt(previousCorner);
            previousCorner = currentCorner;
        }
        DrawPathLine();
        getStep();
        //StartCoroutine(waitFor());
    }

    public void getStep()
    {
        GameObject[] stepPoint = GameObject.FindGameObjectsWithTag("step");
        foreach (GameObject point in stepPoint)
        {
            Transform pointTransform = point.transform;
            Vector3 pos = pointTransform.position;
            StepLocData SLD = new StepLocData(pos.x, pos.y, pos.z);
            data.objects.Add(SLD);
        }
    }

    void DrawPathLine()
    {
        line.positionCount = agent.path.corners.Length;
        line.SetPosition(0, transform.position);

        if (agent.path.corners.Length < 2)
        {
            return;
        }

        for (int i = 0; i < agent.path.corners.Length; i++)
        {
            Vector3 pointPosition = new Vector3(agent.path.corners[i].x, agent.path.corners[i].y, agent.path.corners[i].z);
            line.SetPosition(i, pointPosition);
        }
    }

    public class ObjectData
    {
        public List<StepLocData> objects;
        public ObjectData()
        {
            this.objects = new List<StepLocData>();
        }
    }

    public class StepLocData
    {
        public float posX;
        public float posY;
        public float posZ;
        public StepLocData(float px, float py, float pz)
        {
            this.posX = px;
            this.posY = py;
            this.posZ = pz;
        }
    }

    public void destroyAll()
    {
        GameObject[] willDelete = GameObject.FindGameObjectsWithTag("step");
        foreach (GameObject objDel in willDelete)
        {
            Destroy(objDel);
        }
        data.objects.Clear();
        line.positionCount = 0;
        //agent.velocity = Vector3.zero;
        agent.ResetPath();
    }
}