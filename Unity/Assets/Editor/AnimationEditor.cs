using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class AnimationEditor : EditorWindow {
	public FrameAnimation animation;
	public static GUISkin skin;
	public int frame = 0;
	
	public int frameWidth = 32;
	public int frameHeight = 32;
	public int count = 1;
	
	private bool makingRect = false;
	private Vector2 rectStart;
	
	[MenuItem("Images/EditAnimation %e")]
    static void OpenEditor() {
        var w = EditorWindow.GetWindow<AnimationEditor>();
        w.animation = Selection.activeObject as FrameAnimation;
        w.wantsMouseMove = true;
		w.frame = 0;
		skin = AssetDatabase.LoadAssetAtPath("Assets/EditorSkin.guiskin", typeof(GUISkin)) as GUISkin;
    }
	
	[MenuItem("Images/CreateAnimation %c")]
    static void CreateAnimation() {
		FrameAnimation fa = ScriptableObject.CreateInstance<FrameAnimation>();
		fa.frames = new List<Frame>();
		AssetDatabase.CreateAsset(fa, AssetDatabase.GenerateUniqueAssetPath("Assets/FrameAnimation.asset"));
		AssetDatabase.Refresh();
    }
	
	void OnGUI(){
		GUILayout.BeginArea(new Rect(0,0,600,600));
		
		animation.name = GUILayout.TextField(animation.name, 20, GUILayout.Width(100));
		GUILayout.BeginHorizontal(GUI.skin.box);
		GUILayout.Label("BUILD FROM ATLAS"); 
		GUILayout.Label("w"); 
		frameWidth = int.Parse(GUILayout.TextField(frameWidth.ToString()));
		
		GUILayout.Label("h"); 
		frameHeight = int.Parse(GUILayout.TextField(frameHeight.ToString()));
		
		GUILayout.Label("#"); 
		count = int.Parse(GUILayout.TextField(count.ToString()));
		
		GUILayout.FlexibleSpace();
		if(GUILayout.Button("Build")){
			animation.frames.Clear();
			for(int i = 0; i < count; i++){
				Frame f  = new Frame();
				f.texCoords = new Rect(i * frameWidth, 0, frameWidth, frameHeight);
				f.anchor = new Vector2(frameWidth/2, 0);
				animation.frames.Add(f);
			}
		}
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("prev")){
			if(frame != 0){
				frame--;	
			}
		}
		GUILayout.Label(string.Format("Frame {0} ({1})",frame, animation.frames.Count));
		if(GUILayout.Button("next")){
			if(frame < animation.frames.Count-1){
				frame++;	
			}
		}
		GUILayout.EndHorizontal();
		if(animation.texture && animation.frames.Count > frame){
			Rect r = new Rect(GUILayoutUtility.GetRect(animation.frames[frame].texCoords.width, animation.frames[frame].texCoords.height,GUILayout.ExpandWidth(false)));				
			
			r.width *= 3;
			r.height *= 3;
			Rect source =  new Rect(animation.frames[frame].texCoords);
			source.x = source.x / animation.texture.width;
			source.y = 1 - (source.y / animation.texture.height) - source.height/animation.texture.height;
			source.width = source.width/animation.texture.width;
			source.height = source.height/animation.texture.height;
			Graphics.DrawTexture(r ,animation.texture ,source ,0 ,0 ,0 ,0);
			foreach(CollisionBox box in animation.frames[frame].hitBoxes){
				Rect boxRect = new Rect(box.rect);
				boxRect.x *= 3;
				boxRect.y *= 3;
				boxRect.width*=3;
				boxRect.height*=3;
				GUI.Label(new Rect(r.x +boxRect.x, r.y+boxRect.y, boxRect.width, boxRect.height), "", skin.FindStyle("hitBox"));
			}
			if(Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.N){
				makingRect = true;
				rectStart = Event.current.mousePosition - new Vector2(r.x, r.y);
				Event.current.Use();
			}
			if(makingRect && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return){
				makingRect = false;
				Vector2 diff = Event.current.mousePosition - new Vector2(r.x, r.y) - rectStart;
				Rect newHitBox = new Rect(rectStart.x/3, rectStart.y/3, diff.x/3, diff.y/3);
				CollisionBox cb = new CollisionBox();
				cb.rect = newHitBox;
				animation.frames[frame].hitBoxes.Add(cb);
				Repaint();
				AssetDatabase.SaveAssets();
			}
		}
		GUILayout.EndArea();
		
	}
}
