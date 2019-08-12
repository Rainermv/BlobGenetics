using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts {
    class BlobDecider {

        private const string TAG_FOOD = "Food";
        private const string TAG_WALL = "Wall";
        private const string TAG_BLOB = "Blob";

        public static void DecideOnSensedClosestObject(BlobController blobController, SensedObjectModel sensedObject) {

            switch (sensedObject.Tag) {

                case TAG_WALL:
                    blobController.RunAwayFrom(sensedObject.ContactPoint);
                    Debug.Log(blobController.name + " running from wall");
                    return;

                case TAG_FOOD:
                    blobController.MoveTowards(sensedObject.ContactPoint);
                    return;

                case TAG_BLOB:

                    var otherBlob = sensedObject.Collider.GetComponent<BlobController>();

                    if (blobController.CanEat(otherBlob)) {
                        blobController.MoveTowards(sensedObject.ContactPoint);
                        return;
                    }

                    if (otherBlob.CanEat(blobController)) {
                        blobController.RunAwayFrom(sensedObject.ContactPoint);
                        return;
                    }
  
                    return;

            }


        }
    }
}
