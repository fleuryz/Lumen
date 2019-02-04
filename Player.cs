using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : Movivel
{

	public AudioClip sfx_entrar_bloco;
	public AudioClip sfx_sair_bloco;
	public AudioClip sfx_morrendo;
	public AudioClip[] sfx_subindo;
	public AudioClip sfx_nascendo;
	public AudioClip[] sfx_andando;

	private int inerciaQueda = 0;
    private Vector3 ultimaPosicao;

	public float restartLevelDelay = 1f;

	public Animator animator;
	public int pontos = 9999;

	private bool perderPontos = true;

	public bool cair = false;
	
	//Start overrides the Start function of MovingObject
	public override void Iniciar (bool infinito = false)
	{
		//Get a component reference to the Player's animator component
		animator = GetComponent<Animator>();

		sprite = GetComponent<SpriteRenderer>();

        base.Iniciar (infinito);
		podeMover = false;
		vivo = true;
	}
	
	
	//This function is called when the behaviour becomes disabled or inactive.
	private void OnDisable ()
	{
		
	}
	
	
	protected override void Update ()
	{
		base.Update ();

        Vector3 posReal = infinito ? GameManager.instance.infiniteBoard.PosReal(destino) : GameManager.instance.boardScript.PosReal (destino);
		float sqrRemainingDistance = (transform.localPosition - posReal).sqrMagnitude;

		if (sqrRemainingDistance <= float.Epsilon && movendo)
		{

			FicarIdle ();
			movendo = false;
			if (!dentro || cair)
				ChecarQueda ();

		}

		if (perderPontos)
			StartCoroutine (PerderPonto());
		if (dentro && sprite.sprite != null)
		{
			sprite.enabled = false;
		} else if (!sprite.enabled)
		{
			sprite.enabled = true;
		}

	}
	
	protected override void AttemptMove (int xDir, int zDir)
	{
        if (!vivo)
            return;
		Vector3 movimento = new Vector3 (-xDir, 0, zDir);

		if (ativar)
		{
			
			if (checar_bloco (posicaoAtual + movimento) && entrar_bloco (posicaoAtual + movimento))
			{
				//Há um bloco a frente onde se pode entrar
				if (dentro)
				{
					sair_bloco (posicaoAtual);
				}
				destino = posicaoAtual + movimento;
				movendo = true;
				andar = Move (destino);
				dentro = true;
				SoundManager.instance.PlaySingle (sfx_entrar_bloco);
			} else if (!checar_bloco(posicaoAtual + movimento) && ChecarMovBloco (posicaoAtual + movimento) && dentro)
			{
				//Não há bloco a frente
				Mudar ();
				destino = posicaoAtual + movimento;
				MoverBloco (destino);
				movendo = true;
				andar = Move (destino);
			}
		} else
		{
			if (checar_bloco (posicaoAtual + movimento))
			{
				//Há um bloco a frente
				movimento.y += 1;
				Vector3 teto = new Vector3(0,1,0);
				if (!dentro && !checar_bloco (posicaoAtual + movimento) && !checar_bloco (posicaoAtual + teto))
				{
					destino = posicaoAtual + movimento;
					movendo = true;
					Subir(destino);
				}
			} else
			{
				//Não há bloco a frente
				movimento.y -= 1;
				if (checar_bloco_ativo (posicaoAtual + movimento))
				{
					//Há um bloco para sustentar
					if (dentro)
					{
						sair_bloco (posicaoAtual);
						dentro = false;
						SoundManager.instance.PlaySingle (sfx_sair_bloco);
					}else
						SoundManager.instance.RandomSfxs (0, sfx_andando);

					movimento.y += 1;
					destino = posicaoAtual + movimento;
					movendo = true;
					andar = Move (destino);
					
				} else
				{
					movimento.y -= 1;
					if (checar_bloco_ativo (posicaoAtual + movimento))
					{
						//O bloco pode cair um bloco
						if (dentro)
						{
							sair_bloco (posicaoAtual);
							dentro = false;
						}
						movimento.y += 1;
						destino = posicaoAtual + movimento;
						movendo = true;
						Descer(destino);
					} else
					{
						movimento.y -= 1;
						if (checar_bloco_ativo (posicaoAtual + movimento))
						{
							//O bloco pode cair dois blocos
							if (dentro)
							{
								sair_bloco (posicaoAtual);
								dentro = false;
							}
							movimento.y += 1;
							destino = posicaoAtual + movimento;
							movendo = true;
							Descer(destino);
						}
					}
				}
			}
		}

		if (movendo)
		{
			if (xDir == 1)
				animator.SetBool ("andando_direita", true);
			else if (xDir == -1)
				animator.SetBool ("andando_esquerda", true);
			else if (zDir == 1)
				animator.SetBool ("andando_cima", true);
			else if (zDir == -1)
				animator.SetBool ("andando_baixo", true);
		}
	}

	protected bool Subir (Vector3 end)
	{
		andar = Move (end);
        if (infinito)
            GameManager.instance.infiniteBoard.AtualizarAltura((int)end.y);
		SoundManager.instance.RandomSfxs (0, sfx_subindo);
		GameManager.instance.ChecarSom ((int)end.y);
		return true;

	}

	protected bool   Descer (Vector3 end)
	{
		andar = Move (end);
		SoundManager.instance.RandomSfxs (0, sfx_subindo);
		return true;

	}

	protected bool Cair (Vector3 end)
	{
		andar = Move (end);
		SoundManager.instance.RandomSfxs (0, sfx_subindo);
		return true;

	}

	protected bool MoverBloco (Vector3 teste)
	{
        return infinito ? GameManager.instance.infiniteBoard.MoverBloco(posicaoAtual, teste) : GameManager.instance.boardScript.MoverBloco (posicaoAtual, teste);
	}

	public override void ChecarQueda ()
	{
		Vector3 queda;
			if (posicaoAtual != destino)
		{
			queda = destino + new Vector3 (0, -1, 0);
		} else
		{
			queda = posicaoAtual + new Vector3 (0, -1, 0);
		}

		if (!checar_bloco (queda) || cair)
		{
			if (!dentro || cair)
			{
				cair = false;
				inerciaQueda++;
				andar = Move (queda);
			}
		} else if (!checar_bloco_ativo (queda) || inerciaQueda > 2)
		{
			inerciaQueda = 0;
			Morrer ();
		} else
		{
			inerciaQueda = 0;
		}
			
	}

	public override Coroutine Move (Vector3 end)
	{
        ultimaPosicao = posicaoAtual;
        AtualizarDistancia();
        VerKevin();
		AtualizarPosicao (end);

		if (pontos > 9)
			pontos -= 10;
		else
			pontos = 0;

		Vector3 abaixo = destino + new Vector3 (0, -1, 0);
		Vector3 afrente = destino + new Vector3 (0, 0, 1);

		Coroutine andar = base.Move(end);
		avisar_bloco_abaixo (abaixo);
		avisar_bloco_afrente (afrente);
		avisar_bloco_dentro (destino);

		return andar;
	}

	public override void Remover(){
	
		Destroy (animator);
		this.transform.DetachChildren();
		base.Remover ();

	}

	public void DefinirRetorno(Vector3 novo)
	{
		retorno = novo;
	}

	public void Nascer()
	{
		if (!vivo)
		{
            if (infinito)
                GameManager.instance.Finalizar();
            else if(!GameManager.instance.editor)
                GameManager.instance.boardScript.LoadEstado ();
		}
		DefinirPosicao (retorno);
        AtualizarDistancia();
        if (infinito)
            this.transform.SetPositionAndRotation(GameManager.instance.infiniteBoard.PosReal(retorno), Quaternion.identity);
        else
    		this.transform.SetPositionAndRotation (GameManager.instance.boardScript.PosReal (retorno), Quaternion.identity);
		destino = posicaoAtual;
		podeMover = false;
		inerciaQueda = 0;
		SoundManager.instance.PlaySingle (sfx_nascendo);
		avisar_bloco_abaixo (posicaoAtual + new Vector3 (0, -1, 0));
	}

	public void Viver()
	{
		Reiniciar ();
		perderPontos = true;
		movendo = false;
		podeMover = true;
		vivo = true;
		GameManager.instance.ChecarSom ((int)posicaoAtual.y);
        if (GameManager.instance.editor && !EditorManager.instance.testando)
			Desativar ();
	}

	public void Morrer()
	{
		if (vivo)
		{
			vivo = false;
			FicarIdle ();
			podeMover = false;
			movendo = false;
			if (andar != null)
				StopCoroutine (andar);
			animator.SetTrigger ("morrer");
			SoundManager.instance.PlaySingle (sfx_morrendo);	
		}
	}

	IEnumerator PerderPonto()
	{
		if (pontos > 0)
		{
			perderPontos = false;
			yield return new WaitForSeconds (0.1f);
			pontos--;
			perderPontos = true;
		
		}else
			pontos = 0;
	}

	void FicarIdle()
	{
		animator.SetBool ("andando_direita", false);
		animator.SetBool ("andando_esquerda", false);
		animator.SetBool ("andando_cima", false);
		animator.SetBool ("andando_baixo", false);
	}

    void VerKevin()
    {
        AtualizarTransparencias(ultimaPosicao, 1.0f);
        AtualizarTransparencias(destino, 0.7f);


    }

    void AtualizarTransparencias(Vector3 posicao, float valor)
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    Bloco bloco;
                    Vector3 posicaoTeste = posicao + new Vector3(-i,j,-k);
                    if (mapa.TryGetValue(posicaoTeste, out bloco))
                    {
                        bloco.AtualizarTransparencia(valor);
                    }
                }
            }
        }
    }

    void AtualizarDistancia()
    {
        for (int i = -10; i < 11; i++)
        {
            for (int j = -10; j < 11; j++)
            {
                for (int k = -10; k < 11; k++)
                {
                    Vector3 posicaoTeste = posicaoAtual + new Vector3(i, j, k);
                    float distancia = Vector3.Distance(posicaoAtual, posicaoTeste);
                    if (distancia < 1.0f)
                        continue;
                    Bloco bloco;

                    if (mapa.TryGetValue(posicaoTeste, out bloco))
                    {
                        float valor = 1.1f - distancia*0.1f;
                        bloco.AtualizarCor(valor);
                    }
                }
            }
        }
    }

    public override void Ativar()
    {
        vivo = true;
        base.Ativar();
    }

}

