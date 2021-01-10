using UnityEngine;
using UnityEngine.Networking;

namespace TPDespair.ZetAspects
{
    public class ZetSizeData : MonoBehaviour
	{
		public NetworkInstanceId netId;
		public Vector3 size;
		public Vector3 camera;
		public float scale;
		public float target;
		public bool playerControlled;
		public bool cameraModified;
	}
}
