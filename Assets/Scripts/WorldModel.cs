using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts {
    public class WorldModel {
        public int InitialBlobs = 10;
        public Vector3 Size = new Vector3(28,0,28);
        public float TickTimer = 1f;
        internal int FoodCount = 100;
        internal int FoodRegenerationTime = 10;
    }
}
