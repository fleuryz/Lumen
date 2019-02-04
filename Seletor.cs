using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seletor: Movivel
{
	public Sprite seletor;

	public char blocoUsado = '1';

	private BoardManager boardScript;

	private bool temFim = false;
	private Vector3 localFim = new Vector3();

	public override void Iniciar (bool infinito = false)
	{
        base.Iniciar (infinito);
		if (GameManager.instance != null)
			boardScript = GameManager.instance.boardScript;
		sprite = GetComponent<SpriteRenderer> (); // we are accessing the SpriteRenderer that is attached to the Gameobject
		if (sprite.sprite == null)
		{
			sprite.sprite = seletor;
		}
		sprite.color = new Color (1f, 1f, 1f, 0.5f);
		sprite.sortingOrder = sortingOrder;
		sprite.sortingLayerName = LAYER_NAME;
	}

	protected override void Update ()
	{
		base.Update ();
		if (podeMover && this.transform.childCount == 0)
		{
			this.Ativar ();
		}
	}

	public void RemoverBloco(Vector3 posicao)
	{
		Bloco blocoTemp;
		if (mapa.TryGetValue (posicao, out blocoTemp))
		{
			mapa.Remove (blocoTemp.posicaoAtual);
			Destroy (blocoTemp.gameObject);
		}
        EditorManager.instance.AtualizarHUD ('0', (int)posicaoAtual.x, (int)posicaoAtual.y, (int)posicaoAtual.z);
	}

	public void AdicionarObjeto(Vector3 posicao){
		Bloco blocoTemp;
		if (mapa.TryGetValue (posicao, out blocoTemp))
		{
			mapa.Remove (blocoTemp.posicaoAtual);
			Destroy (blocoTemp.gameObject);
		}
		if (blocoUsado == '9' && boardScript.kevin != null)
		{
			boardScript.kevin.transform.localPosition = boardScript.PosReal (posicao);
			boardScript.kevin.Iniciar();
			boardScript.kevin.DefinirRetorno (posicao);
			boardScript.kevin.animator.SetTrigger ("nascer");
		} else if (blocoUsado == '6' && temFim)
		{
			Bloco bloco;
			if (mapa.TryGetValue (localFim, out bloco))
			{
				bloco.transform.localPosition = boardScript.PosReal (posicao);
				mapa.Remove (localFim);
				mapa.Add (posicao, bloco);
			}
			
		}else
		{
			boardScript.AdicionarObjeto (blocoUsado, (int)posicao.x, (int)posicao.y, (int)posicao.z, true);
			boardScript.ChecarGravidade (new Vector3 (posicao.x-1, posicao.y-1, posicao.z));
		}

		if (blocoUsado == '9')
		{
			boardScript.kevin.Desativar ();
			Ativar ();
		} else if (blocoUsado == '6')
		{
			temFim = true;
			localFim = posicao;
		} else if (blocoUsado == ':' || blocoUsado == ':')
		{
			ConsertarSave ((int)posicao.y);
		}

        EditorManager.instance.AtualizarHUD (blocoUsado, (int)posicaoAtual.x, (int)posicaoAtual.y, (int)posicaoAtual.z);
	}
		
	public override void ChecarQueda()
	{
	}

	public override void Ativar ()
	{
		this.gameObject.SetActive(true);
		base.Ativar ();
	}

	public override void Desativar ()
	{
		base.Desativar ();
		this.gameObject.SetActive(false);
	}

	protected override void AttemptMove (int xDir, int zDir)
	{
		Vector3 movimento;
		if (!podeMover)
			return;
		if (ativar)
		{
			movimento = new Vector3 (-xDir, zDir, 0) + posicaoAtual;
		} else
		{
			movimento = new Vector3 (-xDir, 0, zDir) + posicaoAtual;
		}
		if(movimento.x < 0)
		{
			movimento.x = GameManager.instance.boardScript.columns-1;
		}else if(movimento.y < 0)
		{
			movimento.y = 0;

		}else if(movimento.z < 0)
		{
			movimento.z = 0;

		}else if(movimento.x >= GameManager.instance.boardScript.columns)
		{
			movimento.x = 0;
		}else if(movimento.y >= GameManager.instance.boardScript.rows)
		{
			movimento.y = GameManager.instance.boardScript.rows-1;
		}else if(movimento.z >= GameManager.instance.boardScript.matrixes)
		{
			movimento.z = GameManager.instance.boardScript.matrixes-1;
		}
		Move (movimento);


	}

	void ConsertarSave(int altura)
	{
		bool primeiro = true;
		for (int i = 0; i < GameManager.instance.boardScript.columns; i++)
		{
			for (int j = 0; j < GameManager.instance.boardScript.matrixes; j++)
			{
				Bloco bloco;
				if(mapa.TryGetValue(new Vector3(i, altura, j), out bloco))
				{
					char blocoUsado = ';';
					if (primeiro)
					{
						blocoUsado = ':';
						primeiro = false;
					}
					if (bloco.GetTipo () != blocoUsado)
					{
						mapa.Remove (bloco.posicaoAtual);
						Destroy (bloco.gameObject);
						boardScript.AdicionarObjeto (blocoUsado, i, altura, j, true);
					}
				}	

			}
		}
	}
}