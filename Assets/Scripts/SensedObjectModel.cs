using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts {
    public class SensedObjectModel {

        public static SensedObjectModel Build(Vector3 locatorPosition, Collider collider) {

            var point = collider.ClosestPoint(locatorPosition);
            point = new Vector3(point.x, locatorPosition.y, point.z);

            return new SensedObjectModel() {
                Collider = collider,
                ContactPoint = point,
                Distance = Vector3.Distance(locatorPosition, point),
                Tag = collider.tag
            };

        }

        public string Tag;
        public Collider Collider;
        public Vector3 ContactPoint;
        public float Distance;

    }
}
