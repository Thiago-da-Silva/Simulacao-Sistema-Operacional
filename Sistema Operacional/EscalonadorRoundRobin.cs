using Sistema_Operacional.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sistema_Operacional
{
    public class EscalonadorRoundRobin : IEscalonador
    {
        private Queue<Processo> FilaDeProntos { get; set; } = new Queue<Processo>();
        public int Quantum { get; private set; }

        public EscalonadorRoundRobin(int quantum)
        {
            this.Quantum = quantum;
        }

        public int QuantidadeProcessosNaFila => FilaDeProntos.Count;

        public void AdicionarProcesso(Processo processo)
        {
            if (processo == null)
                throw new ArgumentNullException(nameof(processo));

            processo.Estado = Enums.Estados.Pronto;
            FilaDeProntos.Enqueue(processo);
            Console.WriteLine($"Processo '{processo.Nome}' (ID: {processo.Id}) adicionado à fila Round Robin.");
        }

        public Processo ObterProximoProcesso()
        {
            if (FilaDeProntos.Count == 0)
                return null;

            return FilaDeProntos.Dequeue();
        }

        public bool RemoverProcessoDaFila(int processoId)
        {
            var processosTemp = new List<Processo>();
            bool processoRemovido = false;

            while (FilaDeProntos.Count > 0)
            {
                var processo = FilaDeProntos.Dequeue();
                if (processo.Id == processoId)
                {
                    processoRemovido = true;
                    Console.WriteLine($"Processo '{processo.Nome}' (ID: {processo.Id}) removido da fila.");
                }
                else
                {
                    processosTemp.Add(processo);
                }
            }

            foreach (var processo in processosTemp)
            {
                FilaDeProntos.Enqueue(processo);
            }

            return processoRemovido;
        }

        public void ExibirInformacoesFila()
        {
            Console.WriteLine($"=== INFORMAÇÕES DA FILA (ROUND ROBIN - Quantum: {Quantum}ms) ===");
            Console.WriteLine($"Processos na fila: {FilaDeProntos.Count}");

            if (FilaDeProntos.Count == 0)
            {
                Console.WriteLine("Fila vazia.\n");
                return;
            }

            Console.WriteLine("Ordem de execução (Circular):");
            int i = 1;
            foreach (var processo in FilaDeProntos)
            {
                Console.WriteLine($"  {i++}º: Processo '{processo.Nome}' (ID: {processo.Id}) - Executado: {processo.TempoExecutado}/{processo.TempoDeExecucaoTotal}ms");
            }
            Console.WriteLine();
        }
    }
}