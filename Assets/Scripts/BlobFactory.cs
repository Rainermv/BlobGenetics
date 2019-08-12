using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts {
    public class BlobFactory {

        private static int _idCounter = 0;

        public static BlobModel MakeBlobModel() {

            //var size = UnityEngine.Random.Range(0.2f, 3f);
            //var speed = 5 / size;
            //var sensor = size / 2 + UnityEngine.Random.Range(1f, 5f);

            return new BlobModel() {
                Id = _idCounter++,
                Color = UnityEngine.Random.ColorHSV(),
                //Size = size,
                //MovementSpeed = speed
            };

        }

        public static BlobModel CopyBlobModel(BlobModel parent) {

            return new BlobModel() {
                Color = parent.Color,
                Id = _idCounter++,
                Size = parent.Size,
                MovementSpeed = parent.MovementSpeed,
                SensorRadius = parent.SensorRadius
            };
        }

        public static BlobModel MutateBlobModel(BlobModel parent) {

            var child = MutationController.MutateFrom(parent);
            child.Id = _idCounter++;
            return child;
        }


            public static BlobController InstantiateBlob(
                GameObject prefab, 
                BlobModel blobModel, 
                WorldModel worldModel, 
                Vector3 position, 
                Action<BlobController> onDeath, 
                Action<BlobModel, Vector3> onReplicate,
                Action<BlobController, IEdible> onEat) 
            {

            var blobGameObject = GameObject.Instantiate(prefab, position, Quaternion.identity);

            var blobController = blobGameObject.GetComponent<BlobController>();
            
            blobController.Initialize(position, blobModel, worldModel, () => onDeath(blobController), onReplicate, edible => onEat(blobController, edible));

            return blobController;
        }

        
    }

}
