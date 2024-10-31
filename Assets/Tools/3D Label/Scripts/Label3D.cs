/*
*   Copyright (C) 2021 University of Central Florida, created by Dr. Ryan P. McMahan.
*
*   This program is free software: you can redistribute it and/or modify
*   it under the terms of the GNU General Public License as published by
*   the Free Software Foundation, either version 3 of the License, or
*   (at your option) any later version.
*
*   This program is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*   GNU General Public License for more details.
*
*   You should have received a copy of the GNU General Public License
*   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*
*   Primary Author Contact:  Dr. Ryan P. McMahan <rpm@ucf.edu>
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Class for handling 3D labels.
public class Label3D : MonoBehaviour
{
    // The Camera used for the label.
    [SerializeField]
    [Tooltip("The camera used for the label.")]
    private Camera m_camera;
    new public Camera camera { get { return m_camera; } set { m_camera = value; } }

    // The anchor used for the label.
    [SerializeField]
    [Tooltip("The anchor used for the label.")]
    private Transform m_anchor;
    public Transform anchor { get { return m_anchor; } set { m_anchor = value; } }

    // The Collider used for the anchor.
    [SerializeField]
    [Tooltip("The Collider used for the anchor.")]
    private Collider m_anchorCollider;
    public Collider anchorCollider { get { return m_anchorCollider; } set { m_anchorCollider = value; } }

    // The leader line used for the label.
    [SerializeField]
    [Tooltip("The leader line used for the label.")]
    private Transform m_leaderLine;
    public Transform leaderLine { get { return m_leaderLine; } set { m_leaderLine = value; } }

    // The LineRenderer used for the leader line.
    [SerializeField]
    [Tooltip("The LineRenderer used for the leader line.")]
    private LineRenderer m_lineRenderer;
    public LineRenderer lineRenderer { get { return m_lineRenderer; } set { m_lineRenderer = value; } }

    // The Collider used for the label.
    [SerializeField]
    [Tooltip("The Collider used for the label.")]
    private Collider m_labelCollider;
    public Collider labelCollider { get { return m_labelCollider; } set { m_labelCollider = value; } }

    // The annotation used for the label.
    [SerializeField]
    [Tooltip("The annotation used for the label.")]
    private Transform m_annotation;
    public Transform annotation { get { return m_annotation; } set { m_annotation = value; } }

    // The incremental distance used to extend the leader line.
    [SerializeField]
    [Tooltip("The incremental distance used to extend the leader line.")]
    private float m_incrementDistance = 0.0125f;
    public float incrementDistance { get { return m_incrementDistance; } set { m_incrementDistance = value; } }

    // The minimum distance used to extend the leader line.
    [SerializeField]
    [Tooltip("The minimum distance used to extend the leader line.")]
    private float m_minimumDistance = 0.025f;
    public float minimumDistance { get { return m_minimumDistance; } set { m_minimumDistance = value; } }

    // The maximum distance used to extend the leader line.
    [SerializeField]
    [Tooltip("The maximum distance used to extend the leader line.")]
    private float m_maximumDistance = 5.000f;
    public float maximumDistance { get { return m_maximumDistance; } set { m_maximumDistance = value; } }

    // The scale factor used to further extend the leader line.
    [SerializeField]
    [Tooltip("The distance scale factor used to further extend the leader line.")]
    private float m_distanceScale = 1.500f;
    public float distanceScale { get { return m_distanceScale; } set { m_distanceScale = value; } }

    // The distance from the camera that the label is displayed.
    [SerializeField]
    [Tooltip("The distance from the camera that the label is displayed.")]
    private float m_distanceFromCamera = 0.100f;
    public float distanceFromCamera { get { return m_distanceFromCamera; } set { m_distanceFromCamera = value; } }

    // Whether to require the anchor to be visible to show the label.
    [SerializeField]
    [Tooltip("Whether to require the anchor to be visible to show the label.")]
    private bool m_requireVisibleAnchor = false;
    public bool requireVisibleAnchor { get { return m_requireVisibleAnchor; } set { m_requireVisibleAnchor = value; } }

    // Reset function for initializing the Label3D.
    void Reset()
    {
        // Set the camera to the main camera.
        camera = Camera.main;
        // Provide a warning if a main camera was not found.
        if (camera == null) { Debug.LogWarning("[" + gameObject.name + "][Label3D]: Did not find a main Camera."); }

        // Attempt to find the anchor.
        anchor = transform.Find("Anchor");
        // Provide a warning if the anchor was not found.
        if (anchor == null) { Debug.LogWarning("[" + gameObject.name + "][Label3D]: Did not find a child GameObject named 'Anchor'. Using the '3D Label' prefab is highly recommended."); }
        // If the anchor was found.
        else
        {
            // Attempt to find a Collider for the anchor.
            anchorCollider = anchor.GetComponent<Collider>();
            // Provide a warning if a Collider for the anchor was not found.
            if (anchorCollider == null) { Debug.LogWarning("[" + gameObject.name + "][Label3D]: Did not find a Collider for the anchor. Using the '3D Label' prefab is highly recommended."); }
        }

        // Attempt to find the leader line.
        leaderLine = transform.Find("Anchor/Leader Line");
        // Provide a warning if the leader line was not found.
        if (leaderLine == null) { Debug.LogWarning("[" + gameObject.name + "][Label3D]: Did not find a child GameObject named 'Leader Line'. Using the '3D Label' prefab is highly recommended."); }
        // If the leader line was found.
        else
        {
            // Attempt to find a Collider for the leader line.
            labelCollider = leaderLine.GetComponent<Collider>();
            // Provide a warning if a Collider for the leader line was not found.
            if (labelCollider == null) { Debug.LogWarning("[" + gameObject.name + "][Label3D]: Did not find a Collider for the leader line. Using the '3D Label' prefab is highly recommended."); }

            // Attempt to find a LineRenderer for the leader line.
            lineRenderer = leaderLine.GetComponent<LineRenderer>();
            // Provide a warning if a LineRenderer for the leader line was not found.
            if (lineRenderer == null) { Debug.LogWarning("[" + gameObject.name + "][Label3D]: Did not find a LineRenderer for the leader line. Using the '3D Label' prefab is highly recommended."); }
        }

        // Attempt to find the annotation.
        annotation = transform.Find("Anchor/Leader Line/Annotation");
        // Provide a warning if the annotation was not found.
        if (annotation == null) { Debug.LogWarning("[" + gameObject.name + "][Label3D]: Did not find a child GameObject named 'Annotation'. Using the '3D Label' prefab is highly recommended."); }
    }

    // Start is called before the first frame update.
    void Start()
    {
        // If the camera is not set.
        if (camera == null)
        {
            // Set the camera to the main camera.
            camera = Camera.main;
            // Provide an error if a main camera was not found.
            if (camera == null) { Debug.LogError("[" + gameObject.name + "][Label3D]: Did not find a main Camera."); }
            // Provide a warning that the camera was set to the main camera.
            else { Debug.LogWarning("[" + gameObject.name + "][Label3D]: Set camera to main Camera."); }
        }
    }

    // LateUpdate is called once per frame, after all Updates have been called.
    private void LateUpdate()
    {
        // If the Label3D is properly configured.
        if (camera != null && anchor != null && anchorCollider != null && leaderLine != null && lineRenderer != null && labelCollider != null && annotation != null)
        {
            // Ensure the anchor is shown.
            anchor.gameObject.SetActive(true);

            // If the anchor is visible or not required.
            if (IsAnchorVisible() || !m_requireVisibleAnchor)
            {
                // Reset the local position of the leader line to the minimum extension.
                Vector3 localPosition = new Vector3(0.0f, minimumDistance, 0.0f);
                leaderLine.transform.localPosition = localPosition;

                // Billboard the leader line to align with the camera.
                leaderLine.transform.LookAt(leaderLine.transform.position + (camera.transform.rotation * Vector3.forward), (camera.transform.rotation * Vector3.up));

                // While the label is not fully visible and it is less than the maximum distance.
                while (!IsLabelVisible() && localPosition.y < maximumDistance)
                {
                    // Move the leader line out by the increment.
                    localPosition.y += incrementDistance;
                    leaderLine.transform.localPosition = localPosition;
                }

                // Ensure the leader line is still within the maximum distance.
                if (localPosition.y <= maximumDistance)
                {
                    // Scale the leader line out by the scale factor.
                    localPosition.y *= distanceScale;
                    leaderLine.transform.localPosition = localPosition;

                    // Draw the leader line from the anchor to the annotation.
                    lineRenderer.SetPosition(0, anchor.transform.position);
                    lineRenderer.SetPosition(1, leaderLine.transform.position);
                    
                    // Move the annotation forward.
                    MoveAnnotationForward();
                }
                // Otherwise, hide the anchor.
                else { anchor.gameObject.SetActive(false); }
            }
            // Otherwise, hide the anchor.
            else { anchor.gameObject.SetActive(false); }
        }
        // Otherwise, hide the anchor.
        else { anchor.gameObject.SetActive(false); }
        // Provide a warning if the camera was not found.
        if (camera == null) { Debug.LogError("[" + gameObject.name + "][Label3D]: Missing the Camera."); }
        // Provide a warning if the anchor was not found.
        if (anchor == null) { Debug.LogError("[" + gameObject.name + "][Label3D]: Missing the Anchor."); }
        // Provide a warning if the anchorCollider was not found.
        if (anchorCollider == null) { Debug.LogError("[" + gameObject.name + "][Label3D]: Missing the Anchor Collider."); }
        // Provide a warning if the leaderLine was not found.
        if (leaderLine == null) { Debug.LogError("[" + gameObject.name + "][Label3D]: Missing the Leader Line."); }
        // Provide a warning if the lineRenderer was not found.
        if (lineRenderer == null) { Debug.LogError("[" + gameObject.name + "][Label3D]: Missing the Line Renderer."); }
        // Provide a warning if the labelCollider was not found.
        if (labelCollider == null) { Debug.LogError("[" + gameObject.name + "][Label3D]: Missing the Label Collider."); }
        // Provide a warning if the annotation was not found.
        if (annotation == null) { Debug.LogError("[" + gameObject.name + "][Label3D]: Missing the Annotation."); }
    }

    // Determines whether the anchor is visible based on raycasting and colliders.
    private bool IsAnchorVisible()
    {
        // If the Label3D is properly configured.
        if (camera != null && anchorCollider != null)
        {
            // Set the raycast origin to the camera's position.
            Vector3 origin = camera.transform.position;
            // Set the raycast direction to the vector formed by the anchor and the camera.
            Vector3 direction = anchorCollider.transform.position - camera.transform.position;
            // Information about the raycast hit.
            RaycastHit hitInfo;

            // Track whether raycasts initially hit triggers.
            bool queriesHitTriggers = Physics.queriesHitTriggers;
            // Ensure raycasts hit triggers.
            Physics.queriesHitTriggers = true;

            // Determine whether the raycast hit something.
            if (Physics.Raycast(origin, direction, out hitInfo))
            {
                // If the raycast hit something other than the anchor collider.
                if (hitInfo.collider != anchorCollider)
                {
                    // Reset whether queries hit triggers.
                    Physics.queriesHitTriggers = queriesHitTriggers;
                    // Return that the raycast did not hit the annotation.
                    return false;
                }
            }
        }
        // Otherwise, assume the anchor is visible.
        return true;
    }

    // Determines whether the label is visible based on raycasting and colliders.
    private bool IsLabelVisible()
    {
        // If the Label3D is properly configured.
        if (camera != null && labelCollider != null)
        {
            // Set the raycast origin to the camera's position.
            Vector3 origin = camera.transform.position;
            // Set the raycast direction to the vector formed by the label collider and the camera.
            Vector3 direction = labelCollider.transform.position - camera.transform.position;
            // Information about the raycast hit.
            RaycastHit hitInfo;

            // Track whether raycasts initially hit triggers.
            bool queriesHitTriggers = Physics.queriesHitTriggers;
            // Ensure raycasts hit triggers.
            Physics.queriesHitTriggers = true;

            // Disable the label collider.
            labelCollider.enabled = false;

            // Determine whether the raycast hit something.
            if (Physics.Raycast(origin, direction, out hitInfo))
            {
                // Re-enable the label collider.
                labelCollider.enabled = true;
                // Reset whether queries hit triggers.
                Physics.queriesHitTriggers = queriesHitTriggers;
                // Return that the label is not visible.
                return false;
            }

            // Re-enable the label collider.
            labelCollider.enabled = true;
        }
        // Otherwise, assume the label is visible.
        return true;
    }

    // Brings the annotation forward to be a consistent image space size.
    private void MoveAnnotationForward()
    {
        // If the Label3D is properly configured.
        if (camera != null && annotation != null)
        {
            // Set the origin to the camera's position.
            Vector3 origin = camera.transform.position;
            // Set the direction to the vector formed by the leader line and the camera.
            Vector3 direction = leaderLine.transform.position - camera.transform.position;
            // Normalize the direction vector.
            direction.Normalize();
            // Calculate the new position for the annotation.
            Vector3 position = origin + (direction * distanceFromCamera);
            // Set the new position of the annotation.
            annotation.transform.position = position;
        }
    }
}
