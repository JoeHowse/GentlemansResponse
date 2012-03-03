using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FrameAnimation : ScriptableObject {
	public string name = "";
	public float time;
	public Texture2D texture;
	public List<Frame> frames = new List<Frame>();
}
