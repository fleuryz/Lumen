using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bloco: MovingObject 
{
	protected char tipo;

	protected int fase;

	public Sprite desativado;

    public Sprite sEditor;

	public bool ativado = true;

	public AudioClip movendo;

	private int ALTURA = 10;

	protected bool somAtivo = false;

	public bool dentro = false;

    protected Player kevin;


	public virtual void iniciar (int camada, Vector3 posicao, int fase, bool infinito = false)
	{
		if (fase == 0)
			fase = 1;
		this.fase = fase;
        Iniciar (infinito);
		DefinirPosicao (posicao);
	}

    public override void Iniciar(bool infinito = false)
    {
        BuscarKevin();
        base.Iniciar(infinito);
    }

    public virtual bool checar_bloco ()
	{
		return true;
	}

	public virtual bool entrar_bloco ()
	{
		return false;	
	}

	public virtual void sair_bloco ()
	{
		
	}

	public virtual void avisar_bloco_afrente(){
	}

	public virtual void avisar_bloco_abaixo(){
	}

	public virtual void avisar_bloco_dentro()
	{
		
	}

	protected virtual bool Cair (Vector3 end)
	{
		Bloco bloco;
		if (mapa.TryGetValue (posicaoAtual, out bloco))
		{
			mapa.Remove (posicaoAtual);
			mapa.Add (end, bloco);
		}

		if (dentro)
		{
           
            kevin.cair = true;
            kevin.ChecarQueda();
			
		}
        if(end.y < GameManager.instance.altura)
        {
            Desativar();
        }
		Move (end);
		return true;

	}

	public char GetTipo()
	{
		return tipo;
	}

	public override void ChecarQueda()
	{
		Vector3 destino = posicaoAtual + new Vector3 (0, -1, 0);
		if (!ChecarMovBloco (posicaoAtual) && posicaoAtual.y > 0)
		{
			Cair (destino);
		}
	}

	public override void Desativar()
	{
		ativado = false;
		sprite.sprite = desativado;
	}

	public override Coroutine Move (Vector3 end)
	{
		return base.Move (end);
	}

	public virtual void ChecarAltura(bool ativar)
	{
	}

	protected void SomKevinProximo(AudioClip clip)
	{
		if (kevin != null && !somAtivo)
		{
			int posicaoKevin = (int)kevin.posicaoAtual.y;
			if (posicaoKevin + ALTURA >= posicaoAtual.y && posicaoKevin - ALTURA <= posicaoAtual.y)
			{
				SoundManager.instance.PlayEmCena (clip, (int)posicaoAtual.y);
				somAtivo = true;
			}

		} else if (somAtivo)
		{
			somAtivo = !SoundManager.instance.TocandoAltura ((int)posicaoAtual.y);
		}
	}

	public override void Reiniciar ()
	{
		ativado = true;
		sprite.sprite = null;
		ReiniciarSprite ();
		dentro = false;
		base.Reiniciar ();
	}

	protected virtual void ReiniciarSprite()
	{
		
	}

    public void AtualizarTransparencia(float valor)
    {
        Color temp = sprite.color;
        if (dentro)
        {
            temp.a = 1.0f;
            sprite.color = temp;
        }else
        {
            temp.a = valor;
            sprite.color = temp;
        }


    }

    public void AtualizarCor(float valor)
    {
        Color temp = sprite.color;
        temp.r = valor;
        temp.g = valor;
        temp.b = valor;
        sprite.color = temp;
    }

    public virtual void ModoEditor()
    {
        ativado = false;
        sprite.sprite = sEditor;
    }

    protected void BuscarKevin(){
        kevin = infinito ? GameManager.instance.infiniteBoard.kevin : GameManager.instance.boardScript.kevin;
    }

}