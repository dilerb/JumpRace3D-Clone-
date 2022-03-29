using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    private bool inGreen = false;
    void Start()
    {
        //line = GetComponent<LineRenderer>();
    }

    void FixedUpdate()
    {
        DetectHit(line.transform.position, 1000, -transform.up);
    }

    RaycastHit DetectHit(Vector3 startPos, float distance, Vector3 direction)
    {
        //init ray to save the start and direction values
        Ray ray = new Ray(startPos, direction);
        //varible to hold the detection info
        RaycastHit hit;
        //the end Pos which defaults to the startPos + distance 
        Vector3 endPos = startPos + (distance * direction);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Trampoline"))
            {
                if (Vector3.Distance(hit.point, hit.collider.transform.position) < 1.5f)
                {
                    line.SetColors(Color.white, Color.green);
                }
                else
                {
                    line.SetColors(Color.white, Color.red);
                }
            }

            endPos = hit.point;

            line.SetPosition(0, line.transform.position);
            line.SetPosition(1, hit.point);
        }

        return hit;
    }
}
