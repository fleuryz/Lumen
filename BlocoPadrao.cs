using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocoPadrao: Bloco
{
	public Sprite[] blocosPadrao;

	public Animator animator;
	public bool mudar = true;

	public override void Iniciar (bool infinito = false)
	{
		tipo = '1';

        base.Iniciar(infinito);
        animator = GetComponent<Animator>();
		sprite = GetComponent<SpriteRenderer> (); // we are accessing the SpriteRenderer that is attached to the Gameobject
		animator.enabled = false;
        if(sprite.sprite == null || sprite.sprite != sEditor)
    		sprite.sprite = blocosPadrao[(fase-1)%4];
		sprite.sortingOrder = sortingOrder;
		sprite.sortingLayerName = LAYER_NAME;
	}

	public void Update ()
	{
		if (!ativado)
			return;
		if (!dentro && (animator.enabled || mudar))
		{
			animator.enabled = false;
			if (sprite.sprite != blocosPadrao [(fase - 1) % 4])
				sprite.sprite = blocosPadrao [(fase - 1) % 4];
			else
				mudar = false;
		}
	}
		

	public override bool entrar_bloco ()
	{
		dentro = ativado;
		animator.enabled = true;
		animator.SetBool ("dentro", ativado);
		return ativado;
	}

	public override void sair_bloco ()
	{
		animator.SetBool ("dentro", false);
		animator.enabled = false;
		mudar = true;
		dentro = false;
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

