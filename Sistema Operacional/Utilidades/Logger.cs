using System;
using System.IO;

namespace Sistema_Operacional.Utilidades
{
    public static class Logger
    {
        private static string caminhoArquivo = "log_simulacao.txt";
        private static bool habilitado = true;

        public static void Registrar(string mensagem)
        {
            if (!habilitado) return;

            try
            {
                string linha = $"[{DateTime.Now:HH:mm:ss.fff}] {mensagem}";
                File.AppendAllText(caminhoArquivo, linha + "\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao registrar log: {ex.Message}");
            }
        }

        public static void LimparLog()
        {
            try
            {
                File.WriteAllText(caminhoArquivo, $"=== INÍCIO DA SIMULAÇÃO - {DateTime.Now:dd/MM/yyyy HH:mm:ss} ===\n");
                File.AppendAllText(caminhoArquivo, $"Seed: {AleatorioSistema.GetSeedAtual()}\n\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar log: {ex.Message}");
            }
        }

        public static void Habilitar(bool estado)
        {
            habilitado = estado;
        }

        public static void DefinirCaminho(string caminho)
        {
            caminhoArquivo = caminho;
        }

        public static void Ler()
        {
            try
            {
                if (!File.Exists(caminhoArquivo))
                {
                    Console.WriteLine("Arquivo de log não encontrado.");
                    return;
                }

                string[] linhas = File.ReadAllLines(caminhoArquivo);
                
                if (linhas.Length == 0)
                {
                    Console.WriteLine("Log vazio.");
                    return;
                }

                Console.WriteLine($"Total de entradas: {linhas.Length}");
                Console.WriteLine("Últimas 50 entradas:");
                Console.WriteLine(new string('-', 80));
                
                int inicio = Math.Max(0, linhas.Length - 50);
                for (int i = inicio; i < linhas.Length; i++)
                {
                    Console.WriteLine(linhas[i]);
                }
                
                Console.WriteLine(new string('-', 80));
                Console.WriteLine($"Arquivo completo: {caminhoArquivo}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao ler log: {ex.Message}");
            }
        }
    }
}
