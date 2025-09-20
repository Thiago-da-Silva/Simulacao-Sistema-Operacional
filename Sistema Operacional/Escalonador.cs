using System;
using System.Collections.Generic;
using System.Linq;

namespace Sistema_Operacional
{
    public class Escalonador
    {
        private Queue<Processo> FilaFCFS { get; set; } = new Queue<Processo>();
        
        public int QuantidadeProcessosNaFila => FilaFCFS.Count;

        /// <summary>
        /// Adiciona um processo � fila FCFS baseado na ordem de chegada
        /// </summary>
        /// <param name="processo">Processo a ser adicionado</param>
        public void AdicionarProcesso(Processo processo)
        {
            if (processo == null)
                throw new ArgumentNullException(nameof(processo));

            processo.Estado = Enums.Estados.Pronto;
            FilaFCFS.Enqueue(processo);
            Console.WriteLine($"Processo '{processo.Nome}' (ID: {processo.Id}) adicionado � fila FCFS.");
        }

        /// <summary>
        /// Remove e retorna o pr�ximo processo da fila FCFS
        /// </summary>
        /// <returns>Pr�ximo processo na fila ou null se a fila estiver vazia</returns>
        public Processo ObterProximoProcesso()
        {
            if (FilaFCFS.Count == 0)
                return null;

            return FilaFCFS.Dequeue();
        }

        /// <summary>
        /// Visualiza a fila atual sem remover elementos
        /// </summary>
        /// <returns>Array dos processos na fila ordenados por FCFS</returns>
        public Processo[] VisualizarFila()
        {
            return FilaFCFS.ToArray();
        }

        /// <summary>
        /// Remove um processo espec�fico da fila (usado quando um processo � finalizado antes de executar)
        /// </summary>
        /// <param name="processoId">ID do processo a ser removido</param>
        /// <returns>True se o processo foi removido, false caso contr�rio</returns>
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

        /// <summary>
        /// Limpa toda a fila de processos
        /// </summary>
        public void LimparFila()
        {
            FilaFCFS.Clear();
            Console.WriteLine("Fila FCFS limpa.");
        }

        /// <summary>
        /// Exibe informa��es detalhadas sobre a fila FCFS
        /// </summary>
        public void ExibirInformacoesFila()
        {
            Console.WriteLine("=== INFORMA��ES DA FILA FCFS ===");
            Console.WriteLine($"Processos na fila: {FilaFCFS.Count}");
            
            if (FilaFCFS.Count == 0)
            {
                Console.WriteLine("Fila vazia.\n");
                return;
            }

            var processos = FilaFCFS.ToArray();
            Console.WriteLine("Ordem de execu��o (FCFS):");
            
            for (int i = 0; i < processos.Length; i++)
            {
                var processo = processos[i];
                Console.WriteLine($"  {i + 1}�: Processo '{processo.Nome}' (ID: {processo.Id}) - Chegada: {processo.TempoChegada:HH:mm:ss.fff}");
            }
            Console.WriteLine();
        }
    }
}