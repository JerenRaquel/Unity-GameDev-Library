using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Version 1.2
/// Custom script for dynamic lerping through script.
/// </summary>
namespace ObjectTravel {
    public delegate void FunctionCall();

    #region Argument Class
    /// <summary>
    /// Class to hold the instructions for the object to follow
    /// </summary>
    [System.Serializable]
    public class TravelDataArgs {
        public Transform travelObject;
        public FunctionCall callback;
        public Vector3 start;
        public Vector3 end;
        public bool isRotating;
        public float speed;
        public bool isQueued;

        #region Constructors
        /// <summary>
        /// Positional Travelling
        /// <example>
        /// <code>
        /// TravelDataArgs args = TravelDataArgs(
        ///   obj.transform,
        ///   () => { Debug.log("Travel Complete!")},
        ///   obj.transform.position,
        ///   Vector3.zero
        /// )
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="obj">Root Transform that holds everything that needs to move at once.</param>
        /// <param name="callback">A function that gets called once complete. Can be used to chain call.</param>
        /// <param name="start">The starting position.</param>
        /// <param name="end">The ending position.</param>
        /// <param name="speed">The speed of the movement.</param>
        /// <param name="isQueued">Queue this instruction?</param>
        public TravelDataArgs(
          Transform obj, FunctionCall callback, Vector3 start,
          Vector3 end, float speed = 10f, bool isQueued = true
        ) {
            this.travelObject = obj;
            this.callback = callback;
            this.start = start;
            this.end = end;
            this.isRotating = false;
            this.speed = speed;
            this.isQueued = isQueued;
        }

        /// <summary>
        /// Rotational Travelling
        /// <example>
        /// <code>
        /// TravelDataArgs args = TravelDataArgs(
        ///   obj.transform,
        ///   () => { Debug.log("Travel Complete!")},
        ///   obj.transform.rotation,
        ///   obj.transform.rotation * Quaternion.Euler(rotation)
        /// )
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="obj">Root Transform that holds everything that needs to move at once.</param>
        /// <param name="callback">A function that gets called once complete. Can be used to chain call.</param>
        /// <param name="start">The starting rotation position.</param>
        /// <param name="end">The ending rotation position.</param>
        /// <param name="speed">The speed of the movement.</param>
        /// <param name="isQueued">Queue this instruction?</param>
        public TravelDataArgs(
          Transform obj, FunctionCall callback, Quaternion start,
          Quaternion end, float speed = 2f, bool isQueued = true
        ) {
            this.travelObject = obj;
            this.callback = callback;
            this.start = start.eulerAngles;
            this.end = end.eulerAngles;
            this.isRotating = true;
            this.speed = speed;
            this.isQueued = isQueued;
        }
        #endregion
    }
    #endregion

    /// <summary>
    /// Handler for Object Travel
    /// </summary>
    public class ObjectTravelHandler : MonoBehaviour {
        #region Class Instance
        public static ObjectTravelHandler instance = null;
        private void CreateInstance() {
            if (instance == null)
                instance = this;
            else
                Destroy(this);
        }
        #endregion
        private void Awake() => CreateInstance();

        /// <summary>
        /// Holds the instructions from the arguement class. Parses the data in a usable manner.
        /// </summary>
        private class TravelData {
            // Node
            public Transform travelObject;
            // Positions
            public Vector3 start;
            public Vector3 end;
            // Travel Keeping
            public FunctionCall callback;
            private float initialTime;
            public float speed;
            // Flags
            public bool rotating = false;

            /// <summary>
            /// Parse the required data needed to move the object.
            /// </summary>
            /// <param name="args">Instructions for how to move the object.</param>
            public TravelData(TravelDataArgs args) {
                this.travelObject = args.travelObject;
                this.start = args.start;
                this.end = args.end;
                this.callback = args.callback;
                this.speed = args.speed;
                this.rotating = args.isRotating;
                this.initialTime = -1;
            }

            /// <summary>
            /// Initialize Timer
            /// </summary>
            public void SetStartTime() {
                this.initialTime = Time.time;
            }

            /// <summary>
            /// Activate Trigger Event
            /// </summary>
            public void FinishTravel() {
                if (this.callback != null) {
                    this.callback();
                }
            }

            /// <summary>
            /// Value of the time at start
            /// </summary>
            public float StartTime {
                get {
                    Debug.Assert(this.initialTime >= 0, "Start time was never set.");
                    return this.initialTime;
                }
            }
        }

        #region Handler Member Data and Methods
        // Member Data
        [Header("Settings")]
        public float threshold = 0.001f;

        private Queue<TravelData> travelDataObjects;
        private TravelData currentTravelData = null;
        private bool travelling = false;

        /// <summary>
        /// Initialize on start up.
        /// </summary>
        private void Start() {
            travelDataObjects = new Queue<TravelData>();
        }

        /// <summary>
        /// Move objects on every physics update
        /// </summary>
        private void FixedUpdate() {
            // Travel if the flag is set
            if (this.travelling) {
                // Check if the object reached its destination.
                if (CalculateNewTransform(ref this.currentTravelData)) {
                    // Finish and clean up.
                    this.travelling = false;
                    this.currentTravelData.FinishTravel();
                    this.currentTravelData = null;
                }
            } else if (this.currentTravelData == null && this.travelDataObjects.Count > 0) {
                // Check if the current object is finished travelling and dequeue another.
                this.currentTravelData = this.travelDataObjects.Dequeue();
                this.currentTravelData.SetStartTime();
                this.travelling = true;
            }
        }

        /// <summary>
        /// Method to request for the object in the args to travel. Parses the 
        /// traveDataArgs to be used and determines if the travel should be done 
        /// by parallel means or in a queue.
        /// </summary>
        /// <param name="args">Instructions for how to move the object.</param>
        public void RequestTravel(TravelDataArgs args) {
            // Check to see if the data needs to be queued
            if (args.isQueued) {
                this.travelDataObjects.Enqueue(new TravelData(args));
            } else {
                StartCoroutine(TravelNonQueued(new TravelData(args)));
            }
        }

        /// <summary>
        /// This is the array version to Request trave. Loops and passes the 
        /// indivuals into the normal version. This is not recommended for chained
        /// travels as this is placed in the queue and could happen after any request.
        /// </summary>
        /// <param name="args">Instructions for how to move the object.</param>
        public void RequestTravel(TravelDataArgs[] args) {
            for (int i = 0; i < args.Length; i++) {
                RequestTravel(args[i]);
            }
        }

        /// <summary>
        /// Moves the object to its destined location and calls a trigger if 
        /// provided. This is done as a corroutine to be done in parallel with 
        /// other movement paths or actions. This is the recommended travel 
        /// version for objects that need to be moved as a result of a 
        /// previous travel. Becareful as this is an independent movement that 
        /// may cause race conditions.
        /// </summary>
        /// <param name="data">The data used to determine its instructions.</param>
        private IEnumerator TravelNonQueued(TravelData data) {
            // Copy the data and set its initial time.
            TravelData travelData = data;
            travelData.SetStartTime();

            // Begin the loop
            while (true) {
                // Break the loop once destination is reached.
                if (CalculateNewTransform(ref travelData)) {
                    break;
                }
                yield return new WaitForEndOfFrame();
            }

            // Trigger callback
            travelData.FinishTravel();
        }

        /// <summary>
        /// A wrapper to deal with moving the transform objects
        /// </summary>
        /// <param name="data">A refrence to the instructions needed to travel.</param>
        /// <returns>The state if the object has reached its destination.</returns>
        private bool CalculateNewTransform(ref TravelData data) {
            // Check if the object was destroyed
            if (data.travelObject == null) {
                return true;
            }

            // Get the distance to travel by
            float distance = (Time.time - data.StartTime) * data.speed;

            // Check if the object needs to rotate or translate
            if (data.rotating) {
                // Slerp over the distance
                data.travelObject.localRotation = Quaternion.Slerp(
                  Quaternion.Euler(data.start),
                  Quaternion.Euler(data.end),
                  distance
                );
                // Check if done
                if (Quaternion.Angle(data.travelObject.localRotation, Quaternion.Euler(data.end)) < threshold) {
                    return true;
                }
            } else {
                // Lerp over the distance
                data.travelObject.localPosition = Vector3.Lerp(
                  data.start,
                  data.end,
                  distance
                );
                // Check if done
                if (data.travelObject.localPosition == data.end) {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}