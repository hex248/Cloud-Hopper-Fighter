using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class EnemyDebugRenderer : MonoBehaviour
{
    [SerializeField] LineRenderer viewRadiusRenderer;
    [SerializeField] LineRenderer attackRadiusRenderer;
    [SerializeField] EnemyController enemy;
    [SerializeField] bool renderDebug;
    [SerializeField] Color viewRadiusColour;
    [SerializeField] Color attackRadiusColour;
    [SerializeField] int segmentNumber = 128;
    [SerializeField][Range(0, 100)] float lineWidth = 100;

    void Update()
    {
        if (renderDebug)
        {
            viewRadiusRenderer.enabled = true;
            attackRadiusRenderer.enabled = true;

            viewRadiusRenderer.sharedMaterial.color = viewRadiusColour;
            viewRadiusRenderer.startWidth = lineWidth / 100;
            viewRadiusRenderer.endWidth = lineWidth / 100;
            viewRadiusRenderer.positionCount = segmentNumber + 1;
            viewRadiusRenderer.useWorldSpace = false;

            float viewDeltaTheta = (float)(2.0 * Mathf.PI) / segmentNumber;
            float viewTheta = 0f;

            for (int i = 0; i < segmentNumber + 1; i++)
            {
                float x = enemy.viewDistance * Mathf.Cos(viewTheta);
                float z = enemy.viewDistance * Mathf.Sin(viewTheta);
                Vector3 pos = new Vector3(x, 0, z);
                viewRadiusRenderer.SetPosition(i, pos);
                viewTheta += viewDeltaTheta;
            }


            attackRadiusRenderer.sharedMaterial.color = attackRadiusColour;
            attackRadiusRenderer.startWidth = lineWidth / 100;
            attackRadiusRenderer.endWidth = lineWidth / 100;
            attackRadiusRenderer.positionCount = segmentNumber + 1;
            attackRadiusRenderer.useWorldSpace = false;

            float attackDeltaTheta = (float)(2.0 * Mathf.PI) / segmentNumber;
            float attackTheta = 0f;

            for (int i = 0; i < segmentNumber + 1; i++)
            {
                float x = enemy.attackDistance * Mathf.Cos(attackTheta);
                float z = enemy.attackDistance * Mathf.Sin(attackTheta);
                Vector3 pos = new Vector3(x, 0, z);
                attackRadiusRenderer.SetPosition(i, pos);
                attackTheta += attackDeltaTheta;
            }
        }
        else
        {
            viewRadiusRenderer.enabled = false;
            attackRadiusRenderer.enabled = false;
        }
    }
}
