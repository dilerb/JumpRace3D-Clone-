using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    [SerializeField] private LineRenderer line;

    internal bool inGreen = false;
    private Color redColor, greenColor;

    void Start()
    {
        //line = GetComponent<LineRenderer>();
        redColor = new Color(Color.red.r, Color.red.g, Color.red.b, 0.5f);
        greenColor = new Color(Color.green.r, Color.green.g, Color.green.b, 0.5f);
    }

    void Update()
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
                if (Vector3.Distance(hit.point, hit.collider.transform.position) < 1f)
                {
                    line.startColor = greenColor;
                    line.endColor = greenColor;
                    inGreen = true;
                }
                else
                {
                    line.startColor = redColor;
                    line.endColor = redColor;
                    inGreen = false;
                }
            }

            endPos = hit.point;

            line.SetPosition(0, line.transform.position);
            line.SetPosition(1, hit.point);
        }

        return hit;
    }
}
