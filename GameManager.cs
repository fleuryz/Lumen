	using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public float levelStartDelay = 2f;
	public float turnDelay = .1f;
	public static GameManager instance = null;
	public BoardManager boardScript;
    public InfiniteBoard infiniteBoard;
	public int playerFoodPoints = 100;
	[HideInInspector] public bool mudou = false;
	[HideInInspector] public Vector3 mudanca;
	[HideInInspector] public bool playersTurn = true;
	public bool JogBeta = false;
	private bool subir = false;

	public bool pause = false;

	private Text levelText;
	private GameObject levelImage;
	private int level = 0;
	private bool enemiesMoving;
	public bool editor = false;
	private bool emJogo = false;
	private Coroutine RotinaSom;
    private int highScore = 0;
    private string faseAtual = "temp";
    public bool infinito = false;
    private float tempoAltura = 10.0f;
    private readonly float tempoPadrao = 10.0f;
    public int highScoreAltura = 0;
    public int altura = 0;
    public bool puroInfinito = false;
    private Player kevin;

	// Use this for initialization
	void Awake ()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

        infinito = false;
        DontDestroyOnLoad(gameObject);
		boardScript = GetComponent<BoardManager>();
        infiniteBoard = GetComponent<InfiniteBoard>();
	}

	public void InitGame(int level, string nomeArquivo, bool padrao)
	{
        setLevel(level);
        AtualizarTempoAltura(tempoPadrao);
        faseAtual = nomeArquivo;
        boardScript.SetupScene (level, nomeArquivo, padrao);
        kevin = boardScript.kevin;
		subir = true;
		emJogo = true;
	}
		
	
	// Update is called once per frame
	void Update () {
		if (!emJogo)
			return;
        if (subir && !pause && kevin.posicaoAtual.y > 1)
			RotinaSom = StartCoroutine (MatarAltura());
		if (mudou)
		{
            if (infinito)
                infiniteBoard.ChecarGravidade(mudanca);
            else
                boardScript.ChecarGravidade (mudanca);
			mudou = false;
		}

	}

	private void setLevel(int novoLevel){
		level = novoLevel;
	}

	public void Finalizar()
	{
        altura = 0;
		Pause (false);
        if (infinito)
            infiniteBoard.Finalizar();
        else
    		boardScript.Finalizar ();
		subir = false;
		emJogo = false;
		if(RotinaSom != null)
			StopCoroutine (RotinaSom);
	}

	public void KevinMorrer()
	{
        if (infinito)
            infiniteBoard.kevin.Morrer();
        else
    		boardScript.kevin.Morrer ();
	}

	public void DefinirEditor(bool estado)
	{
		editor = estado;
	}

	public void Pause(bool estado)
	{
		if (estado)
		{
            if (infinito)
                infiniteBoard.Parar();
            else
			    boardScript.Parar ();
			pause = true;

            if(puroInfinito)
            {
                if (!MenuInfinito.instance.paused)
                    MenuInfinito.instance.AbriMenu();
            }
            else
            {
                if (!Menu.instance.paused)
                    Menu.instance.AbriMenu();

            }
            Time.timeScale = 0;
		} else
		{
            if (infinito)
                infiniteBoard.Resumir();
            else
                boardScript.Resumir ();
			pause = false;

            if(puroInfinito)
            {
                if (MenuInfinito.instance.paused)
                    MenuInfinito.instance.SairMenu();
            }
            else
            {
                if (Menu.instance.paused)
                    Menu.instance.SairMenu();

            }
            Time.timeScale = 1.0f;
		}
	}

	public void FinalizarFase()
	{
		if (editor)
		{
            EditorManager.instance.RetornarEditor ();
            //boardScript.ModoEditorFase ();
		} else
		{
            if (puroInfinito)
                MenuInfinito.instance.VoltarMenu();
            else
    			Menu.instance.VoltarMenu ();
		}
		emJogo = false;
	}

	private IEnumerator MatarAltura(){
		subir = false;
		yield return new WaitForSeconds (tempoAltura);
        if (infinito)
            infiniteBoard.MatarAltura();
        else
            boardScript.MatarAltura ();
        altura++;
		subir = true;
	}

	public void AtualizarHUD(Vector3 origem, Vector3 destino, bool fromKevin = false)
	{
        if (infinito)
            //infiniteBoard.AtualizarHUD(origem, destino, fromKevin);
        {}else
            boardScript.AtualizarHUD (origem, destino, fromKevin);
		
	}

	public void ChecarSom(int altura)
	{
        if (infinito)
            infiniteBoard.ChecarSom(altura);
            else
            boardScript.ChecarSom (altura);
		
	}

	public void ChecarScores()
	{
        if(infinito)
        {
            if (infiniteBoard.alturaKevin > highScoreAltura)
            {
                if (puroInfinito)
                    MenuInfinito.instance.EscreverAltura(infiniteBoard.alturaKevin);
                else
                    Menu.instance.EscreverAltura(infiniteBoard.alturaKevin);
            }

        }else if (!editor && boardScript.kevin.pontos > highScore)
		{
            Menu.instance.AtualizarTextos (boardScript.kevin.pontos, faseAtual);
		}
	}

	public void PauseEditor()
	{
        EditorManager.instance.Pause();
		
	}

    public void IniciarInfinito(int cor, bool puroInfinito = false)
    {
        this.puroInfinito = puroInfinito;
        AtualizarTempoAltura(tempoPadrao);
        infinito = true;
        setLevel(-1);
        faseAtual = "infinito";
        infiniteBoard.BoardSetup(cor, puroInfinito);
        kevin = infiniteBoard.kevin;
        subir = true;
        emJogo = true;
    }

    public void KevinNascer()
    {
        if (infinito)
            infiniteBoard.kevin.Nascer();
        else
            boardScript.kevin.Nascer();
    }

    public void KevinViver()
    {
        if (infinito)
            infiniteBoard.kevin.Viver();
        else
            boardScript.kevin.Viver();
    }

    public void AtualizarTempoAltura(float tempoNovo)
    {
        tempoAltura = tempoNovo;
    }
    public void MudarVelocidadeKevin(float velocidadeNova)
    {
        if (infinito)
            infiniteBoard.kevin.Mudar_MoveTime(velocidadeNova);
        else
            boardScript.kevin.Mudar_MoveTime(velocidadeNova);
    }

}
