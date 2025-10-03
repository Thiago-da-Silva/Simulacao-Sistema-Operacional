using Sistema_Operacional.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sistema_Operacional
{
    public class EscalonadorPrioridades : IEscalonador
    {
        private List<Processo> FilaDeProntos { get; set; } = new List<Processo>();

        public int QuantidadeProcessosNaFila => FilaDeProntos.Count;

        public void AdicionarProcesso(Processo processo)
        {
            if (processo == null)
                throw new ArgumentNullException(nameof(processo));

            processo.Estado = Enums.Estados.Pronto;
            FilaDeProntos.Add(processo);

            FilaDeProntos = FilaDeProntos.OrderBy(p => p.Prioridade).ThenBy(p => p.TempoChegada).ToList();

            Console.WriteLine($"Processo '{processo.Nome}' (ID: {processo.Id}) adicionado à fila de prontos.");
        }

        public Processo ObterProximoProcesso()
        {
            if (FilaDeProntos.Count == 0)
                return null;

            var proximoProcesso = FilaDeProntos[0];
            FilaDeProntos.RemoveAt(0);
            return proximoProcesso;
        }

        public bool RemoverProcessoDaFila(int processoId)
        {
            var processoParaRemover = FilaDeProntos.FirstOrDefault(p => p.Id == processoId);
            if (processoParaRemover != null)
            {
                FilaDeProntos.Remove(processoParaRemover);
                Console.WriteLine($"Processo '{processoParaRemover.Nome}' (ID: {processoParaRemover.Id}) removido da fila.");
                return true;
            }
            return false;
        }

        public void ExibirInformacoesFila()
        {
            Console.WriteLine("=== INFORMAÇÕES DA FILA (POR PRIORIDADE) ===");
            Console.WriteLine($"Processos na fila: {FilaDeProntos.Count}");

            if (FilaDeProntos.Count == 0)
            {
                Console.WriteLine("Fila vazia.\n");
                return;
            }

            Console.WriteLine("Ordem de execução (Maior prioridade primeiro):");

            for (int i = 0; i < FilaDeProntos.Count; i++)
            {
                var processo = FilaDeProntos[i];
                Console.WriteLine($"  {i + 1}º: Processo '{processo.Nome}' (ID: {processo.Id}, Prioridade: {processo.Prioridade})");
            }
            Console.WriteLine();
        }
    }
}