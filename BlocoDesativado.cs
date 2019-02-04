using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocoDesativado: Bloco
{
	public Sprite[] blocosDesativados;

	public override void Iniciar (bool infinito = false)
	{
		tipo = '>';

        base.Iniciar(infinito);
        sprite = GetComponent<SpriteRenderer> (); // we are accessing the SpriteRenderer that is attached to the Gameobject
        if(sprite.sprite == null || sprite.sprite != sEditor)
    		sprite.sprite = blocosDesativados[(fase-1)%4];
		sprite.sortingOrder = sortingOrder;
		sprite.sortingLayerName = LAYER_NAME;
	}

	void Update ()
	{
	}

	protected override void ReiniciarSprite ()
	{
		sprite.sprite = blocosDesativados[(fase-1)%4];
	}
}

