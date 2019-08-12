using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts {
    class MutationController {

        private static readonly float COLOR_VARIATION = 0.1f;

        private static readonly float MOVEMENT_SPEED_VARIATION = 0.1f;
        private static readonly float SIZE_VARIATION = 0.1f;
        private static readonly float SENSOR_RADIUS_VARIATION = 0.1f;

        private static readonly float MOVEMENT_SPEED_MIN = 0.1f;
        private static readonly float SIZE_MIN = 0.1f;
        private static readonly float SENSOR_RADIUS_MIN = 0.1f;

        private static readonly float MOVEMENT_SPEED_MAX = 50f;
        private static readonly float SIZE_MAX = 10f;
        private static readonly float SENSOR_RADIUS_MAX = 10f;

        public static BlobModel MutateFrom(BlobModel parent) {

            return new BlobModel() {

                Color = MutateColor(parent.Color),
                MovementSpeed = MutateMovementSpeed(parent.MovementSpeed),
                Size = MutateSize(parent.Size),
                SensorRadius = MutateSensorRadius(parent.SensorRadius)
            };

        }

        private static Color MutateColor(Color parentColor) {

            var mutationR = UnityEngine.Random.Range(-COLOR_VARIATION, +COLOR_VARIATION);
            var mutationG = UnityEngine.Random.Range(-COLOR_VARIATION, +COLOR_VARIATION);
            var mutationB = UnityEngine.Random.Range(-COLOR_VARIATION, +COLOR_VARIATION);

            return new Color() {
                r = Mathf.Clamp01((parentColor.r + mutationR)),
                g = Mathf.Clamp01((parentColor.g + mutationG)),
                b = Mathf.Clamp01((parentColor.b + mutationB))
            };

        }

        private static float MutateMovementSpeed(float parentMovementSpeed) {
            
            var mutation = UnityEngine.Random.Range(-MOVEMENT_SPEED_VARIATION, +MOVEMENT_SPEED_VARIATION);

            return Mathf.Clamp(parentMovementSpeed + mutation, MOVEMENT_SPEED_MIN, MOVEMENT_SPEED_MAX);

        }

        private static float MutateSize(float parentSize) {

            var mutation = UnityEngine.Random.Range(-SIZE_VARIATION, +SIZE_VARIATION);

            return Mathf.Clamp(parentSize + mutation, SIZE_MIN, SIZE_MAX);
        }

        private static float MutateSensorRadius(float parentSensorRadius) {

            var mutation = UnityEngine.Random.Range(-SENSOR_RADIUS_VARIATION, +SENSOR_RADIUS_VARIATION);

            return Mathf.Clamp(parentSensorRadius + mutation, SENSOR_RADIUS_MIN, SENSOR_RADIUS_MAX);

        }

        

        

        
    }
}
