using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimator : MonoBehaviour {

	public Animator animator;
	public Sprite image;
	protected SpriteRenderer sprite;

	// Use this for initialization
	public void Start () {
		animator = GetComponent<Animator>();
		sprite = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
