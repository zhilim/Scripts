using System;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class Camera2DFollow : MonoBehaviour
    {
        public Transform target;
        public Transform background;
        public float damping = 1;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;

        private float m_OffsetZ;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;
        public float parallaxScale = 0.5f;

        // Use this for initialization
        private void Start()
        {
            m_LastTargetPosition = target.position;
            m_OffsetZ = (transform.position - target.position).z;
            transform.parent = null;
        }


        // Update is called once per frame
        private void Update()
        {
            // only update lookahead pos if accelerating or changed direction
            float xMoveDelta = (target.position - m_LastTargetPosition).x;

            bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

            if (updateLookAheadTarget)
            {
                m_LookAheadPos = lookAheadFactor*Vector3.right*Mathf.Sign(xMoveDelta);
            }
            else
            {
                m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime*lookAheadReturnSpeed);
            }

            Vector3 aheadTargetPos = target.position + m_LookAheadPos + Vector3.forward*m_OffsetZ;
            Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);

            if (newPos.y < 4.7f)
            {
                //Debug.Log("Camera reached ground.");
                newPos.y = 4.7f;
                //Debug.Log("Camera newpos: "  + newPos.y);
            }

            if (newPos.x < -5.0f)
                newPos.x = -5.0f;
            if (newPos.x > 5.0f)
                newPos.x = 5.0f;

            float parax = newPos.x - transform.position.x;
            float tempx = background.position.x;
            Vector3 bgPos = background.position;
            bgPos.x = tempx + parax * parallaxScale;
            background.position = bgPos;



            transform.position = newPos;

            m_LastTargetPosition = target.position;
        }
    }
}
