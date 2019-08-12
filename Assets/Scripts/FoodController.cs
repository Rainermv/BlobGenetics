using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts {
    public class FoodController : MonoBehaviour, IEdible {

        public static int FOOD_ENERGY_VALUE = 50;

        Action _onRegenerate;

        private FoodModel _foodModel;

        private int EnergyValue = 50;

        public bool isAlive = true;

        private int _regenerationTime;
        private int _regenerationTick;

        public void Initialize(Action onRegenerate, int regenerationTime) {

            _onRegenerate = onRegenerate;
            _regenerationTime = regenerationTime;

            EnergyValue = FOOD_ENERGY_VALUE;

        }

        public void Tick() {

            if (isAlive)
                return;

            _regenerationTick--;

            if (_regenerationTick <= 0) {
                _onRegenerate();
            }
        }

        public void SetEaten() {
            isAlive = false;
            gameObject.SetActive(false);
        }

        public void Activate(Vector3 position) {

            position.y += 0.5f;

            isAlive = true;
            gameObject.SetActive(true);
            transform.position = position;
            _regenerationTick = _regenerationTime;

        }

        public float GetEnergyValue() {
            return EnergyValue;
        }
    }
}