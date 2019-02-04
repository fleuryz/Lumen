using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.IO;

public class InfiniteBoard : MonoBehaviour
{

    public float tamanho_x_bloco = 0.75f;
    public float tamanho_y_bloco = 0.875f;
    public int columns = 10;
    public int rows = 10;
    public int matrixes = 10;
    public int num_base;
    public Bloco[] blocos;
    public Player kevin;
    public Player kevinScript;
    public AudioClip matando_andar;

    public Dictionary<Vector3, Bloco> mapa = new Dictionary<Vector3, Bloco>();
    public Dictionary<Vector3, Bloco> mapaDesativado = new Dictionary<Vector3, Bloco>();

    public List<Parte>[] partes = new List<Parte>[10];

    private Transform boardHolder;
    private readonly int ALTURA = 10;

    private int altura = 0;

    private int atualSaida;
    private int alturaSaida;
    private int colunaSaida;
    private int linhaSaida;
    private int fase;
    public int alturaKevin;
    private int alturaMaxima;
    public bool morrer = false;
    private int ultimaColunaMorta = 0;
    private int ultimaMatrizMorta = 0;
    private int ultimaColunaHUD = 0;
    private int ultimaAlturaHUD = 0;
    private int ultimaMatrixHUD = 0;
    private bool puroInfinito = false;

    public void BoardStart()
    {
        boardHolder = new GameObject("Board").transform;
    }

    public void LerPartes()
    {
        for (int i = 0; i < 10; i++)
            partes[i] = new List<Parte>();

        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "Desafios/partes.txt");

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // Android only use WWW to read file
            WWW readerwww = new WWW(path);
            while (!readerwww.isDone)
            {
            }

            string realPath = Application.persistentDataPath + "/desafios";
            System.IO.File.WriteAllBytes(realPath, readerwww.bytes);

            path = realPath;
        }
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        string[] dados;
        do
        {
            dados = reader.ReadLine().Split(',');
            partes[int.Parse(dados[1])-1].Add(new Parte(dados[0], int.Parse(dados[2])));
        
        } while (!reader.EndOfStream);

        reader.Close();
       
    }


    public int BuscarParte(int entradaMaxima)
    {
        int novaColunaSaida = 0, novaLinhaSaida = 0;
        bool continuar = false;
        int entrada = 0;
        int contagem = 0;
        while (!continuar)
        {
            entrada = Random.Range(0, entradaMaxima);
            continuar = partes[entrada].Count > 0;
            contagem++;
            if(contagem == 100){
                continuar = true;
                Debug.Log("Entrou em loop infinito 2.");
                Debug.Log(entradaMaxima);
                Debug.Log(entrada);
                Debug.Log(partes[entrada]);
                Debug.Break();
                Application.Quit();
            }
        }
        Parte parte = partes[entrada][Random.Range(0, partes[entrada].Count)];

        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "Desafios/" + parte.GetNome() + ".txt");

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // Android only use WWW to read file
            WWW readerwww = new WWW(path);
            while (!readerwww.isDone)
            {
            }

            string realPath = Application.persistentDataPath + "/parte";
            System.IO.File.WriteAllBytes(realPath, readerwww.bytes);

            path = realPath;
        }


        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        char[] buffer = new char[1];

        if(!puroInfinito)
        {
            if (!Menu.instance.MapaHUDMontado())
                Menu.instance.CriarHUD(columns, rows);
            else
                Menu.instance.LimparHUD();

        }else
        {
            if (!MenuInfinito.instance.MapaHUDMontado())
                MenuInfinito.instance.CriarHUD(columns, rows);
            else
                MenuInfinito.instance.LimparHUD();
        }

        int distancia = 0;
        bool primeiraLinha = true;

        for (int i = alturaSaida; i < alturaSaida + rows; i++)
        {
            for (int j = linhaSaida; j < linhaSaida + matrixes; j++)
            {
                bool mudarColuna = true;
                for (int k = colunaSaida; k < colunaSaida + columns; k++)
                {
                    reader.ReadBlock(buffer, 0, buffer.Length);
                    //buffer[0] = linhas[1 + j + i*matrixes + i][k];
                    AdicionarObjeto(buffer[0], k - distancia, i, j);
                    if (buffer[0] != '0'){
                        //EstabelecerHUD(buffer[0], k - distancia, i, j, true);
                        if (mudarColuna){
                            novaColunaSaida = k-distancia;
                            mudarColuna = false;
                        }
                        novaLinhaSaida = j;
                        primeiraLinha = false;
                    }else if(primeiraLinha && i == alturaSaida && j == linhaSaida)
                    {
                        distancia++;
                    }
                        

                }
                reader.ReadBlock(buffer, 0, buffer.Length);
            }
            reader.ReadBlock(buffer, 0, buffer.Length);
        }

        alturaSaida = alturaSaida + rows;
        colunaSaida = novaColunaSaida;
        linhaSaida = novaLinhaSaida + 1;
        reader.Close();

        return parte.GetSaida();

    }

    public void BoardSetup(int cor, bool puroInfinito = false)
    {
        this.puroInfinito = puroInfinito;
        morrer = false;
        BoardStart();
        LerPartes();
        fase = cor;
        altura = 0;
        atualSaida = 0;
        alturaSaida = 0;
        colunaSaida = 0;
        linhaSaida = 0;
        fase = 0;
        alturaKevin = 0;
        alturaMaxima = 0;
        ultimaColunaMorta = 0;
        ultimaMatrizMorta = 0;

        bool continuar = false;
        int contagem = 0;
        while(!continuar)
        {
            atualSaida = Random.Range(0, 10);
            continuar = partes[atualSaida].Count > 0;
            contagem++;
            if (contagem == 100)
            {
                continuar = true;
                Debug.Log("Entrou em loop infinito 1");
                Debug.Break();
                Application.Quit();
            }
        }
        atualSaida++;

        AdicionarObjeto('9', 0, 1, 0);


        for (int i = 0; i < 5; i++)
            atualSaida = BuscarParte(atualSaida);

        Bloco bloco;
        Vector3 posicao0 = new Vector3(0, 0, 0);
        if (mapa.TryGetValue(posicao0, out bloco))
        {
            mapa.Remove(new Vector3(0, 0, 0));
            Destroy(bloco.gameObject);
            AdicionarObjeto('4', 0, 0, 0);
        }

        alturaMaxima = 20;
        AtualizarAltura(1);
        //HUDPassarPagina();

        return;

    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChecarGravidade(Vector3 posicao)
    {


        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i != 0 && j != 0)
                    continue;
                Bloco bloco;
                Vector3 destino = posicao + new Vector3(i, 1, j);
                mapa.TryGetValue(destino, out bloco);
                if (bloco != null)
                {
                    bloco.ChecarQueda();
                    ChecarGravidade(posicao + new Vector3(0, -1, 0));
                }
            }
        }
    }

    public Vector2 PosReal(Vector3 posMapa)
    {

        return new Vector2(-posMapa.x * tamanho_x_bloco + posMapa.z * tamanho_x_bloco, posMapa.y * tamanho_y_bloco + posMapa.x * tamanho_y_bloco / 2 + posMapa.z * tamanho_y_bloco / 2);
    }

    public void AdicionarObjeto(char tipo, int x, int y, int z)
    {
        Bloco toInstantiate;
        if (tipo == '9')
        {
            Vector3 posicaoInicial = new Vector3(x, y, z);
            kevin = Instantiate<Player>(kevinScript, new Vector3(-posicaoInicial.x * tamanho_x_bloco + posicaoInicial.z * tamanho_x_bloco, posicaoInicial.y * tamanho_y_bloco + posicaoInicial.x * tamanho_y_bloco / 2 + posicaoInicial.z * tamanho_y_bloco / 2, 0f), Quaternion.identity);
            kevin.Iniciar(true);
            kevin.DefinirRetorno(posicaoInicial);
            kevin.animator.SetTrigger("nascer");
            return;

        }
        else if (tipo == '6')
        {
            toInstantiate = blocos[1];

        }
        else if (tipo == '2')
        {
            toInstantiate = blocos[2];

        }
        else if (tipo == ':')
        {
            toInstantiate = blocos[3];

        }
        else if (tipo == ';')
        {
            toInstantiate = blocos[4];

        }
        else if (tipo == '3')
        {
            toInstantiate = blocos[5];

        }
        else if (tipo == '5')
        {
            toInstantiate = blocos[6];

        }
        else if (tipo == '=')
        {
            //Cobra
            toInstantiate = blocos[7];

        }
        else if (tipo == '>')
        {
            toInstantiate = blocos[8];

        }
        else if (tipo == '4')
        {
            //BlocoInfo
            toInstantiate = blocos[9];

        }
        else if (tipo == 'D')
        {
            //Desativado
            toInstantiate = blocos[0];

        }
        else if (tipo != '0')
        {
            //Padrao
            toInstantiate = blocos[0];
        }
        else
        {
            return;
        }

        Bloco instance = Instantiate(toInstantiate, new Vector3(-x * tamanho_x_bloco + z * tamanho_x_bloco, y * tamanho_y_bloco + x * tamanho_y_bloco / 2 + z * tamanho_y_bloco / 2, 0f), Quaternion.identity) as Bloco;

        Vector3 local = new Vector3(x, y, z);

        instance.iniciar(-x - z * columns + num_base + y * columns * matrixes, local, fase, true);

        if (tipo == '4')
        {
            ((BlocoInfo)instance).ReceberInfo("Touch with a single finger to move and use two fingers to enter and move blocks.");
        }

        if (tipo == 'D')
        {
            instance.Desativar();
        }

        instance.transform.SetParent(boardHolder);

        mapa.Add(local, instance);
    }

    public void Finalizar()
    {
        GameManager.instance.ChecarScores();
        if (boardHolder == null)
            return;
        foreach (Transform child in boardHolder)
        {
            Destroy(child.gameObject);
        }
        if (kevin != null)
        {
            kevin.Remover();
            Destroy(kevin.gameObject);
        }
        Destroy(boardHolder.gameObject);
        mapa.Clear();
        mapaDesativado.Clear();
        Debug.Log("Comecando a apagar");
    }


    public void Parar()
    {
        foreach (MovingObject child in mapa.Values)
        {
            child.Parar();

        }
    }

    public void Resumir()
    {
        foreach (MovingObject child in mapa.Values)
        {
            child.Resumir();
        }
    }

    public bool MoverBloco(Vector3 origem, Vector3 destino)
    {
        Bloco bloco;

        if (mapa.TryGetValue(origem, out bloco))
        {
            mapa.Remove(origem);
            if (mapa.ContainsKey(destino))
            {
                Bloco bloco2;
                mapa.TryGetValue(destino, out bloco2);
                if (bloco.dentro && bloco2.GetTipo() == '5')
                    bloco2.avisar_bloco_dentro();
                else if (bloco2.GetTipo() == '5')
                    bloco2.Desativar();
            }
            mapa.Add(destino, bloco);

            bloco.Move(destino);
            return true;

        }
        return false;

    }


    public void MatarAltura()
    {
        SoundManager.instance.PlaySingle(matando_andar, (int)kevin.posicaoAtual.y - altura);
        for (int i = ultimaColunaMorta - 10; i < ultimaColunaMorta + 10; i++)
        {
            for (int j = ultimaMatrizMorta - 10; j < ultimaMatrizMorta + 10; j++)
            {
                Bloco bloco;
                Vector3 posicao = new Vector3(i, altura, j);
                if (mapa.TryGetValue(posicao, out bloco))
                {
                    bloco.Desativar();
                    if (puroInfinito)
                        MenuInfinito.instance.AtualizarHUD('D', i, altura, j);
                    else
                        Menu.instance.AtualizarHUD('D', i, altura, j);
                    mapaDesativado.Add(posicao, bloco);
                    mapa.Remove(posicao);
                    ultimaColunaMorta = i;
                    ultimaMatrizMorta = j;
                }
            }
        }
        if (altura > 5)
        {
            EliminarBlocos();
        }
        altura++;

        if (kevin.posicaoAtual.y == altura || kevin.destino.y == altura)
        {
            GameManager.instance.KevinMorrer();
        }
    }

    public void EliminarBlocos()
    {
        List<Vector3> aEliminar = new List<Vector3>();

        foreach ( Vector3 posicao in mapaDesativado.Keys){
            if (posicao.y <= altura-5)
            {
                aEliminar.Add(posicao);

            }
        }

        foreach(Vector3 posicao in aEliminar)
        {
            Bloco bloco;
            mapaDesativado.TryGetValue(posicao, out bloco);
            mapaDesativado.Remove(posicao);
            Destroy(bloco.gameObject);
        }
    }

    public void AtualizarHUD(Vector3 origem, Vector3 destino, bool fromKevin)
    {
        if (fromKevin)
        {
            if (puroInfinito)
            {
                char kevinHUD = kevin.dentro ? 'K' : '9';
                MenuInfinito.instance.AtualizarHUD(kevinHUD, (int)kevin.posicaoAtual.x % 20, (int)kevin.posicaoAtual.y % 50, (int)kevin.posicaoAtual.z % 20);
            }
            else
            {
                char kevinHUD = kevin.dentro ? 'K' : '9';
                Menu.instance.AtualizarHUD(kevinHUD, (int)kevin.posicaoAtual.x % 20, (int)kevin.posicaoAtual.y % 50, (int)kevin.posicaoAtual.z % 20);
            }
            return;
        }

        for (int i = 0; i < matrixes; i++)
        {
            Bloco bloco;
            if (mapa.TryGetValue(new Vector3(origem.x, origem.y, i), out bloco))
            {
                if (puroInfinito)
                {
                    MenuInfinito.instance.AtualizarHUD(bloco.GetTipo(), (int)origem.x % 20, (int)origem.y % 50, i);
                    break;
                }
                else
                {
                    Menu.instance.AtualizarHUD(bloco.GetTipo(), (int)origem.x % 20, (int)origem.y % 50, i);
                    break;
                }
            }
            if (i == matrixes - 1)
            {
                if (puroInfinito)
                {
                    MenuInfinito.instance.AtualizarHUD('N', (int)origem.x % 20, (int)origem.y % 50, i);
                    break;
                }
                else
                {
                    Menu.instance.AtualizarHUD('N', (int)origem.x % 20, (int)origem.y % 50, i);
                    break;
                }
            }
        }

        for (int i = 0; i < matrixes; i++)
        {
            Bloco bloco;
            if (mapa.TryGetValue(new Vector3(destino.x, destino.y, i), out bloco))
            {
                if (puroInfinito)
                {
                    MenuInfinito.instance.AtualizarHUD(bloco.GetTipo(), (int)destino.x % 20, (int)destino.y % 50, i);
                    break;
                }
                else
                {
                    Menu.instance.AtualizarHUD(bloco.GetTipo(), (int)destino.x % 20, (int)destino.y % 50, i);
                    break;
                }
            }
            if (i == matrixes - 1)
            {
                if (puroInfinito)
                {
                    MenuInfinito.instance.AtualizarHUD('N', (int)destino.x % 20, (int)destino.y % 50, i);
                    break;
                }
                else
                {
                    Menu.instance.AtualizarHUD('N', (int)destino.x % 20, (int)destino.y % 50, i);
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
                if (mapa.TryGetValue(new Vector3(i, altura + ALTURA, j), out bloco))
                {
                    bloco.ChecarAltura(false);

                }

                for (int k = -ALTURA + 1; k < ALTURA; k++)
                {
                    if (mapa.TryGetValue(new Vector3(i, altura + k, j), out bloco))
                    {
                        bloco.ChecarAltura(true);
                    }
                }

                if (mapa.TryGetValue(new Vector3(i, altura - ALTURA, j), out bloco))
                {
                    bloco.ChecarAltura(false);
                }
            }
        }
    }

    public void AtivarFase()
    {
        foreach (Bloco bloco in mapa.Values)
        {
            bloco.Reiniciar();
        }
    }

    public void DesativarFase()
    {
        foreach (Bloco bloco in mapa.Values)
        {
            bloco.Desativar();
        }
    }

    public void HUDPassarPagina()
    {
        for (int i = ultimaColunaHUD - 10; i < ultimaColunaHUD + 10; i++)
        {
            for (int k = ultimaAlturaHUD; k < ultimaAlturaHUD + 50; k++)
            {
                for (int j = ultimaMatrixHUD - 10; j < ultimaMatrixHUD + 10; j++)
                {

                    Bloco bloco;
                    Vector3 posicao = new Vector3(i, altura, j);
                    if (mapa.TryGetValue(posicao, out bloco))
                    {
                        if (puroInfinito)
                            MenuInfinito.instance.AtualizarHUD(bloco.GetTipo(), i % 20, k % 50, j % 20);
                        else
                            Menu.instance.AtualizarHUD(bloco.GetTipo(), i % 20, k % 50, j % 20);
                        ultimaColunaHUD = i;
                        ultimaMatrixHUD = j;
                    }
                }
            }
        }

        ultimaAlturaHUD += 50;

    }

    public void AtualizarAltura(int altura)
    {
        if(alturaKevin < altura)
            alturaKevin = altura;

        if(puroInfinito)
            MenuInfinito.instance.t_highScoreAltura.text = "Score: " + alturaKevin;
        else
            Menu.instance.textoPontos.text = "Score: " + alturaKevin;

        if(altura > alturaMaxima){
            alturaMaxima += 20;
            atualSaida = BuscarParte(atualSaida);
            atualSaida = BuscarParte(atualSaida);
            GameManager.instance.AtualizarTempoAltura(50 / (alturaMaxima/10 + 3));
        }

        /*if (altura >= ultimaAlturaHUD + 50)
            HUDPassarPagina();
*/
    }

    public void EstabelecerHUD(char tipo, int i, int j, int k, bool fromKevin)
    {
        if (GameManager.instance.puroInfinito)
            MenuInfinito.instance.AtualizarHUD(tipo, i % 20, j % 50, k % 20, fromKevin);
        else
            Menu.instance.AtualizarHUD(tipo, i % 20, j % 50, k % 20, fromKevin);
    }

}
