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


        public int Id;

        public float MovementSpeed = 1;
        public float Size = 1;
        public float SensorRadius = 1f;

        public float Energy = 50;

        public float TimeToAcquireNewTarget = 5;
        public float RotationSpeed = 160;
        public float TimeToChangeDirection = 0.3f;
        public float ChangeDirectionRange = 90;
        public float TimeToFireSensor = 0.1f;
        public float DyingChance = 0.05f;
        public float ReplicateChance = 0.02f;
        public Color Color = Color.white;

        public bool IsAlive = true;
    }

}
