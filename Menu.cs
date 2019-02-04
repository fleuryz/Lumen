using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.IO;

public class Menu : MonoBehaviour {

	public GameObject gameManager;

	public static Menu instance = null;


	public AudioClip selecionado;
	public AudioClip mudou_opcao;

	public Button b_Exit;
	public Button b_Settings;
	public Image i_Logo;

	public Button b_Jogar;
    public RectTransform t_Fases;
    public Button b_Padrao;
    public Button b_Usuario;
    public Button b_Infinito;
    public Button b_JogarInfinito;
    public Text t_highScoreAltura;
    public RectTransform t_Content;
	public Button b_Editor;
	public Button b_Creditos;
	public Button b_Settings2;

	public Button b_Continuar;
	public Button b_Sair;

	public Image fundo;
	public MenuAnimator kevin;
	public Text creditos;

	public Image i_logo2;
	public Slider s_Musica;
	public Slider s_Efeitos;
	public Toggle t_JogTest;
	public Button b_Exit2;

	public Text textoPontos;


	public AudioClip musicaMenu;
	public AudioClip musicaFase1;
	public AudioClip musicaFase2;

	private readonly Component fases;
	private int cor;

	public bool paused = false;
	private int menu = 1;

	public Image fundo2;
	public Image kevin2;

	public Sprite[] Fundos;

	public RectTransform MapaHUDHolder;
	public Image I_BlocoHUD;

	public Sprite[] HUDElements;

	public MenuAnimator powerup;
	public MenuAnimator suga;
	public MenuAnimator atraso;
	public MenuAnimator info;
	public MenuAnimator checkpoint;

    public GameObject botao;

	private Dictionary<Vector2, Image> mapaHUD= new Dictionary<Vector2, Image>();

	private bool mudarMenu = true;
	public int[] scores = new int[4];
    private bool usarFasesPadrao = true;
    private List<Fase> fasesPadrao = new List<Fase>();
    private List<Fase> fasesUsuario = new List<Fase>();
    private int highScoreAltura = 0;


    // Use this for initialization
    void Start () {

		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
		if (GameManager.instance == null)
		{
			Instantiate (gameManager);
		}

		cor = Random.Range (0, 3);
		fundo.sprite = Fundos [cor];

		powerup.Start ();
		suga.Start ();
		atraso.Start ();
		info.Start ();
		checkpoint.Start ();

		SoundManager.instance.PlayMusic (musicaMenu);
        BuscarFases();
	}
	
	// Update is called once per frame
	void Update () {
 		if (GameManager.instance.pause && !paused)
			AbriMenu ();
		else if (!GameManager.instance.pause && paused)
			SairMenu ();
		//Controle ();
		if (mudarMenu)
		{
			switch (menu)
			{
			case(0):
				SetMenu (0);

				break;
			case(1):
				SetMenu (1);
				b_Jogar.Select ();

				break;
			case(2):
				SetMenu (2);
				b_Continuar.Select ();
				break;
			case(3):
				SetMenu (3);
				s_Musica.Select();

				break;
			}	
		}

		if(menu == 0 && GameManager.instance.boardScript.kevin != null)
			textoPontos.text = "Points: " + GameManager.instance.boardScript.kevin.pontos;

		mudarMenu = false;
	}

	public void SetMenu(int menu)
	{
		this.menu = menu;
        MostrarCreditos(false);
        textoPontos.gameObject.SetActive(menu == 0);
		b_Settings2.gameObject.SetActive(menu == 0);
		MapaHUDHolder.gameObject.SetActive (menu == 0);

		b_Exit.gameObject.SetActive(menu == 1);
		b_Settings.gameObject.SetActive(menu == 1);
		i_Logo.gameObject.SetActive(menu == 1);
		b_Jogar.gameObject.SetActive(menu == 1);
		MostrarFases (false);
		b_Editor.gameObject.SetActive(menu == 1);
		b_Creditos.gameObject.SetActive(menu == 1);
		fundo.gameObject.SetActive(menu == 1);
		kevin.gameObject.SetActive(menu == 1);
		powerup.gameObject.SetActive(menu == 1);
		suga.gameObject.SetActive(menu == 1 && cor == 0);
		atraso.gameObject.SetActive(menu == 1 && cor == 1);
		info.gameObject.SetActive(menu == 1 && cor == 2);
		checkpoint.gameObject.SetActive(menu == 1 && cor == 3);
		if(menu == 1)
			AtivarAnimators ();

		b_Continuar.gameObject.SetActive(menu == 2);
		b_Sair.gameObject.SetActive(menu == 2);

		i_logo2.gameObject.SetActive(menu == 3);
		s_Musica.gameObject.SetActive(menu == 3);
		s_Efeitos.gameObject.SetActive(menu == 3);
		t_JogTest.gameObject.SetActive(menu == 3);
		b_Exit2.gameObject.SetActive(menu == 3);
	}

	public void AbriMenu()
	{

		menu = 2;
		mudarMenu = true;
		paused = true;
		GameManager.instance.Pause(true);
		
	}

	public void SairMenu()
	{
		menu = 0;
		mudarMenu = true;
		paused = false;
		GameManager.instance.Pause (false);
		
	}

	public void SairJogo()
	{
		Application.Quit ();
	}

	public void VoltarMenu()
	{
		
		if (GameManager.instance != null)
		{
			GameManager.instance.Finalizar ();
			//GameManager.instance.Resumir();
		}
		textoPontos.enabled = false;
		paused = false;
		menu = 1;
		mudarMenu = true;
        SoundManager.instance.StopSounds();
		SoundManager.instance.StopMusic ();
		SoundManager.instance.PlayMusic (musicaMenu);
		
	}

	public void IniciarFase (int fase)
	{
		
		SoundManager.instance.StopMusic ();
		if (fase % 2 == 0)
			SoundManager.instance.PlayMusic (musicaFase1);
		else
			SoundManager.instance.PlayMusic (musicaFase2);

		menu = 0;
		mudarMenu = true;
		if (GameManager.instance == null)
		{
			Instantiate (gameManager);
			GameManager.instance.JogBeta = t_JogTest.isOn;
		}
        GameManager.instance.infinito = false;
		textoPontos.enabled = true;

	}

	public void MostrarFases (bool mostrar){
        t_Fases.gameObject.SetActive(mostrar);
        if (mostrar)
            b_Padrao.Select();

    }

	public void MostrarCreditos(bool mostrar)
	{
		creditos.gameObject.SetActive (mostrar);
		fundo2.gameObject.SetActive (mostrar);
		fundo.gameObject.SetActive (!mostrar);
		kevin2.gameObject.SetActive (mostrar);
		kevin.gameObject.SetActive (!mostrar);
		suga.gameObject.SetActive(!mostrar && cor == 0);
		atraso.gameObject.SetActive(!mostrar && cor == 1);
		info.gameObject.SetActive(!mostrar && cor == 2);
		checkpoint.gameObject.SetActive(!mostrar && cor == 3);
	}

	public void MudarJogabilidade()
	{
		GameManager.instance.JogBeta = t_JogTest.isOn;
	}

	public void IniciarEditor()
	{
		SceneManager.LoadScene ("EditorFases");
	}

	public void MudarVolumeEfeitos()
	{
		if (SoundManager.instance != null)
			SoundManager.instance.VolumeEfeitos(s_Efeitos.value);
	}

	public void MudarVolumeMusicas()
	{
		if (SoundManager.instance != null)
			SoundManager.instance.VolumeMusica(s_Musica.value);
	}

	public void CriarHUD(int columns, int rows)
	{
		columns = 20;
        rows = 50;
		for (int i = 0; i < rows; i++)
		{
			for (int k = 0; k < columns; k++)
			{
				Image bloco;
				bloco = Instantiate<Image> (I_BlocoHUD, MapaHUDHolder);
				bloco.transform.localPosition = new Vector3 (10 *( columns - k - 1), 10 * i, 0);
//				bloco = Instantiate<Image> (I_BlocoHUD, new Vector3 (10 * k, 10 * i, 0), Quaternion.identity, MapaHUDHolder);
//				bloco = Instantiate<Image> (I_BlocoHUD, new Vector3 (MapaHUDHolder.localPosition.x + 10*k, MapaHUDHolder.localPosition.y + 10*i, 0), Quaternion.identity);
				mapaHUD.Add (new Vector2(k,i), bloco);	
			}
		}
	}

	public void AtualizarHUD(char tipo, int x, int y, int z, bool setup = false)
	{
		Image bloco;
		if (mapaHUD.TryGetValue (new Vector2 (x, y), out bloco))
		{
			if (setup && bloco.sprite == null)
			{
				bloco.sprite = HUDElements [GetHUDSprite (tipo)];
				if (bloco.sprite != null)
					bloco.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
			} else if (!setup)
			{
				bloco.sprite = HUDElements [GetHUDSprite (tipo)];
				if (bloco.sprite != null)
					bloco.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
				else
					bloco.color = new Color (1.0f, 1.0f, 1.0f, 0.0f);
			}
		}
	}

	int GetHUDSprite(char tipo)
	{	
		if (tipo == '9')
			return 3;
		if (tipo == '5' || tipo == '=' || tipo == 'N')
			return 4;
		if (tipo == 'D')
			return 2;
		if (tipo == 'K')
			return 1;
		return 0;
	}

	public bool MapaHUDMontado()
	{
		return mapaHUD.Count > 1;
	}

	public void LimparHUD()
	{
		foreach (Image bloco in mapaHUD.Values)
		{
			bloco.sprite = null;
			bloco.color = new Color (1.0f, 1.0f, 1.0f, 0.0f);
		}
	}

	public void CarregarScores()
	{

		string folderPath = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath);

		string path = folderPath  + "/FasesNovas/scores.txt";

		if (File.Exists (path))
		{
            StreamReader reader = new StreamReader(path);
            do
            {
                string[] dados = reader.ReadLine().Split(',');

                int indice = fasesPadrao.FindIndex(x => x.nomeArquivo == dados[0]);
                if (indice == -1)
                {
                    indice = fasesUsuario.FindIndex(x => x.nomeArquivo == dados[0]);
                    Fase temp = fasesUsuario[indice];
                    temp.score = int.Parse(dados[1]);
                    temp.AtualizarScore();
                    fasesUsuario[indice] = temp;
                }else
                {
                    Fase temp = fasesPadrao[indice];
                    temp.score = int.Parse(dados[1]);
                    temp.AtualizarScore();
                    fasesPadrao[indice] = temp;
                }
            } while (!reader.EndOfStream);


			reader.Close ();
		}


	}

	public void EscreverScores()
	{
		
        string folderPath = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath) + "/FasesNovas/";


        string path = folderPath + "scores.txt";


        if (!Directory.Exists(folderPath))
        {

            Directory.CreateDirectory(folderPath);
        }

        if (File.Exists(path))
            File.Delete(path);

        //Write some text to the test.txt file
        StreamWriter writer = System.IO.File.CreateText(path);
		string linha;

        foreach(Fase fase in fasesPadrao)
        {
            linha = fase.nomeArquivo + ',' + fase.score.ToString();
            writer.WriteLine(linha);
        }

        foreach (Fase fase in fasesUsuario)
        {
            linha = fase.nomeArquivo + ',' + fase.score.ToString();
            writer.WriteLine(linha);
        }

        writer.Close();
	}

	public void AtualizarTextos(int score, string nomeArquivo)
	{
        int indice = fasesPadrao.FindIndex(x => x.nomeArquivo == nomeArquivo);
        if (indice == -1){
            indice = fasesUsuario.FindIndex(x => x.nomeArquivo == nomeArquivo);
            Fase temp = fasesUsuario[indice];
            temp.score = score;
            temp.AtualizarScore();
            fasesUsuario[indice] = temp;
        }else
        {
            Fase temp = fasesPadrao[indice];
            temp.score = score;
            temp.AtualizarScore();
            fasesPadrao[indice] = temp;
        }
        EscreverScores ();
	}

    private void CarregarFases(bool padrao)
    {
        string path;
        if(padrao)
        {

            path = System.IO.Path.Combine(Application.streamingAssetsPath, "fases.txt");

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WebGLPlayer)
            {
                // Android only use WWW to read file
                WWW readerwww = new WWW(path);
                while (!readerwww.isDone)
                {
                }

                string realPath = Application.persistentDataPath + "/fases";
                System.IO.File.WriteAllBytes(realPath, readerwww.bytes);

                path = realPath;
            }
        }
        else
        {
            string folderPath = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath);

            path = folderPath + "/FasesNovas/fases.txt";
        }

        if (File.Exists(path))
        {
            //Read the text from directly from the test.txt file
            StreamReader reader = new StreamReader(path);
            int i = 0;
            do
            {
                string[] dados = reader.ReadLine().Split(',');
                string nome = dados[0];
                string nomeArquivo = dados[1];


                GameObject b_botao = Instantiate(botao, t_Content);
                b_botao.transform.localPosition = new Vector3(250, -35 - i * 70, 0);

                if (padrao)
                {
                    int fase = int.Parse(dados[2]);
                    fasesPadrao.Add(new Fase(0,nome, nomeArquivo, fase, (Button)b_botao.GetComponent("Button"), true));
                }else
                {
                    fasesUsuario.Add(new Fase(0, nome, nomeArquivo, (int)Random.Range(1, 4), (Button)b_botao.GetComponent("Button"), false));
                }

                i++;
            } while (!reader.EndOfStream);

            t_Content.sizeDelta = new Vector2(t_Content.sizeDelta.x, fasesUsuario.Count*70);
            reader.Close();
        }
    }

    private void BuscarFases()
    {
        CarregarFases(true);
        CarregarFases(false);

        CarregarScores();
        CarregarAltura();

        //AtualizarTextos();

        MudarFases(usarFasesPadrao);
    }

    public void MudarFases(bool padrao)
    {
        usarFasesPadrao = padrao;

        foreach (Fase fase in fasesPadrao)
        {
            fase.ApagarBotao(!padrao);
        }
        foreach (Fase fase in fasesUsuario)
        {
            fase.ApagarBotao(padrao);
        }

        b_JogarInfinito.gameObject.SetActive(false);
    }

    public void VerInfinito()
    {
        foreach (Fase fase in fasesPadrao)
        {
            fase.ApagarBotao(true);
        }
        foreach (Fase fase in fasesUsuario)
        {
            fase.ApagarBotao(true);
        }

        b_JogarInfinito.gameObject.SetActive(true);
    }

    private void AtivarAnimators()
    {
        string stringFase = string.Concat("fase_", (cor % 4) + 1);

        powerup.animator.SetBool(stringFase, true);
        suga.animator.SetBool(stringFase, true);
        atraso.animator.SetBool(stringFase, true);
        info.animator.SetBool(stringFase, true);
        checkpoint.animator.SetBool(stringFase, true);
    }

    public void IniciarInfinito()
    {
        SoundManager.instance.StopMusic();
        if (cor % 2 == 0)
            SoundManager.instance.PlayMusic(musicaFase1);
        else
            SoundManager.instance.PlayMusic(musicaFase2);

        menu = 0;
        mudarMenu = true;
        if (GameManager.instance == null)
        {
            Instantiate(gameManager);
            GameManager.instance.JogBeta = t_JogTest.isOn;
        }
        textoPontos.enabled = true;
        //TODO ativar um HUD com a atual altura do kevin
        GameManager.instance.highScoreAltura = highScoreAltura;
        GameManager.instance.IniciarInfinito(cor);
    }

    public void CarregarAltura()
    {
        t_highScoreAltura.text = "Play \nHighscore: " + highScoreAltura.ToString();

        string folderPath = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath);

        string path = folderPath + "/infinito.txt";

        if (File.Exists(path))
        {
            StreamReader reader = new StreamReader(path);
            highScoreAltura = int.Parse(reader.ReadLine());
            reader.Close();
            t_highScoreAltura.text = "Play \nHighscore: " + highScoreAltura.ToString();
        }
    }

    public void EscreverAltura(int altura)
    {
        highScoreAltura = altura;

        string folderPath = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath);
        t_highScoreAltura.text = "Play \nHighscore: " + highScoreAltura.ToString();

        string path = folderPath + "/infinito.txt";

        if (File.Exists(path))
            File.Delete(path);

        //Write some text to the test.txt file
        StreamWriter writer = System.IO.File.CreateText(path);
        writer.WriteLine(highScoreAltura);
        writer.Close();
    }
}
