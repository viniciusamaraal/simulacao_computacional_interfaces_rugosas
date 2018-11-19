using System.IO;
using System.Linq;

namespace projeto_simulacao
{
    public static class Util
    {
        public static void GravarInformacoesArquivo(double[] vetor, string nomeArquivo)
        {
            using (var escritorArquivo = new StreamWriter($"..\\..\\_resultados\\{ nomeArquivo }"))
                escritorArquivo.Write(string.Join(" ", vetor));
        }

        public static void PlotarVetor(double[] vetor)
        {
            MLApp.MLApp matlab = new MLApp.MLApp();
            matlab.Visible = 0;
            //matlab.PutWorkspaceData("a", "base", matriz);
            matlab.PutWorkspaceData("a", "base", vetor);
            matlab.Execute("plot(a);");
        }
    }
}
