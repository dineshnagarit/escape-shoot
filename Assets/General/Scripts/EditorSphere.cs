#if UNITY_EDITOR

using UnityEngine;
using System.Collections;

public class EditorSphere : MonoBehaviour
{
    public bool DrawOnlyOnSelected = false;
    

    private void OnDrawGizmosSelected()
    {
        if (!DrawOnlyOnSelected)
            return;

        DrawGizmos();
    }

    private void OnDrawGizmos()
    {
        if (DrawOnlyOnSelected)
            return;

        DrawGizmos();
    }

    private void DrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}

#endif