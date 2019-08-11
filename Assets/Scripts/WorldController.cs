using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts {
    public class WorldController : MonoBehaviour{

        public GameObject BlobPrefab;
        public GameObject FoodPrefab;

        public Text BlobCount;

        private WorldModel _worldModel;
        private List<BlobController> _blobs = new List<BlobController>();
        private List<BlobController> _blobsToAdd = new List<BlobController>();
        private List<FoodController> _foods = new List<FoodController>();
        

        private IEnumerable AliveBlobs => _blobs.Where(blobController => blobController.IsAlive);

        private IEnumerable AliveFoods => _foods.Where(foodController => foodController.isAlive);
        private IEnumerable EatenFoods => _foods.Where(foodController => !foodController.isAlive);


        private Vector3 RandomWorldPosition {
            get {
                var x = UnityEngine.Random.Range(-_worldModel.Size.x / 2, _worldModel.Size.x / 2);
                var z = UnityEngine.Random.Range(-_worldModel.Size.z / 2, _worldModel.Size.z / 2);

                return new Vector3(x, 0, z);
            }
        }


        private void Start() {
            InitializeWorld();
            InitializeBlobs();
            InitializeFoods();

            StartCoroutine(TickWorld());
        }

       

        private void Update() {

            BlobCount.text = $"Blob Count: {_blobs.Count.ToString()} \nFood Count: {_foods.Count.ToString()}";

            foreach (BlobController blob in AliveBlobs) {
                BlobCount.text = String.Concat(BlobCount.text, "\n", blob.name, ": ", blob.Energy);
            }
        }

        private IEnumerator TickWorld() {

            while (true) {

                TickFoods();
                TickBlobs();
                
                RemoveDeadBlobs();

                AddNewBlobs();

                yield return new WaitForSeconds(_worldModel.TickTimer);
            }
        }

        private void AddNewBlobs() {
            _blobs.AddRange(_blobsToAdd);
            _blobsToAdd.Clear();
        }

        private void TickFoods() {
            
            foreach (FoodController food in _foods) {

                food.Tick();
            }

        }

        private void RemoveDeadBlobs() {
            _blobs.RemoveAll(blobController => !blobController.IsAlive);
        }

        private void TickBlobs() {
            foreach (BlobController blob in AliveBlobs) {

                //if (blob.isActiveAndEnabled && !blob.IsDead)
                    blob.Tick();
            }
        }

        private void InitializeWorld() {
            _worldModel = WorldFactory.MakeWorld();
        }

        private void InitializeBlobs() {

            for (int i = 0; i < _worldModel.InitialBlobs; i++) {

                var blobModel = Factory.MakeBlobModel();
                CreateBlob(blobModel, RandomWorldPosition);
            }

        }

        private void InitializeFoods() {

            for (int i = 0; i < _worldModel.FoodCount; i++) {

                _foods.Add(Factory.InstantiateFood(FoodPrefab, RandomWorldPosition, foodController => foodController.Activate(RandomWorldPosition), _worldModel.FoodRegenerationTime));

            }


        }


        private void RemoveBlob(BlobController blob) {
            //_blobs.Remove(blob);
        }

        private void EatFood(BlobController blob, FoodController food) {

            blob.Energy += food.EnergyValue;

            food.SetEaten();
            
        }

        private void CreateBlob(BlobModel blobModel, Vector3 position) {

            _blobsToAdd.Add(Factory.InstantiateBlob(BlobPrefab, blobModel, _worldModel, position, RemoveBlob, CreateBlob, EatFood));
        }


    }
}
