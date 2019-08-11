using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts {
    public class LayerMaskUtilities {

        public static int IGNORE_LAYER = 9;

        public static int LayerMaskIgnore() {

            // Bit shift the index of the layer (8) to get a bit mask
            int layerMask = 1 << IGNORE_LAYER;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            return ~layerMask;
        }
    }
}
