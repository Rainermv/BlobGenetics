
using UnityEngine;

namespace Assets.Scripts {

    class RotationHelper {

        public static Quaternion? GetOppositeLookRotation(Vector3 p1, Vector3 p2 ) {

            var oppositeDirection = Vector3.Normalize(p1 - p2);

            if (oppositeDirection == Vector3.zero)
                return null;

            return Quaternion.LookRotation(oppositeDirection);
        }

        public static Quaternion? GetTowardsLookRotation(Vector3 p1, Vector3 p2) {

            var towardsDirection = Vector3.Normalize(p2 - p1);

            if (towardsDirection == Vector3.zero)
                return null;

            return Quaternion.LookRotation(towardsDirection);
        }

    }
}
