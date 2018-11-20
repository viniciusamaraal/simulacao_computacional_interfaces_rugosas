using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projeto_simulacao
{
    public enum Tipo
    {
        DA,
        DARS
    }

    class Program
    {
        static Tipo TipoSimulacao = Tipo.DA;
        static int tempo = (int)Math.Pow(10, 6);
        static int qtdAmostras = 100;
        static int[] listaExperimentos = new int[] { 800, 1600 }; // 200, 400, 800, 1600

        static int[] vetSubstrato = null;
        static double[] vetAlturaMedia = null;
        static double[] vetRugosidadeMedia = null;

        static List<double[]> listaAlturasMediasAmostras = null;
        static List<double[]> listaRugosidadesMediasAmostras = null;

        static void Main(string[] args)
        {
            Random r = new Random();

            foreach (int tamanhoSubstrato in listaExperimentos)
            {
                int numParticulas = tamanhoSubstrato; // definido no enunciado
                listaAlturasMediasAmostras = new List<double[]>(qtdAmostras);
                listaRugosidadesMediasAmostras = new List<double[]>(qtdAmostras);

                var cronometro = new Stopwatch();
                cronometro.Start();

                for (int i = 0; i < qtdAmostras; i++)
                {
                    vetSubstrato = new int[tamanhoSubstrato];
                    vetAlturaMedia = new double[tempo];
                    vetRugosidadeMedia = new double[tempo];

                    Console.WriteLine($"Tamanho: { tamanhoSubstrato } - Amostra: { i + 1 } - Início em: { DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") }");
                    for (int j = 0; j < tempo; j++)
                    {
                        for (int k = 0; k < numParticulas; k++)
                        {
                            //vetSubstrato[r.Next(0, tamanhoSubstrato)]++; // Deposição Aleatória
                            DepositarParticulaRelaxacao(r.Next(0, tamanhoSubstrato), ref vetSubstrato); // Deposição Aleatória com Relaxação Superficia
                        }

                        vetAlturaMedia[j] = (numParticulas * (j + 1)) / tamanhoSubstrato;
                        vetRugosidadeMedia[j] = CalcularRugosidadeMediaUnidadeTempo(vetSubstrato, vetAlturaMedia[j]);
                    }

                    listaAlturasMediasAmostras.Add(vetAlturaMedia);
                    listaRugosidadesMediasAmostras.Add(vetRugosidadeMedia);
                }

                cronometro.Stop();
                var tempoMedio = cronometro.Elapsed.TotalSeconds / qtdAmostras;

                var vetAlturaMediaAmostras = CalcularMediaAmostras(listaAlturasMediasAmostras, tempo);
                var vetRugosidadeMediaAmostras = CalcularMediaAmostras(listaRugosidadesMediasAmostras, tempo);

                Util.GravarInformacoesArquivo(vetAlturaMedia, $"{ TipoSimulacao.ToString() }\\{ DateTime.Now.ToString("ddMMyyyy_HHmmss") }-ALTURA_MEDIA-TAM_{ tamanhoSubstrato }-TEMPO_MEDIO_{ tempoMedio }s.txt");
                Util.GravarInformacoesArquivo(vetRugosidadeMediaAmostras, $"{ TipoSimulacao.ToString() }\\{ DateTime.Now.ToString("ddMMyyyy_HHmmss") }-RUGOSIDADE_MEDIA-TAM{ tamanhoSubstrato }-TEMPO_MEDIO_{ tempoMedio }s.txt");
            }
        }

        static void DepositarParticulaRelaxacao(int posicaoSorteada, ref int[] vetSubstrato)
        {
            int posAntes = posicaoSorteada - 1, posDepois = posicaoSorteada + 1;
            bool considerarAntes = false, considerarDepois = false;

            if (posicaoSorteada == 0)
                posAntes = vetSubstrato.Length - 1;
            else if (posicaoSorteada == vetSubstrato.Length - 1)
                posDepois = 0;

            if (vetSubstrato[posAntes] < vetSubstrato[posicaoSorteada])
                considerarAntes = true;
            if (vetSubstrato[posDepois] < vetSubstrato[posicaoSorteada])
                considerarDepois = true;

            if (considerarAntes && considerarDepois)
            {
                Random r = new Random();
                int posicao = r.Next(0, 2);
                if (posicao == 0)
                    vetSubstrato[posAntes]++;
                else
                    vetSubstrato[posDepois]++;
            }
            else if (considerarAntes)
                vetSubstrato[posAntes]++;
            else if (considerarDepois)
                vetSubstrato[posDepois]++;
            else
                vetSubstrato[posicaoSorteada]++;
        }

        static double[] CalcularMediaAmostras(List<double[]> lista, int unidadesTempo)
        {
            double[] mediaTempo = new double[unidadesTempo];
            for (int i = 0; i < unidadesTempo; i++)
            {
                double soma = 0;
                for (int j = 0; j < lista.Count; j++)
                    soma += lista[j][i];

                mediaTempo[i] = soma / lista.Count;
            }

            return mediaTempo;
        }
        
        static double CalcularRugosidadeMediaUnidadeTempo(int[] substrato, double alturaMedia)
        {
            double soma = 0;
            for (int i = 0; i < substrato.Length; i++)
                soma = soma + Math.Pow((substrato[i] - alturaMedia), 2);

            return Math.Sqrt(soma / substrato.Length);
        }
    }
}