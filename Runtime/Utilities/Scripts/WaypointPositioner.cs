using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Virtuademy.SDK.Core.Utilities
{
    ///////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Simple component that allow to move, rotate and scale an object throw different waypoints.
    /// Waypoints can be editetd in scene <see cref="WayPointPositionerEditor"/>.
    /// </summary>
    public class WaypointPositioner : MonoBehaviour
    {
        //Enums
        public enum ELerpMovementType
        {
            LookThenMoveThenRotate = 0,
            LookThenMove = 1,
            MoveThenRotate = 2,
            RotateThenMove = 3
        }

        //Public
        public Transform objectToMove;
        public bool lerpMovement = false;
        public float lerpRotationSpeed = 0f;
        public float lerpTranslationSpeed = 0f;
        public ELerpMovementType lerpMovementType = ELerpMovementType.LookThenMoveThenRotate;
        public FixedTransform[] waypoints;

        //Editor
        public EditorParameters editorParameters = new EditorParameters(true, Color.green, Color.white, 0.5f, true);

        //Private
        private int lastWaypointIndex = 0;
        private Vector3 initialScale = Vector3.zero;
        private bool initialized = false;

        ///////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// For Unity Editor Inspector purposes.
        /// </summary>
        public int LastWaypointIndex => lastWaypointIndex;
        /// <summary>
        /// For Unity Editor Inspector purposes.
        /// </summary>
        public bool Initialized => initialized;

        ///////////////////////////////////////////////////////////////////////////
        //Structs
        [Serializable]
        public struct FixedTransform
        {
            [Tooltip("Target position")]
            public Vector3 pos;
            [Tooltip("Target Rotation")]
            public Vector3 rot;
            [Tooltip("Scale multiplier. If 0, reset to initial scale")]
            public float scaleMultiplier;
        }

        /// <summary>
        /// Parameters for the custom editor
        /// </summary>
        [Serializable]
        public struct EditorParameters
        {
            public bool showHandles;
            public Color firstWaypointColor;
            public Color waypointColor;
            public float waypointIconSize;
            public bool linkWaypoints;

            public EditorParameters(bool _showHandles, Color _firstWaypointColor, Color _waypointColor, float _waypointIconSize, bool _linkWaypoints)
            {
                showHandles = _showHandles;
                firstWaypointColor = _firstWaypointColor;
                waypointColor = _waypointColor;
                waypointIconSize = _waypointIconSize;
                linkWaypoints = _linkWaypoints;
            }
        }

        ///////////////////////////////////////////////////////////////////////////
        private void Init()
        {
            if (initialized) return;
            initialScale = objectToMove.localScale;
            lastWaypointIndex = 0;
            initialized = true;
        }

        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Change the referenced object's position and rotation according to the next waypoint
        /// </summary>
        public async Task MoveToNextWaypoint()
        {
            var index = lastWaypointIndex + 1;
            if (index >= waypoints.Length)
                Debug.LogError("There is no next waypoint");
            else
                await MoveIn(index);
        }

        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Change the referenced object's position and rotation according to the previous waypoint
        /// </summary>
        public async Task MoveToPreviousWaypoint()
        {
            var index = lastWaypointIndex - 1;
            if (index < 0)
                Debug.LogError("There is no previous waypoint");
            else
                await MoveIn(index);
        }

        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Change the referenced object's position and rotation according to the fist waypoint
        /// </summary>
        public async Task MoveToFirstWaypoint()
        {
            await MoveIn(0);
        }

        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Change the referenced object's position and rotation according to the last waypoint
        /// </summary>
        public async Task MoveToLastWaypoint()
        {
            await MoveIn(waypoints.Length - 1);
        }


        public async Task DoFullTrip()
        {
            await MoveToFirstWaypoint();
            while (lastWaypointIndex < waypoints.Length - 1)
            {
                await MoveToNextWaypoint();
            }
        }


        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Change the referenced object's position and rotation according to the waypoint
        /// </summary>
        /// <param name="wayPointIndex">index of the waypoint the object has to move to</param>
        public async Task MoveIn(int wayPointIndex)
        {
            if (!initialized)
                Init();

            if (lastWaypointIndex == wayPointIndex)
            {
                // Reset data to same position.
                StaticMoveIn(wayPointIndex);
            }
            else
            {
                lastWaypointIndex = wayPointIndex;

                if (!lerpMovement)
                {
                    StaticMoveIn(wayPointIndex);
                }
                else
                {
                    FixedTransform lastFixedTransform = waypoints[wayPointIndex];

                    switch (lerpMovementType)
                    {
                        case ELerpMovementType.LookThenMoveThenRotate:

                            // Look
                            await LerpLook(lastFixedTransform);

                            // Position
                            await LerpPos(lastFixedTransform);

                            // Rotation
                            await LerpRot(lastFixedTransform);

                            break;
                        case ELerpMovementType.LookThenMove:

                            // Look
                            await LerpLook(lastFixedTransform);

                            // Position
                            await LerpPos(lastFixedTransform);

                            break;
                        case ELerpMovementType.MoveThenRotate:

                            // Position
                            await LerpPos(lastFixedTransform);

                            // Rotation
                            await LerpRot(lastFixedTransform);

                            break;
                        case ELerpMovementType.RotateThenMove:

                            // Rotation
                            await LerpRot(lastFixedTransform);

                            // Position
                            await LerpPos(lastFixedTransform);

                            break;
                        default:
                            StaticMoveIn(wayPointIndex);
                            break;
                    }
                }
            }

            void StaticMoveIn(int wayPointIndex)
            {
                objectToMove.position = waypoints[wayPointIndex].pos;
                objectToMove.eulerAngles = waypoints[wayPointIndex].rot;
                var scaleMultiplier = waypoints[wayPointIndex].scaleMultiplier;
                if (scaleMultiplier == 0)
                    objectToMove.localScale = initialScale;
                else
                    objectToMove.localScale *= scaleMultiplier;
            }

            async Task LerpPos(FixedTransform lastFixedTransform)
            {
                if (lerpTranslationSpeed > 0f)
                {
                    var initialPos = objectToMove.position;

                    var spaceDif = Vector3.Distance(initialPos, lastFixedTransform.pos);
                    var posTime = spaceDif / lerpTranslationSpeed;
                    float currPosTime = 0f;
                    while (currPosTime < posTime)
                    {
                        currPosTime += Time.deltaTime;
                        if (currPosTime > posTime)
                        {
                            currPosTime = posTime;
                        }
                        objectToMove.position = Vector3.Lerp(initialPos, lastFixedTransform.pos, currPosTime / posTime);

                        await Task.Yield();
                    }
                }
                objectToMove.position = lastFixedTransform.pos;
            }

            async Task LerpRot(FixedTransform lastFixedTransform)
            {
                if (lerpRotationSpeed > 0f)
                {
                    Quaternion prevAngleQ = objectToMove.rotation;
                    Quaternion lastAngleQ = Quaternion.Euler(lastFixedTransform.rot);

                    var angleDif = Quaternion.Angle(prevAngleQ, lastAngleQ);
                    var rotTime = angleDif / lerpRotationSpeed;
                    float currRotTime = 0f;
                    while (currRotTime < rotTime)
                    {
                        currRotTime += Time.deltaTime;
                        if (currRotTime > rotTime)
                        {
                            currRotTime = rotTime;
                        }
                        objectToMove.rotation = Quaternion.Lerp(prevAngleQ, lastAngleQ, currRotTime / rotTime);

                        await Task.Yield();
                    }
                }
                objectToMove.eulerAngles = lastFixedTransform.rot;
            }

            async Task LerpLook(FixedTransform lastFixedTransform)
            {
                Quaternion lastAngleQ = Quaternion.LookRotation(lastFixedTransform.pos - objectToMove.position, Vector3.up); //calc a rotation that
                if (lerpRotationSpeed > 0f && lastFixedTransform.pos != objectToMove.position)
                {
                    Quaternion prevAngleQ = objectToMove.rotation;

                    var angleDif = Quaternion.Angle(prevAngleQ, lastAngleQ);
                    var rotTime = angleDif / lerpRotationSpeed;
                    float currRotTime = 0f;
                    Debug.Log("Start LOOK");
                    while (currRotTime < rotTime)
                    {
                        currRotTime += Time.deltaTime;
                        Debug.Log("LOOK " + currRotTime.ToString() + "/" + rotTime.ToString());
                        if (currRotTime > rotTime)
                        {
                            currRotTime = rotTime;
                        }
                        objectToMove.rotation = Quaternion.Lerp(prevAngleQ, lastAngleQ, currRotTime / rotTime);

                        await Task.Yield();
                    }
                    Debug.Log("End LOOK");
                }
                objectToMove.rotation = lastAngleQ;
            }
        }
    }
}
