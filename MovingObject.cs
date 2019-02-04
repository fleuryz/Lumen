using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {

	public float tamanho_x_bloco = 0.75f;
	public float tamanho_y_bloco = 0.875f;

	public float moveTime = 0.1f;
	public LayerMask blockingLayer;

	protected bool endMove = false;

	//private BoxCollider2D boxCollider;
	protected Rigidbody2D rb2D;
	protected float inverseMoveTime;
	public Vector3 posicaoAtual;

//	protected BoardManager mapa;	
	protected Dictionary<Vector3, Bloco> mapa;

	public const string LAYER_NAME = "Mapa";
	public int sortingOrder = 0;
	protected SpriteRenderer sprite;

	private bool voltarVelocidade = false;

	public bool podeMover = true;
	protected float moveTimePadrao = 0.1f;
	protected Coroutine andar;
	protected bool pausado = false;
    protected bool infinito = false;

	// Use this for initialization
	public virtual void Iniciar (bool infinito = false) {
        mapa = infinito ? GameManager.instance.infiniteBoard.mapa : GameManager.instance.boardScript.mapa;
        this.infinito = infinito;
        //boxCollider = GetComponent<BoxCollider2D> ();
        rb2D = GetComponent<Rigidbody2D>();
		inverseMoveTime = 1f / moveTime;
	}

	public virtual Coroutine Move (Vector3 end)
	{
		StartCoroutine (AtualizarPosicao (end));
		podeMover = false;
        return infinito? StartCoroutine(SmoothMovement(GameManager.instance.infiniteBoard.PosReal(end))) : StartCoroutine (SmoothMovement (GameManager.instance.boardScript.PosReal(end)));
	}

	protected IEnumerator SmoothMovement (Vector3 end)
	{
		float sqrRemainingDistance = (transform.localPosition - end).sqrMagnitude;
		
		while (sqrRemainingDistance > float.Epsilon)
		{
			Vector3 newPosition = Vector3.MoveTowards (rb2D.position, end, inverseMoveTime * Time.deltaTime);
			rb2D.MovePosition (newPosition);
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			yield return null;
		}
		if (moveTime != moveTimePadrao)
		{
			if (voltarVelocidade)
			{
				Mudar_MoveTime (moveTimePadrao);
				voltarVelocidade = false;
			}
			else
				voltarVelocidade = true;
		}
		podeMover = true;
		ChecarQueda ();
	}

	public bool checar_bloco(Vector3 teste)
	{
		Bloco bloco;

		if (mapa.TryGetValue (teste, out bloco))
		{
			return bloco.checar_bloco();
		}
		return false;
	}

	public bool checar_bloco_ativo(Vector3 teste)
	{
		Bloco bloco;

		if (mapa.TryGetValue (teste, out bloco))
		{
			return bloco.checar_bloco() && bloco.ativado;
		}
		return false;
	}

	public bool entrar_bloco(Vector3 teste)
	{
		Bloco bloco;

		if (mapa.TryGetValue (teste, out bloco))
		{
			 return bloco.entrar_bloco();
		}
		return false;
	}

	public bool sair_bloco(Vector3 teste)
	{
		Bloco bloco;

		if (mapa.TryGetValue (teste, out bloco))
		{
			bloco.sair_bloco();
			return true;
		}
		return false;
	}

	public bool avisar_bloco_abaixo(Vector3 teste)
	{
		Bloco bloco;

		if (mapa.TryGetValue (teste, out bloco))
		{
			bloco.avisar_bloco_abaixo();
			return true;
		}
		return false;	
	}

	public bool avisar_bloco_afrente(Vector3 teste)
	{
		Bloco bloco;

		if (mapa.TryGetValue (teste, out bloco))
		{
			bloco.avisar_bloco_afrente();
			return true;
		}
		return false;	
	}

	public bool avisar_bloco_dentro(Vector3 teste)
	{
		Bloco bloco;

		if (mapa.TryGetValue (teste, out bloco))
		{
			bloco.avisar_bloco_dentro();
			return true;
		}
		return false;	
	}

	public virtual void DefinirPosicao(Vector3 posicao)
	{
		GameManager.instance.AtualizarHUD (posicaoAtual, posicao);
		posicaoAtual = posicao;
        int columns = infinito? GameManager.instance.infiniteBoard.columns : GameManager.instance.boardScript.columns;
        int matrixes = infinito ? GameManager.instance.infiniteBoard.matrixes : GameManager.instance.boardScript.matrixes;
		sortingOrder = -(int)posicaoAtual.x - (int)posicaoAtual.z * columns + columns * matrixes + (int)posicaoAtual.y * columns * matrixes;
		sprite.sortingOrder = sortingOrder;
		sprite.sortingLayerName = LAYER_NAME;

	}

	public abstract void ChecarQueda ();

	public bool ChecarMovBloco (Vector3 destino)
	{
		if (GameManager.instance.JogBeta)
		{
			return (checar_bloco (destino + new Vector3 (0, -1, 0)));

		} else
		{
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					if (i != 0 && j != 0)
						continue;
					if (checar_bloco (destino + new Vector3 (i, -1, j)))
						return true;
				}
			}

			return false;
		}

	}

	protected void Mudar()
	{
		if (!GameManager.instance.mudou)
		{
			GameManager.instance.mudou = true;
			GameManager.instance.mudanca = posicaoAtual;
		}
	}

	public virtual void Remover(){
		Destroy (sprite);
		Destroy (this);

	}

	public virtual void Reiniciar()
	{
		podeMover = true;
		if(andar != null)
			StopCoroutine (andar);

        mapa = infinito ? GameManager.instance.infiniteBoard.mapa : GameManager.instance.boardScript.mapa;
	}

	public void Mudar_MoveTime(float novoTempo)
	{
		moveTime = novoTempo;
		inverseMoveTime = 1f / novoTempo;
	}

	public void Parar()
	{
		pausado = true;
	}

	public void Resumir()
	{
		pausado = false;
	}

	protected IEnumerator AtualizarPosicao(Vector3 posicao)
	{
		yield return new WaitForSeconds (moveTime / 3);
		DefinirPosicao (posicao);
	}
		

	public virtual void Ativar()
	{}

	public virtual void Desativar()
	{}
}


