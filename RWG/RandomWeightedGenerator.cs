using System.Collections;
using UnityEngine;

namespace RandomGenerator {
    public class RandomWeightedGenerator : MonoBehaviour {
        public WeightedValues[] weights;

        private int total = 0;
        private bool updateTotal = true;

        public string GenerateValue() {
            if (this.updateTotal) UpdateTotal();

            string result = null;
            int processed = 0;
            int rng = Random.Range(1, 1 + this.total);

            foreach (WeightedValues wv in this.weights) {
                if (!wv.selectable) continue;

                processed += wv.weight;
                if (rng <= processed) {
                    result = wv.name;
                    break;
                }
            }
            return result;
        }

        private void UpdateTotal() {
            this.updateTotal = false;
            this.total = 0;
            foreach (WeightedValues wv in this.weights) {
                if (!wv.selectable) continue;

                this.total += wv.weight;
            }
        }
    }
}
