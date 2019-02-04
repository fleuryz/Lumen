using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.IO;

public class BoardManager : MonoBehaviour {

	[Serializable]
	public class Count
	{
		public int maximum;
		public int minimum;

		public Count (int min, int max)
			{
				maximum = max;
				minimum = min;
			}
	}

	private Dictionary<Vector3, Bloco> save;

	public float tamanho_x_bloco = 0.75f;
	public float tamanho_y_bloco = 0.875f;
	public int columns = 20;
	public int rows = 30;
	public int matrixes = 50;
	public int num_base;
	public GameObject grid;
	public Bloco[] blocos;
	public Player kevin;
	public Player kevinScript;
	public int ultimoSave;
	public AudioClip matando_andar;

	public TextAsset[] Fases;

	public Dictionary<Vector3, Bloco> mapa= new Dictionary<Vector3, Bloco>();

	private int fase;
	private Transform gridHolder;
	private Transform boardHolder;
	private Vector3 posicaoInicial;
	private int ALTURA = 10;

	private int altura = 0;

	private int saveAltura = 0;

	private int numInfo = 0;

	public void BoardStart()
	{
		boardHolder = new GameObject ("Board").transform;
	}

    void BoardSetup (int fase, string nomeArquivo, bool padrao)
	{
		BoardStart ();
		this.fase = fase;
		ultimoSave = 0;
		altura = 0;
		numInfo = 0;

		string path = "";
		if(padrao)
		{
            path = System.IO.Path.Combine(Application.streamingAssetsPath, nomeArquivo + ".txt");

			if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WebGLPlayer)
			{
				// Android only use WWW to read file
				WWW readerwww = new WWW (path);
				while (!readerwww.isDone)
				{
				}

				string realPath = Application.persistentDataPath + "/fase";
				System.IO.File.WriteAllBytes (realPath, readerwww.bytes);

				path = realPath;
			}
		} else
		{
			string folderPath = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath);

            path = folderPath  + "/FasesNovas/" + nomeArquivo + ".txt";


			if (!File.Exists (path))
				return;

		}


		//Read the text from directly from the test.txt file
		StreamReader reader = new StreamReader(path); 
		string[] dados = reader.ReadLine().Split (',');
		//string[] dados = linhas[0].Split (',');
		columns = Int32.Parse(dados[0]);
		matrixes = Int32.Parse(dados[1]);
		rows = Int32.Parse(dados[2]);
		char[] buffer =  new char[1];

		if (GameManager.instance.editor)
		{
			if (!EditorManager.instance.MapaHUDMontado ())
                EditorManager.instance.CriarHUD (rows, matrixes, columns);
			else
                EditorManager.instance.LimparHUD ();
		} else
		{
			if (!Menu.instance.MapaHUDMontado ())
				Menu.instance.CriarHUD (columns, rows);
			else
				Menu.instance.LimparHUD ();
		}
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < matrixes; j++)
			{
				for (int k = 0; k < columns; k++)
				{
					reader.ReadBlock(buffer, 0, buffer.Length);
					//buffer[0] = linhas[1 + j + i*matrixes + i][k];
					AdicionarObjeto (buffer [0], k, i, j);
					if (GameManager.instance.editor && buffer[0] != '0')
                        EditorManager.instance.AtualizarHUD (buffer [0], k, i, j, true);
					else if (!GameManager.instance.editor && buffer[0] != '0')
						Menu.instance.AtualizarHUD (buffer [0], k, i, j, true);
					
				}
				reader.ReadBlock(buffer, 0, buffer.Length);
			}
			reader.ReadBlock(buffer, 0, buffer.Length);
		}
	
		SalvarEstado ();
		reader.Close();

		return;

	}

	public void SetupScene(int level, string nomeArquivo, bool padrao)
	{
        BoardSetup (level, nomeArquivo, padrao);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ChecarGravidade (Vector3 posicao)
	{
		
		
		for (int i = -1; i < 2; i++)
		{
			for (int j = -1; j < 2; j++)
			{
				if (i != 0 && j != 0)
					continue;
				Bloco bloco;
				Vector3 destino = posicao + new Vector3 (i,1,j);
				mapa.TryGetValue (destino, out bloco);
				if (bloco != null)
				{
					bloco.ChecarQueda ();
					ChecarGravidade (posicao + new Vector3(0,-1,0));
				}
			}
		}
	}

	public Vector2 PosReal(Vector3 posMapa)
	{

		return new Vector2 (-posMapa.x * tamanho_x_bloco + posMapa.z * tamanho_x_bloco, posMapa.y * tamanho_y_bloco + posMapa.x * tamanho_y_bloco / 2 + posMapa.z * tamanho_y_bloco / 2);
	}

	public void AdicionarObjeto(char tipo, int x, int y, int z, bool editor = false)
	{
		Bloco toInstantiate;
		if (tipo == '9')
		{
			posicaoInicial = new Vector3 (x, y, z);
			kevin = Instantiate<Player> (kevinScript, new Vector3 (-posicaoInicial.x * tamanho_x_bloco + posicaoInicial.z * tamanho_x_bloco, posicaoInicial.y * tamanho_y_bloco + posicaoInicial.x * tamanho_y_bloco / 2 + posicaoInicial.z * tamanho_y_bloco / 2, 0f), Quaternion.identity);
			kevin.Iniciar();
			kevin.DefinirRetorno (posicaoInicial);
			kevin.animator.SetTrigger ("nascer");
			return;

		} else if (tipo == '6')
		{
			toInstantiate = blocos [1];

		} else if (tipo == '2')
		{
			toInstantiate = blocos [2];

		} else if (tipo == ':')
		{
			toInstantiate = blocos [3];

		} else if (tipo == ';')
		{
			toInstantiate = blocos [4];

		}else if (tipo == '3')
		{
			toInstantiate = blocos [5];

		}else if (tipo == '5')
		{
			toInstantiate = blocos [6];

		}else if (tipo == '=')
		{
			//Cobra
			//toInstantiate = blocos [0];//TODO Arrumar isso aqui
			toInstantiate = blocos [7];

		}else if (tipo == '>')
		{
			toInstantiate = blocos [8];
		
		}else if (tipo == '4')
		{
			//BlocoInfo
			//toInstantiate = blocos [0];//TODO Arrumar isso aqui
			toInstantiate = blocos [9];

		}else if (tipo == 'D')
		{
			//Desativado
			toInstantiate = blocos [0];

		}  else if (tipo != '0')
		{
			toInstantiate = blocos [0];
		} else
		{
			return;
		}

		Bloco instance = Instantiate (toInstantiate, new Vector3 (-x * tamanho_x_bloco + z * tamanho_x_bloco, y * tamanho_y_bloco + x * tamanho_y_bloco / 2 + z * tamanho_y_bloco / 2, 0f), Quaternion.identity) as Bloco;

		Vector3 local = new Vector3 (x, y, z);

		instance.iniciar (-x - z * columns + num_base + y * columns * matrixes, local, fase);

		if (tipo == '4')
		{
			if(GameManager.instance.editor)
				((BlocoInfo)instance).ReceberInfo (EditorManager.instance.info);
			else
				((BlocoInfo)instance).LerInfo (numInfo);
			numInfo++;
		}

		if (tipo == 'D')
		{
			instance.Desativar ();
		}

		instance.transform.SetParent (boardHolder);

		mapa.Add (local, instance);	

		if (editor)
            instance.ModoEditor ();
	}

	public void Finalizar(){
		if (boardHolder == null)
			return;
		foreach (Transform child in boardHolder)
		{
			Destroy(child.gameObject);
		}
		if (kevin != null)
		{
			kevin.Remover ();
			Destroy (kevin.gameObject);
		}
		Destroy (boardHolder.gameObject);
		mapa.Clear();
		Debug.Log ("Comecando a apagar");
	}

	public void SalvarEstado(){
		save = new Dictionary<Vector3, Bloco>(mapa);
	}

	public void LoadEstado()
	{
		mapa = new Dictionary<Vector3, Bloco> (save);
		foreach (KeyValuePair<Vector3, Bloco> i in mapa)
		{
			i.Value.DefinirPosicao (i.Key);
			i.Value.transform.SetPositionAndRotation (GameManager.instance.boardScript.PosReal (i.Key), Quaternion.identity);
			i.Value.Reiniciar ();
		}
		altura = saveAltura;
	}

	public void Parar()
	{
		foreach (MovingObject child in mapa.Values)
		{
			child.Parar ();

		}
	}

	public void Resumir()
	{
		foreach (MovingObject child in mapa.Values)
		{
			child.Resumir ();
		}
	}

	public bool MoverBloco(Vector3 origem, Vector3 destino)
	{
		Bloco bloco;

		if (mapa.TryGetValue (origem, out bloco))
		{
			mapa.Remove (origem);
			if (mapa.ContainsKey (destino))
			{
				Bloco bloco2;
				mapa.TryGetValue (destino, out bloco2);
				if (bloco.dentro && bloco2.GetTipo () == '5')
					bloco2.avisar_bloco_dentro ();
				else if (bloco2.GetTipo () == '5')
					bloco2.Desativar ();
			}
			mapa.Add (destino, bloco);

			bloco.Move(destino);
			return true;

		}
		return false;

	}

	public void SetGrid()
	{
		int x, y, z;

		gridHolder = new GameObject ("Grid").transform;
		y = 0;
		for (int i = 0; i <= columns-1; i++)
		{
			for (int j = 0; j <= matrixes-1; j++)
			{
				x = i;
				z = j;
				GameObject instance = Instantiate (grid, new Vector3 (-x * tamanho_x_bloco + z * tamanho_x_bloco, y * tamanho_y_bloco + x * tamanho_y_bloco / 2 + z * tamanho_y_bloco / 2, 0f), Quaternion.identity);
				instance.transform.SetParent (gridHolder);
			}	
		}
	}

    public void EscreverMapa(string nome)
	{
		ApagarInfo ();

        string nomeArquivo = nome.Normalize();



        string folderPath = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath)+ "/FasesNovas/";
      
 

        string path = folderPath  + nomeArquivo + ".txt";


        if (!Directory.Exists(folderPath))
        {

            Directory.CreateDirectory(folderPath);
        }


        StreamWriter writer = File.CreateText(path);
        string linha;
		linha = columns.ToString() + ',' + matrixes.ToString() + ',' + rows.ToString();
		writer.WriteLine(linha);

		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < matrixes; j++)
			{
				for (int k = 0; k < columns; k++)
				{

					Bloco bloco;
					Vector3 posicao = new Vector3 (k, i, j);
					mapa.TryGetValue (posicao, out bloco);
					if (bloco != null)
					{
						writer.Write (bloco.GetTipo ());
						if(bloco.GetTipo () == '4')
                            EscreverInfo(nomeArquivo, ((BlocoInfo)bloco).texto);
					} else if (kevin != null && kevin.posicaoAtual == posicao)
					{
						writer.Write ('9');
					}else
						writer.Write ('0');

				}
				writer.Write ('\n');
			}
			writer.Write ('\n');
		}
		writer.Close();

        path = folderPath + "fases.txt";

        StreamWriter writer2 = !File.Exists(path) ? File.CreateText(path) : new StreamWriter(path, true);

        linha = nome + "," + nomeArquivo;

        writer2.WriteLine(linha);

        writer2.Close();
    }

	public void MatarAltura()
	{
		SoundManager.instance.PlaySingle (matando_andar, (int)kevin.posicaoAtual.y - altura);
		for (int i = 0; i < columns; i++)
		{
			for (int j = 0; j < matrixes; j++)
			{
				for (int k = 0; k <= altura; k++)
				{
					Bloco bloco;
					if(mapa.TryGetValue(new Vector3(i, k, j), out bloco))
					{
						bloco.Desativar();
						Menu.instance.AtualizarHUD ('D',i, k, j);
						mapa.Remove (new Vector3(i, k, j));
					}	
				}

			}
		}

		altura++;

		if (kevin.posicaoAtual.y == altura || kevin.destino.y == altura)
		{
			GameManager.instance.KevinMorrer ();
		}
	}

	public void Salvar(float altura)
	{

		ultimoSave = (int)altura;
		saveAltura = this.altura;
		Debug.Log ("Salvando!");

		for (int i = 0; i < columns; i++)
		{
			for (int j = 0; j < matrixes; j++)
			{
				Bloco bloco;
				Vector3 posicao = new Vector3 (i, altura, j);
				mapa.TryGetValue (posicao, out bloco);
				if (bloco != null)
				{
						
					if (bloco.GetTipo () == ':' || bloco.GetTipo () == ';')
					{
						bloco.avisar_bloco_abaixo ();
					}
				}
			}
		}
		SalvarEstado ();
		kevin.DefinirRetorno (GameManager.instance.boardScript.kevin.destino);

	}

	public void AtualizarHUD(Vector3 origem, Vector3 destino, bool fromKevin)
	{
		if (fromKevin)
		{
			if (GameManager.instance.editor)
			{
                EditorManager.instance.AtualizarHUD ('K', (int)EditorManager.instance.seletor.posicaoAtual.x, (int)EditorManager.instance.seletor.posicaoAtual.y, (int)EditorManager.instance.seletor.posicaoAtual.z);
			} else
			{
				char kevinHUD = kevin.dentro ? 'K' : '9';
				Menu.instance.AtualizarHUD (kevinHUD, (int)kevin.posicaoAtual.x, (int)kevin.posicaoAtual.y, (int)kevin.posicaoAtual.z);
			}
			return;
		}
		
		for (int i = 0; i < matrixes; i++){
			Bloco bloco;
			if (mapa.TryGetValue (new Vector3 (origem.x, origem.y, i), out bloco))
			{
				if (GameManager.instance.editor)
				{
                    EditorManager.instance.AtualizarHUDF (bloco.GetTipo (), (int)origem.x, (int)origem.y, i);
					break;
				} else
				{
					Menu.instance.AtualizarHUD (bloco.GetTipo (), (int)origem.x, (int)origem.y, i);
					break;
				}
			}
			if (i == matrixes - 1)
			{
				if (GameManager.instance.editor)
				{
                    EditorManager.instance.AtualizarHUDF ('N', (int)origem.x, (int)origem.y, i);
					break;
				} else
				{
					Menu.instance.AtualizarHUD ('N', (int)origem.x, (int)origem.y, i);
					break;
				}
			}
		}

		for (int i = 0; i < matrixes; i++)
		{
			Bloco bloco;
			if (mapa.TryGetValue (new Vector3 (destino.x, destino.y, i), out bloco))
			{
				if (GameManager.instance.editor)
				{
                    EditorManager.instance.AtualizarHUDF (bloco.GetTipo (), (int)destino.x, (int)destino.y, i);
					break;
				} else
				{
					Menu.instance.AtualizarHUD (bloco.GetTipo (), (int)destino.x, (int)destino.y, i);
					break;
				}
			}
			if (i == matrixes - 1)
			{
				if (GameManager.instance.editor)
				{
                    EditorManager.instance.AtualizarHUDF ('N', (int)destino.x, (int)destino.y, i);
					break;
				} else
				{
					Menu.instance.AtualizarHUD ('N', (int)destino.x, (int)destino.y, i);
					break;
				}
			}
		}

		if (GameManager.instance.editor)
		{
			for (int i = 0; i < columns; i++){
				Bloco bloco;
				if (mapa.TryGetValue (new Vector3 (i, origem.y, origem.z), out bloco))
				{
                    EditorManager.instance.AtualizarHUDL (bloco.GetTipo (), i, (int)origem.y, (int)origem.z);
					break;
				}
				if (i == columns - 1)
				{
                    EditorManager.instance.AtualizarHUDL ('N', i, (int)origem.y, (int)origem.z);
					break;
				}
			}

			for (int i = 0; i < columns; i++)
			{
				Bloco bloco;
				if (mapa.TryGetValue (new Vector3 (i, destino.y, destino.z), out bloco))
				{
                    EditorManager.instance.AtualizarHUDL (bloco.GetTipo (), i, (int)destino.y, (int)destino.z);
					break;
				}
				if (i == columns - 1)
				{
                    EditorManager.instance.AtualizarHUDL ('N', i, (int)destino.y, (int)destino.z);
					break;
				}
			}
		}

	}

	public void ChecarSom(int altura)
	{
		for (int i = 0; i < columns; i++)
		{
			for (int j = 0; j < matrixes; j++)
			{
				Bloco bloco;
				if(mapa.TryGetValue(new Vector3(i, altura + ALTURA, j), out bloco))
				{
					bloco.ChecarAltura (false);

				}

				for (int k = -ALTURA+1; k < ALTURA; k++)
				{
					if(mapa.TryGetValue(new Vector3(i, altura + k, j), out bloco))
					{
						bloco.ChecarAltura (true);
					}
				}

				if(mapa.TryGetValue(new Vector3(i, altura - ALTURA, j), out bloco))
				{
					bloco.ChecarAltura (false);
				}
			}
		}
	}

	public void AtivarFase()
	{
		foreach (Bloco bloco in mapa.Values)
		{
			bloco.Reiniciar ();
		}
	}

	public void DesativarFase()
	{
		foreach (Bloco bloco in mapa.Values)
		{
			bloco.Desativar ();
		}
	}

    public void ModoEditorFase()
    {
        foreach (Bloco bloco in mapa.Values)
        {
            bloco.ModoEditor();
        }
    }

    public void ApagarInfo()
	{

		string folderPath = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath);

		string path = folderPath  + "/temp_texto.txt";


		if(File.Exists(path))
			File.Delete (path);
	}

	public void EscreverInfo(string nomeArquivo, string texto)
	{
		string folderPath = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath);

		string path = folderPath  + "/" + nomeArquivo + "_texto.txt";

		//Write some text to the test.txt file
		StreamWriter writer = new StreamWriter(path, true);
		writer.WriteLine(texto);
		writer.Close();
	}
}
