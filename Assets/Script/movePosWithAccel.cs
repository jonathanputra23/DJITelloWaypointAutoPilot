using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movePosWithAccel : MonoBehaviour
{
    public telloState tState;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float tempx = transform.position.x + tState.sx2;
        float tempy = transform.position.z + tState.sy2;
        float tempz = transform.position.y + tState.sz2;
        transform.position = new Vector3(tempx, 0, tempy);
    }
}
