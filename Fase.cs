using System;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public struct Fase
{
    public int score;
    public string nome;
    public string nomeArquivo;
    private Button b_botao;
    private int fase;
    private bool padrao;

    public Fase(int score, string nome, string nomeArquivo, int fase, Button botao, bool padrao, bool editor = false){
        this.score = score;
        this.nome = nome;
        this.fase = fase;
        this.nomeArquivo = nomeArquivo;
        this.padrao = padrao;

        b_botao = botao;

        if (!editor)
            b_botao.onClick.AddListener(IniciarFase);
        else
            b_botao.onClick.AddListener(DefinirNome);

        b_botao.GetComponentInChildren<Text>().text = nome + " // Points: " + score.ToString();
    }

    public void AtualizarScore()
    {
        b_botao.GetComponentInChildren<Text>().text = nome + " // Points: " + score.ToString();
    }

    public void ApagarBotao(bool apagar)
    {
        b_botao.gameObject.SetActive(!apagar);
    }

    public void IniciarFase()
    {
        Menu.instance.IniciarFase(fase);

        GameManager.instance.InitGame(fase, nomeArquivo, padrao);
    }

    public void DefinirNome()
    {
        EditorManager.instance.DefinirNomeArquivo(nomeArquivo);
        EditorManager.instance.CarregarTemp();

    }

}
