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

        public Terrain terrain;

        public Text LeftText;
        public Text RightText;
        public Slider SpeedSlider;

        public Button Speed1x;
        public Button Speed5x;
        public Button Speed10x;


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

                var pos = new Vector3(x, 0, z);
                pos.y = terrain.SampleHeight(pos);

                var y = terrain.SampleHeight(new Vector3(x, 0, z));

                return pos;
            }
        }


        private void Start() {
            InitializeWorld();
            InitializeBlobs();
            InitializeFoods();

            StartCoroutine(TickWorld());

            Speed1x.onClick.AddListener(() => SetSpeedSlider(1));
            Speed5x.onClick.AddListener(() => SetSpeedSlider(5));
            Speed10x.onClick.AddListener(() => SetSpeedSlider(10));

        }

        private void debuginst(float x, float z) {
            var pos = new Vector3(x, 0, z);
            pos.y = terrain.SampleHeight(pos);

            DebugObjectFactory.Instance.InstantiateAt(pos);
        }

        private void SetSpeedSlider(float value) {
            SpeedSlider.value = value;
        }
       

        private void UpdateUI() {

            LeftText.text = $"Blob Count: {_blobs.Count.ToString()}";

            var foodCount = 0;
            foreach (FoodController food in AliveFoods) {

                foodCount++;
            }

            LeftText.text += $"\nFood Count: {foodCount.ToString()}";

            int totalAge = 0;
            float totalSpeed = 0;
            float totalSize = 0;
            float totalSensor = 0;
            
            foreach (BlobController blob in AliveBlobs) {
                LeftText.text = String.Concat(LeftText.text, "\n", blob.name, ": ", blob.Energy);

                totalAge += blob.Age;
                totalSpeed += blob.MovementSpeed;
                totalSize += blob.Size;
                totalSensor += blob.SensorRange;
            }

            var blobsCount = Mathf.Max(1, _blobs.Count);

            RightText.text = $"Average Age: {totalAge / blobsCount}";
            RightText.text += $"\nAverage Speed: {totalSpeed / blobsCount}";
            RightText.text += $"\nAverage Size: {totalSize / blobsCount}";
            RightText.text += $"\nAverage Sensor: {totalSensor / blobsCount}";

            Time.timeScale = Mathf.Clamp(SpeedSlider.value, 0, 10);

        }

        private IEnumerator TickWorld() {

            while (true) {

                TickFoods();
                TickBlobs();
                
                RemoveDeadBlobs();

                AddNewBlobs();

                UpdateUI();

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

            TerrainFactory.GenerateHeights(terrain, 5, _worldModel.Ruggedness);
        }

        private void InitializeBlobs() {

            for (int i = 0; i < _worldModel.InitialBlobs; i++) {

                var blobModel = BlobFactory.MakeBlobModel();
                CreateBlob(blobModel, RandomWorldPosition);
            }

        }

        private void InitializeFoods() {

            for (int i = 0; i < _worldModel.FoodCount; i++) {

                _foods.Add(FoodFactory.InstantiateFood(FoodPrefab, RandomWorldPosition, foodController => foodController.Activate(RandomWorldPosition), _worldModel.FoodRegenerationTicks));

            }


        }


        private void RemoveBlob(BlobController blob) {
            //_blobs.Remove(blob);
        }

        private void EatFood(BlobController blob, IEdible food) {

            blob.Energy += food.GetEnergyValue();

            food.SetEaten();
            
        }

        private void CreateBlob(BlobModel blobModel, Vector3 position) {

            var blob = BlobFactory.InstantiateBlob(BlobPrefab, blobModel, _worldModel, position, RemoveBlob, CreateBlob, EatFood);
            blob.Terrain = terrain;


            _blobsToAdd.Add(blob);
        }


    }
}
