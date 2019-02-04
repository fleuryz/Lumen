using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocoCobra: Bloco
{
	public Sprite blocoCobra;

	private bool fazerSom = false;
	private bool fazendoSom = false;
	private bool irDireita = false;
	private bool irEsquerda = false;
	private bool subir = false;
	private bool descer = false;
    private int eixo = 0;
    Vector3 checagem_0h = new Vector3(0, 0, 1);
    Vector3 checagem_1h = new Vector3(-1, 0, 1);
    Vector3 checagem_3h = new Vector3(-1, 0, 0);
    Vector3 checagem_5h = new Vector3(-1, 0, -1);
    Vector3 checagem_6h = new Vector3(0, 0, -1);
    Vector3 checagem_7h = new Vector3(1, 0, -1);
    Vector3 checagem_9h = new Vector3(1, 0, 0);
    Vector3 checagem_11h = new Vector3(1, 0, 1);
    private int contBloq = 0;

    public override void Iniciar (bool infinito = false)
	{
		tipo = '=';

        base.Iniciar(infinito);
        sprite = GetComponent<SpriteRenderer> (); // we are accessing the SpriteRenderer that is attached to the Gameobject
		if (sprite.sprite == null)
		{
			sprite.sprite = blocoCobra;
		}
		sprite.sortingOrder = sortingOrder;
		sprite.sortingLayerName = LAYER_NAME;
		moveTimePadrao = 0.7f;
        moveTime = 0.7f;
        inverseMoveTime = 1.0f / moveTimePadrao;
	}


	void Update ()
	{
		if (podeMover && !pausado && ativado)
			Mover ();
	}

	void Mover(){

        if (eixo == 0)
            BuscarEixo();

        if (contBloq > 3)
            Morrer();

        switch(eixo){
            case 0:
                Morrer();
                break;
            case 1:
                // Bloco acima
                if(!checar_bloco(posicaoAtual + checagem_3h)){
                    eixo = checar_bloco(posicaoAtual + checagem_1h) ? 1 : 2;
                    contBloq = 0;
                    andar = Move(posicaoAtual + new Vector3(-1, 0, 0)); //DIREITA
                }else{
                    eixo = 7;
                    contBloq++;
                    Mover();
                }
                break;
            case 2:
                // Bloco diagonal esquerda superior
                eixo = 3;
                if (checar_bloco(posicaoAtual + checagem_1h))
                    throw new UnityException();
                andar = Move(posicaoAtual + new Vector3(0, 0, 1)); //CIMA
                break;
            case 3:
                // Bloco esquerda
                if (!checar_bloco(posicaoAtual + checagem_0h))
                {
                    eixo = checar_bloco(posicaoAtual + checagem_11h) ? 3 : 4;
                    contBloq = 0;
                    andar = Move(posicaoAtual + new Vector3(0, 0, 1)); //CIMA
                }
                else
                {
                    eixo = 1;
                    contBloq++;
                    Mover();
                }
                break;
            case 4:
                // Bloco diagonal esquerda inferior
                eixo = 5;
                if (checar_bloco(posicaoAtual + checagem_11h))
                    throw new UnityException();
                andar = Move(posicaoAtual + new Vector3(1, 0, 0)); //ESQUERDA
                break;
            case 5:
                // Bloco abaixo
                if (!checar_bloco(posicaoAtual + checagem_9h))
                {
                    eixo = checar_bloco(posicaoAtual + checagem_7h) ? 5 : 6;
                    contBloq = 0;
                    andar = Move(posicaoAtual + new Vector3(1, 0, 0)); //ESQUERDA
                }
                else
                {
                    eixo = 3;
                    contBloq++;
                    Mover();
                }
                break;
            case 6:
                // Bloco diagonal direita inferior
                eixo = 7;
                if (checar_bloco(posicaoAtual + checagem_7h))
                    throw new UnityException();
                andar = Move(posicaoAtual + new Vector3(0, 0, -1)); //BAIXO
                break;
            case 7:
                // Bloco direita
                if (!checar_bloco(posicaoAtual + checagem_6h))
                {
                    eixo = checar_bloco(posicaoAtual + checagem_5h) ? 7 : 8;
                    contBloq = 0;
                    andar = Move(posicaoAtual + new Vector3(0, 0, -1)); //BAIXO
                }
                else
                {
                    eixo = 5;
                    contBloq++;
                    Mover();
                }
                break;
            case 8:
                // Bloco diagonal direita superior
                eixo = 1;
                if (checar_bloco(posicaoAtual + checagem_5h))
                    throw new UnityException();
                andar = Move(posicaoAtual + new Vector3(-1, 0, 0)); //DIREITA
                break;
        }


		//if (!irDireita && !irEsquerda)
		//{
		//	irDireita = checar_bloco (posicaoAtual + checagem_3h) || checar_bloco (posicaoAtual + checagem_0h);
		//	irEsquerda = !irDireita && checar_bloco (posicaoAtual + checagem_6h) || checar_bloco (posicaoAtual + checagem_9h);
		//}

		//if (irDireita)
		//{
			
		//	if (!checar_bloco (posicaoAtual + checagem_3h) && !descer)
		//	{
		//		irDireita = checar_bloco (posicaoAtual + checagem_1h);
		//		irEsquerda = !irDireita;
		//		subir = !irDireita;
		//		andar = Move (posicaoAtual + new Vector3 (-1, 0, 0)); //DIREITA
		//		return;
		//	} else if (!checar_bloco (posicaoAtual + checagem_6h))
		//	{
		//		descer = false;
		//		andar = Move (posicaoAtual + new Vector3 (0, 0, -1)); //BAIXO
		//		return;
		//	} else
		//	{
		//		irDireita = false;
		//		irEsquerda = true;
		//	}
				
		//}

		//if (irEsquerda)
		//{
		//	if (!checar_bloco (posicaoAtual + checagem_9h) && !subir)
		//	{
		//		irEsquerda = checar_bloco (posicaoAtual + checagem_7h);
		//		irDireita = !irEsquerda;
		//		descer = !irEsquerda;
		//		andar = Move (posicaoAtual + new Vector3 (1, 0, 0)); //ESQUERDA
		//		return;
		//	} else if (!checar_bloco (posicaoAtual + checagem_0h))
		//	{
		//		subir = false;
		//		andar = Move (posicaoAtual + new Vector3 (0, 0, 1)); //CIMA
		//		return;
		//	}
		//	if (!(checar_bloco (posicaoAtual + checagem_3h) && checar_bloco (posicaoAtual + checagem_6h)))
		//	{
		//		irDireita = true;
		//		return;
		//	}
				
		//}
		//Morrer ();

	}

    private void BuscarEixo()
    {
        if((checar_bloco(posicaoAtual + checagem_0h)))
        {
            eixo = 1;
        } else if ((checar_bloco(posicaoAtual + checagem_1h)))
        {
            eixo = 8;
        }
        else if ((checar_bloco(posicaoAtual + checagem_3h)))
        {
            eixo = 7;
        }
        else if ((checar_bloco(posicaoAtual + checagem_5h)))
        {
            eixo = 6;
        }
        else if ((checar_bloco(posicaoAtual + checagem_6h)))
        {
            eixo = 5;
        }
        else if ((checar_bloco(posicaoAtual + checagem_7h)))
        {
            eixo = 4;
        }
        else if ((checar_bloco(posicaoAtual + checagem_9h)))
        {
            eixo = 3;
        }
        else if ((checar_bloco(posicaoAtual + checagem_11h)))
        {
            eixo = 2;
        }
    }

    public override Coroutine Move (Vector3 end)
	{

		if (GameManager.instance.boardScript.kevin.posicaoAtual == end || GameManager.instance.boardScript.kevin.destino == end)
		{
			GameManager.instance.KevinMorrer ();
			return andar;
		}



		if (mapa.ContainsKey (end))
		{
			Bloco bloco;
			if (mapa.TryGetValue (end, out bloco))
			{
				Debug.Log(bloco.name);
				if (bloco.GetTipo () == '5')
					bloco.Desativar ();
				mapa.Remove(end);
			}
		}

		mapa.Remove(posicaoAtual);
		mapa.Add(end, this);

		if (fazerSom && !fazendoSom)
		{
			int distancia = 0;
			if (GameManager.instance.boardScript.kevin != null)
				distancia = (int)Mathf.Abs(GameManager.instance.boardScript.kevin.posicaoAtual.y - posicaoAtual.y);
			SoundManager.instance.PlaySingle (movendo, distancia);
			fazendoSom = true;
			StartCoroutine (PararSom ());
		}
		
		return base.Move (end);
	}

	public override bool checar_bloco ()
	{
		return false;
	}

	protected override bool Cair (Vector3 end)
	{
		return true;

	}

	public override void ChecarQueda()
	{
	}
	public override void avisar_bloco_dentro ()
	{
		GameManager.instance.KevinMorrer ();
	}

	public override void Desativar ()
	{
		base.Desativar ();
	}

	public override void Reiniciar ()
	{
		if(andar != null)
			StopCoroutine(andar);
		sprite.enabled = true;
		irDireita = false;
		irEsquerda = false;
		subir = false;
		descer = false;
		base.Reiniciar ();
	}

	protected override void ReiniciarSprite ()
	{
		sprite.sprite = blocoCobra;
	}

	public override void ChecarAltura (bool ativar)
	{
		fazerSom = ativar;
	}

	private IEnumerator PararSom()
	{
		yield return new WaitForSeconds(movendo.length);
		fazendoSom = false;
	}

	private void Morrer()
	{
		sprite.enabled = false;
		Desativar ();
	}

}