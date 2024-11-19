#if UNITY_EDITOR
using UnityEditor;

namespace ArtNetUnity.ArtNet
{
    [CustomEditor(typeof(ANUArtNetReceiver))]
    public class ANUArtNetReceiverEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}
#endif
