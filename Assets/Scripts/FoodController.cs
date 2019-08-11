using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts {
    public class FoodController : MonoBehaviour {

        Action _onRegenerate;

        public int EnergyValue = 50;

        public bool isAlive = true;

        private int _regenerationTime;
        private int _regenerationTick;

        public void Initialize(Action onRegenerate, int regenerationTime) {

            _onRegenerate = onRegenerate;
            _regenerationTime = regenerationTime;

        }

        public void Tick() {

            if (isAlive)
                return;

            _regenerationTick--;

            if (_regenerationTick <= 0) {
                _onRegenerate();
            }

            // nothing for now
        }

        public void SetEaten() {
            isAlive = false;
            gameObject.SetActive(false);
        }

        public void Activate(Vector3 position) {

            isAlive = true;
            gameObject.SetActive(true);
            transform.position = new Vector3(position.x, 1, position.z);
            _regenerationTick = _regenerationTime;

        }


    }
}