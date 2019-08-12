using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts {
    class FoodFactory {

        public static FoodController InstantiateFood(GameObject foodPrefab, Vector3 position, Action<FoodController> onRegenerate, int regenerationTime) {

            var foodGameObject = GameObject.Instantiate(foodPrefab, position, Quaternion.identity);

            var foodController = foodGameObject.GetComponent<FoodController>();

            foodController.Initialize(() => onRegenerate(foodController), regenerationTime);
            foodController.Activate(position);

            return foodController;

        }

    }
}
