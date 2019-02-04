using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp: Bloco
{
	public Sprite powerUp;
	public AudioClip pegou;
	private int pontos = 100;
	private bool pego = false;
	private Animator animator;

	public override void Iniciar (bool infinito = false)
	{
		tipo = '5';

		mapa = GameManager.instance.boardScript.mapa;
		//boxCollider = GetComponent<BoxCollider2D> ();
		rb2D = GetComponent<Rigidbody2D>();
		inverseMoveTime = 1f / moveTime;
		animator = GetComponent<Animator>();
		sprite = GetComponent<SpriteRenderer> (); // we are accessing the SpriteRenderer that is attached to the Gameobject
		if (sprite.sprite == null)
		{
			sprite.sprite = powerUp;
		}
		string stringFase = string.Concat("fase_", ((fase - 1) % 4) + 1);
		animator.SetBool (stringFase, true);
		sprite.sortingOrder = sortingOrder;
		sprite.sortingLayerName = LAYER_NAME;
	}


	void Update ()
	{

	}

	public override bool checar_bloco ()
	{
		//Debug.Log ("Kevin pegou?");
		return false;
	}

	public override void avisar_bloco_dentro ()
	{
		if (!pego)
		{
			pego = true;
			SoundManager.instance.PlaySingle (pegou);
			GameManager.instance.boardScript.kevin.pontos += pontos;
			Desativar ();
		}
	}

	protected override bool Cair (Vector3 end)
	{
		return true;

	}

	public override void ChecarQueda()
	{
	}

	public override void Desativar ()
	{
		animator.enabled = false;
    	GameManager.instance.boardScript.mapa.Remove (posicaoAtual);
		base.Desativar ();
		this.gameObject.SetActive (false);
	}

    public override void ModoEditor()
    {
        animator.enabled = false;
        base.ModoEditor();
    }

    public override void Reiniciar ()
	{
        this.gameObject.SetActive(true);
        animator.enabled = true;
		base.Reiniciar ();
	}

}