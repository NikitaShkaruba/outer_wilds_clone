using UnityEngine;

namespace Common
{
    /**
     * Class with some blender models integration methods
     */
    public static class BlenderBugFixes
    {
        /**
         * Imported from blender models are rotated strangely
         * @link https://www.immersivelimit.com/tutorials/blender-to-unity-export-correct-scale-rotation
         */
        public static Vector3 TransformBlenderEulerAngles(Vector3 vector)
        {
            vector.x += 270;
            vector.y += 180;

            return vector;
        }
    }
}
