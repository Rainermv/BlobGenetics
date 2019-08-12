using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts {
    class BlobMouth : MonoBehaviour {

        public BlobController parent;

        public void OnTriggerEnter(Collider other) {

            if (other.name == parent.name)
                return;

            parent.MouthCollision(other);

        }

    }
}
