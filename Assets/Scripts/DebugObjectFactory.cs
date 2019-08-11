using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts {
    public class DebugObjectFactory : MonoBehaviour {

        public GameObject DebugObject;

        public static DebugObjectFactory Instance => FindObjectOfType<DebugObjectFactory>();

        public void InstantiateAt(Vector3 position) {

            Instantiate(DebugObject, position, Quaternion.identity);

        }

        public void InstantiateAt(Vector3 position, string name) {

            var obj = Instantiate(DebugObject, position, Quaternion.identity);
            obj.name = name;

        }
    }
}
