using Sistema_Operacional.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sistema_Operacional.Escalonamento
{
    public class EscalonadorFCFS : IEscalonador
    {
        private Queue<Processo> FilaFCFS { get; set; } = new Queue<Processo>();
        
        public int QuantidadeProcessosNaFila => FilaFCFS.Count;

        //Adiciona um processo à fila FCFS baseado na ordem de chegada
        public void AdicionarProcesso(Processo processo)
        {
            if (processo == null)
                throw new ArgumentNullException(nameof(processo));

            processo.Estado = Enums.Estados.Pronto;
            FilaFCFS.Enqueue(processo);
            Console.WriteLine($"Processo '{processo.Nome}' (ID: {processo.Id}) adicionado à fila FCFS.");
        }

        /// Remove e retorna o próximo processo da fila FCFS
        public Processo ObterProximoProcesso()
        {
            if (FilaFCFS.Count == 0)
                return null;

            return FilaFCFS.Dequeue();
        }
        public Processo[] VisualizarFila()
        {
            return FilaFCFS.ToArray();
        }

        // Remove um processo específico da fila (usado quando um processo é finalizado antes de executar)
        public bool RemoverProcessoDaFila(int processoId)
        {
            var processosTemp = new List<Processo>();
            bool processoRemovido = false;

            // Remove todos os processos da fila
            while (FilaFCFS.Count > 0)
            {
                var processo = FilaFCFS.Dequeue();
                if (processo.Id == processoId)
                {
                    processoRemovido = true;
                    Console.WriteLine($"Processo '{processo.Nome}' (ID: {processo.Id}) removido da fila FCFS.");
                }
                else
                {
                    processosTemp.Add(processo);
                }
            }

            // Recoloca os processos restantes na fila mantendo a ordem FCFS
            foreach (var processo in processosTemp)
            {
                FilaFCFS.Enqueue(processo);
            }

            return processoRemovido;
        }

         // Limpa toda a fila de processos
        public void LimparFila()
        {
            FilaFCFS.Clear();
            Console.WriteLine("Fila FCFS limpa.");
        }

        // Exibe informações detalhadas sobre a fila FCFS
        public void ExibirInformacoesFila()
        {
            Console.WriteLine("=== INFORMAÇÕES DA FILA FCFS ===");
            Console.WriteLine($"Processos na fila: {FilaFCFS.Count}");
            
            if (FilaFCFS.Count == 0)
            {
                Console.WriteLine("Fila vazia.\n");
                return;
            }

            var processos = FilaFCFS.ToArray();
            Console.WriteLine("Ordem de execução (FCFS):");
            
            for (int i = 0; i < processos.Length; i++)
            {
                var processo = processos[i];
                Console.WriteLine($"  {i + 1}º: Processo '{processo.Nome}' (ID: {processo.Id}) - Chegada: {processo.TempoChegada:HH:mm:ss.fff}");
            }
            Console.WriteLine();
        }
    }
}
