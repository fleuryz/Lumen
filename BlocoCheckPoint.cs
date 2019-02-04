using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocoCheckPoint: Bloco
{
	public Sprite blocoCheckPoint;

	private Animator animator;

	public AudioClip[] salvando;

	public override void Iniciar (bool infinito = false)
	{
		tipo = ':';

		animator = GetComponent<Animator>();

        base.Iniciar(infinito);
        sprite = GetComponent<SpriteRenderer> (); // we are accessing the SpriteRenderer that is attached to the Gameobject
		if (sprite.sprite == null)
		{
			sprite.sprite = blocoCheckPoint;
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
		return true;
	}

	public override bool entrar_bloco ()
	{
		return false;
	}

	public override void sair_bloco ()
	{
	}

	protected override bool Cair (Vector3 end)
	{
		return true;

	}

	public override void ChecarQueda()
	{
	}

	public override void avisar_bloco_abaixo(){
		animator.SetBool ("Ativado", true);
		SoundManager.instance.RandomSfxs (1, salvando);
		if (GameManager.instance.boardScript.ultimoSave < posicaoAtual.y)
		{
			GameManager.instance.boardScript.Salvar (posicaoAtual.y);
		}

	}

	public override void Desativar ()
	{
		animator.enabled = false;
		base.Desativar ();
	}

    public override void ModoEditor()
    {
        animator.enabled = false;
        base.ModoEditor();
    }

    public override void Reiniciar ()
	{
		animator.enabled = true;
		base.Reiniciar ();
	}
}