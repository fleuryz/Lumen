using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.IO;

public class EditorManager : MonoBehaviour {

	private BoardManager boardScript;

	private GameManager gameManager;

	public static EditorManager instance = null;

	private MonoBehaviour[] elementos;

	private char blocoUsado = '1';

	private bool mudarMenu = true;

	public Seletor seletor;

	//private Image logo;

	public GameObject grid;

	public Button b_Settings;

	public Button b_Resume;
	public Button b_Return;
	public Button b_Padrao;
	public Button b_Desativado;
	public Button b_Absorve;
	public Button b_Checkpoint;
	public Button b_Atraso;
	public Button b_Fim;
	public Button b_Cobra;
	public Button b_Info;
	public Button b_Powerup;
	public Button b_Kevin;
	public Button b_Outros;
	public Button b_Salvar;
	public Button b_Carregar;
	public Button b_Testar;
	public Button b_Voltar;
	public Button b_Adicionar_Bloco;
	public Button b_Remover_Bloco;
    public Button b_Voltar_Carregar;

	public AudioClip musicaEditor;

	public InputField inputInfo;
	public Text textoInfo;
	public Button botaoInfo;

	public int menu = 0;

	public RectTransform MapaHUDHolderF;
	public RectTransform MapaHUDHolderL;
	public Image I_BlocoHUD;
	public string info;

	public Sprite[] HUDElements;

    public RectTransform t_Content;
    public GameObject botao;
    public ScrollRect nomesFases;
    public Button b_BotaoNome;

    public bool testando;
    private string nomeArquivo = "temp";
    private List<Fase> fases = new List<Fase>();

    private Dictionary<Vector2, Image> mapaHUDF = new Dictionary<Vector2, Image>();
	private Dictionary<Vector2, Image> mapaHUDL = new Dictionary<Vector2, Image>();

    private readonly string stringInfo = "Digite a mensagem do bloco de informação.";
    public string stringNome = "Digite o nome da fase.";

    // Use this for initialization
    void Start () {

		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
				
		boardScript = GameManager.instance.boardScript;
        //Comente para partes do infinito
		boardScript.columns = 20;
		boardScript.rows = 50;
		boardScript.matrixes = 30;
		
        /*//Descomente para partes do infinito
        boardScript.columns = 10;
        boardScript.rows = 10;
        boardScript.matrixes = 10;
*/
        boardScript.BoardStart ();
		boardScript.SetGrid ();
		SoundManager.instance.PlayMusic (musicaEditor); //TIRAR O COMENTÁRIO
		seletor = Instantiate(seletor, new Vector3 (0,0,0), Quaternion.identity);
		seletor.Iniciar();
		seletor.Move (new Vector3(0,0,0));
		seletor.Desativar ();
		GameManager.instance.DefinirEditor (true);
		CriarHUD (50,30,20);
        SetMenu(0);
        seletor.gameObject.SetActive(true);
        seletor.Ativar();
        SelecionarBloco();
    }

	// Update is called once per frame
	void Update () {
 
	}

	public void DefinirBloco(string i)
	{
		blocoUsado = i [0];
		seletor.blocoUsado = blocoUsado;
	}

	public void VoltarMenuInicial()
	{
		GameManager.instance.DefinirEditor (false);
		GameManager.instance.Pause (false);
		SceneManager.LoadScene ("Scene1");
	}

	public void SetMenu(int menu)
	{
		this.menu = menu;
		b_Settings.gameObject.SetActive(menu == 0) ;
		b_Adicionar_Bloco.gameObject.SetActive (menu == 0);
		b_Remover_Bloco.gameObject.SetActive (menu == 0);
		MapaHUDHolderF.gameObject.SetActive (menu == 0);
		MapaHUDHolderL.gameObject.SetActive (menu == 0);

		b_Resume.gameObject.SetActive(menu == 1);
		b_Return.gameObject.SetActive(menu == 1);
		b_Padrao.gameObject.SetActive(menu == 1);
		b_Desativado.gameObject.SetActive(menu == 1);
		b_Absorve.gameObject.SetActive(menu == 1);
		b_Checkpoint.gameObject.SetActive(menu == 1);
		b_Atraso.gameObject.SetActive(menu == 1);
		b_Fim.gameObject.SetActive(menu == 1);
		b_Cobra.gameObject.SetActive(menu == 1);
		b_Info.gameObject.SetActive(menu == 1 || menu == 3);
		b_Powerup.gameObject.SetActive(menu == 1);
		b_Kevin.gameObject.SetActive(menu == 1);
		b_Outros.gameObject.SetActive (menu == 1);

		b_Salvar.gameObject.SetActive (menu == 2);
		b_Carregar.gameObject.SetActive (menu == 2);
		b_Testar.gameObject.SetActive (menu == 2);
		b_Voltar.gameObject.SetActive (menu == 2);

		inputInfo.gameObject.SetActive (menu == 3 || menu == 5);
		textoInfo.gameObject.SetActive (menu == 3 || menu == 5);
        if (menu == 3)
            textoInfo.text = stringInfo;
        else if (menu == 5)
            textoInfo.text = stringNome;
        botaoInfo.gameObject.SetActive (menu == 3);

        nomesFases.gameObject.SetActive (menu == 4);
        b_Voltar_Carregar.gameObject.SetActive(menu == 4);

        b_BotaoNome.gameObject.SetActive (menu == 5);


        mudarMenu = false;
		seletor.podeMover = (menu == 0);
		if (menu == 0 && GameManager.instance.pause)
		{
			GameManager.instance.Pause (false);
		}
	}

	private void SelecionarBloco(){
		switch (blocoUsado)
		{
		case('1'):
			b_Padrao.Select ();
			break;
		case('2'):
			b_Absorve.Select ();
			break;
		case('3'):
			b_Atraso.Select ();
			break;
		case('4'):
			b_Info.Select ();
			break;
		case('5'):
			b_Powerup.Select ();
			break;
		case('6'):
			b_Fim.Select ();
			break;
		case('9'):
			b_Kevin.Select ();
			break;
		case(':'):
			b_Checkpoint.Select ();
			break;
		case('='):
			b_Cobra.Select ();
			break;
		case('>'):
			b_Desativado.Select ();
			break;
		}
	}

    public void MostrarFases()
    {
        CarregarFases();
        SetMenu(4);
    }

	public void CarregarTemp(){
        stringNome = "Type level name.";
        boardScript.Finalizar ();
        boardScript.SetupScene (1, nomeArquivo, false);
        boardScript.ModoEditorFase();
        seletor.Ativar ();
		if(boardScript.kevin != null)
			boardScript.kevin.Desativar ();
		GameManager.instance.Pause (true);
		SetMenu (0);
	}
	public void TestarTemp(){
		if (boardScript.kevin != null)
		{
			seletor.Desativar ();
			boardScript.kevin.Ativar ();
			boardScript.SalvarEstado ();
			boardScript.AtivarFase ();
			GameManager.instance.Pause (false);
            testando = true;
			SetMenu (0);
		}
	}

	public void RetornarEditor()
	{
		boardScript.LoadEstado ();
        boardScript.ModoEditorFase();
		seletor.Reiniciar ();
		boardScript.kevin.Reiniciar ();
		boardScript.kevin.Nascer ();
		boardScript.kevin.Desativar ();
		seletor.Ativar ();
        testando = false;
		SetMenu (0);
	}

	public void AdicionarBloco()
	{
		seletor.AdicionarObjeto (seletor.posicaoAtual);
	}

	public void RemoverBloco()
	{
		seletor.RemoverBloco (seletor.posicaoAtual);
	}

	public void CriarHUD(int rows, int matrixes, int columns)
	{
		columns = 20;
		matrixes = 30;
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < matrixes; j++)
			{
				Image bloco2;
				bloco2 = Instantiate<Image> (I_BlocoHUD, MapaHUDHolderL);
				bloco2.transform.localPosition = new Vector3 (10 * -( matrixes - j - 1), 10 * i, 0);
				mapaHUDL.Add (new Vector2(j,i), bloco2);
			}
		}

		for (int i = 0; i < rows; i++)
		{
			for (int k = 0; k < columns; k++)
			{
				Image bloco1;
				bloco1 = Instantiate<Image> (I_BlocoHUD, MapaHUDHolderF);
				bloco1.transform.localPosition = new Vector3 (10 * (columns - k - 1), 10 * i, 0);
				mapaHUDF.Add (new Vector2 (k, i), bloco1);
			}
		}
	}

	public void AtualizarHUD(char tipo, int x, int y, int z, bool setup = false)
	{
		AtualizarHUDF (tipo, x, y, z, setup);

		AtualizarHUDL (tipo, x, y, z, setup);
	}

	public void AtualizarHUDF(char tipo, int x, int y, int z, bool setup = false)
	{
		Image bloco;
		if (mapaHUDF.TryGetValue (new Vector2(x,y), out bloco))
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

	public void AtualizarHUDL(char tipo, int x, int y, int z, bool setup = false)
	{
		Image bloco;
		if (mapaHUDL.TryGetValue (new Vector2(z,y), out bloco))
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
		return mapaHUDF.Count > 1;
	}

	public void LimparHUD()
	{
		foreach (Image bloco in mapaHUDF.Values)
		{
			bloco.sprite = null;
			bloco.color = new Color (1.0f, 1.0f, 1.0f, 0.0f);
		}

		foreach (Image bloco in mapaHUDL.Values)
		{
			bloco.sprite = null;
			bloco.color = new Color (1.0f, 1.0f, 1.0f, 0.0f);
		}
	}

	public void ConfirmarInfo()
	{
		SetMenu (0);
		info = inputInfo.text;
	}

    public void ConfirmarNome()
    {
        CarregarFases();
        if (fases.FindIndex(x => x.nomeArquivo == inputInfo.text) != -1)
        {
            stringNome = "Name already used. Use another.";
            return;
        }
        nomeArquivo = inputInfo.text;
        boardScript.EscreverMapa(nomeArquivo);
        SetMenu(0);


    }

    public void Pause()
    {
        switch (menu)
        {
            case (0):
                GameManager.instance.boardScript.Parar();
                GameManager.instance.pause = true;
                if (boardScript.kevin != null)
                    boardScript.kevin.Desativar();
                seletor.gameObject.SetActive(true);
                if (testando)
                    RetornarEditor();
                testando = false;
                SetMenu(1);
                break;
            case (1):
                boardScript.Resumir();
                GameManager.instance.pause = false;
                SetMenu(0);
                break;
            case (2):
                SetMenu(1);
                break;
            case (3):
                SetMenu(1);
                break;
            case (4):
                SetMenu(2);
                break;
            case (5):
                SetMenu(2);
                break;
            default:
                break;

        }
    }

    private void CarregarFases()
    {
        fases.Clear();

        string path;
        string folderPath = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath);

        path = folderPath + "/FasesNovas/fases.txt";

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

                fases.Add(new Fase(0, nome, nomeArquivo, 1, (Button)b_botao.GetComponent("Button"), false, true));

                i++;
            } while (!reader.EndOfStream);
            reader.Close();
        }
    }

    public void DefinirNomeArquivo(string novoNome)
    {
        nomeArquivo = novoNome;
    }
}
