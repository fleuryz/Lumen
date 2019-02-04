using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocoAtrasa: Bloco
{
	public Sprite blocoAtrasa;
	private float velocidadeLento = 0.3f;
	private Animator animator;

	public override void Iniciar (bool infinito = false)
	{
		tipo = '3';

        base.Iniciar(infinito);
        animator = GetComponent<Animator>();
		sprite = GetComponent<SpriteRenderer> (); // we are accessing the SpriteRenderer that is attached to the Gameobject
		if (sprite.sprite == null)
		{
			sprite.sprite = blocoAtrasa;
		}
		string stringFase = string.Concat("fase_", ((fase - 1) % 4) + 1);
		animator.SetBool (stringFase, true);
		sprite.sortingOrder = sortingOrder;
		sprite.sortingLayerName = LAYER_NAME;
	}


	void Update ()
	{

	}

	public override void avisar_bloco_abaixo(){
        GameManager.instance.MudarVelocidadeKevin(velocidadeLento);
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