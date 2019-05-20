using UnityEngine;
using System.Collections;

public class TrailRendererSetting : MonoBehaviour {

	TrailRenderer tr;
	public string sortingLayerName;
	public int orderInLayer;

	// Use this for initialization
	void Start () {
		tr = GetComponent<TrailRenderer> ();
		tr.sortingLayerName = sortingLayerName;
		tr.sortingOrder = orderInLayer;
	}
}
