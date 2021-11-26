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

    public GameObject playerPrefabs;



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
        nav.Stop();
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
            //Debug.Log(middle);
            getStep(middle);
            go.transform.LookAt(previousCorner);
            previousCorner = currentCorner;
        }
        //for(int i = 0; i < data.objects.Count; i++)
        //{
        //Debug.Log(data.objects[i]);

        //}
        DrawPathLine();
        //getStep();
        //StartCoroutine(waitFor());
    }

    public void getStep(Vector3 stepData)
    {
        data.objects.Add(stepData);
        //GameObject[] stepPoint = GameObject.FindGameObjectsWithTag("step");
        //foreach (GameObject point in stepPoint)
        //{
        //    Transform pointTransform = point.transform;
        //    Vector3 pos = pointTransform.position;
        //    StepLocData SLD = new StepLocData(pos.x, pos.y, pos.z);
        //    data.objects.Add(SLD);
        //}
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
        public List<Vector3> objects;
        public ObjectData()
        {
            this.objects = new List<Vector3>();
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