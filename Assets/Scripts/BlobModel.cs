using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts {

    [Serializable]
    public class BlobModel
    {
        public static int REPLICATION_ENERGY_COST = 50;
        public static int REPLICATION_ENERGY_MINIMUM = 100;
        public static float SIZE_DIFFERENCE_TO_EAT = 0.8f;

        public int Id;

        public float MovementSpeed = 1;
        public float Size = 1;
        public float SensorRadius = 2f;

        public float AdjustedRadius => SensorRadius + Size / 2;

        public float Energy = 50;

        public float TimeToAcquireNewTarget = 5;
        public float RotationSpeed = 560;
        public float TimeToChangeDirection = 0.3f;
        public float ChangeDirectionRange = 90;
        public float TimeToFireSensor = 0.1f;
        public float DyingChance = 0.0001f;
        public float ReplicateChance = 0.02f;
        public Color Color = Color.white;

        public int Age = 0;

        public bool IsAlive = true;

        
    }

}
