#if UNITY_EDITOR
using UnityEditor;

namespace ArtNetUnity.ArtNet
{
    [CustomEditor(typeof(ANUArtNetReciver))]
    public class ANUArtNetReciverEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}
#endif
