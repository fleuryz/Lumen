using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocoFim: Bloco
{
	public Sprite blocoFim;

	public AudioClip fim_fase;
	public AudioClip som_bloco;

	public override void Iniciar (bool infinito = false)
	{
		tipo = '6';

        base.Iniciar(infinito);
        sprite = GetComponent<SpriteRenderer> (); // we are accessing the SpriteRenderer that is attached to the Gameobject
		SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();

		sprites [1].sortingOrder = sortingOrder + 1;
		sprites [1].sortingLayerName = LAYER_NAME;

		sprites [2].sortingOrder = sortingOrder + 2;
		sprites [2].sortingLayerName = LAYER_NAME;


		if (sprite.sprite == null)
		{
			sprite.sprite = blocoFim;
		}
		sprite.sortingOrder = sortingOrder;
		sprite.sortingLayerName = LAYER_NAME;
	}



	public override void avisar_bloco_abaixo(){
		GameManager.instance.boardScript.kevin.Parar();
		StartCoroutine (AcabarFase());


	}

	public override void ChecarQueda ()
	{
		
	}

	IEnumerator AcabarFase()
	{
		SoundManager.instance.PlaySingle (fim_fase);
        GameManager.instance.boardScript.kevin.vivo = false;
		yield return new WaitForSeconds (1.0f);
		GameManager.instance.ChecarScores ();
		GameManager.instance.FinalizarFase ();
	}

	public override void ChecarAltura (bool ativar)
	{
		SomKevinProximo (som_bloco);
	}

	protected override void ReiniciarSprite ()
	{
		sprite.sprite = blocoFim;
	}

    public override void DefinirPosicao(Vector3 posicao)
    {
        base.DefinirPosicao(posicao);

        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();

        sprites[1].sortingOrder = sortingOrder + 1;
        sprites[1].sortingLayerName = LAYER_NAME;

        sprites[2].sortingOrder = sortingOrder + 2;
        sprites[2].sortingLayerName = LAYER_NAME;

    }

}