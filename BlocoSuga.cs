using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocoSuga: Bloco
{
	public Sprite blocoSuga;

	public AudioClip matando;
	public AudioClip som_constante;

	private Animator animator;

	private bool isMatando = false;

    private float tempoMatar = 0.4f;

	public override void Iniciar (bool infinito = false)
	{
		tipo = '2';

		animator = GetComponent<Animator>();

        base.Iniciar(infinito);
        sprite = GetComponent<SpriteRenderer> (); // we are accessing the SpriteRenderer that is attached to the Gameobject
		if (sprite.sprite == null)
		{
			sprite.sprite = blocoSuga;
		}
		string stringFase = string.Concat("fase_", ((fase - 1) % 4) + 1);
		animator.SetBool (stringFase, true);
		sprite.sortingOrder = sortingOrder;
		sprite.sortingLayerName = LAYER_NAME;
	}


	void Update ()
	{
	}

	public override void avisar_bloco_afrente(){
		if (!isMatando)
		{
			isMatando = true;
			StartCoroutine (Matar ());
		}

	}

	IEnumerator Matar()
	{
		animator.SetBool ("Ativado", true);
        yield return new WaitForSeconds(tempoMatar);
		isMatando = false;
		animator.SetBool ("Ativado", false);
		Vector3 teste = posicaoAtual + new Vector3 (0, 0, -1);
		
		if (kevin.posicaoAtual == teste && !kevin.dentro)
		{
			SoundManager.instance.PlaySingle (matando);
			GameManager.instance.KevinMorrer ();
		}
	}

	public override void ChecarAltura (bool ativar)
	{
		SomKevinProximo (som_constante);
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