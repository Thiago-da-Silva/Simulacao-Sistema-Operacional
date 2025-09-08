using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Operacional
{
    public class SistemaOperacional
    {
        private int TotalMemoria { get; set; }
        private int NumeroProcessos { get; set; }
        private bool CpuEmUso { get; set; } = false;
        private DateTime DataInicio { get; set; } = DateTime.Now;
        private DateTime? DataFinal { get; set; } = null;
        private int ProcessoEmExecucaoId { get; set; } = 0;

        private List<Processo> Processos = new List<Processo>();

        public SistemaOperacional(int totalMemoria)
        {
            TotalMemoria = totalMemoria;
            NumeroProcessos = 0;
            CpuEmUso = false;
        }

        public void CriarProcesso(string nome)
        {
            NumeroProcessos++;
            this.Processos.Add(new Processo(nome, this.NumeroProcessos, 1)); // Prioridade temporária como 1 (até fazer o escalonamento)
            Console.WriteLine($"Processo '{nome}' criado com ID {this.NumeroProcessos}.");
        }

        public void ListarProcessos()
        {
            foreach (var processo in Processos)
            {
                Console.WriteLine($"Processo ID: {processo.Id}\nNome: {processo.Nome}\nPrioridade: {processo.Prioridade}\nEstado: {processo.Estado}\nThreads: {processo.Threads.Count}\n");
            }
        }

        public void FinalizarProcesso(int id)
        {
            try
            {
                Processo processo = this.Processos.FirstOrDefault(p => p.Id == id);
                if (processo == null)
                {
                    Console.WriteLine($"Processo com ID {id} não encontrado.");
                    return;
                }
                processo.Estado = Enums.Estados.Finalizado;
                this.Processos.Remove(processo);
                this.NumeroProcessos--;
                this.CpuEmUso = false;
                this.ProcessoEmExecucaoId = 0;
                Console.WriteLine($"Processo com ID {id} finalizado.");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao finalizar o processo: {ex.Message}");
            }
        }

        public void ExecutarProcesso(int id)
        // Falta fazer o escalonamento pra pegar qual processo vai ser executado ao inves de digitar o ID, depois executar dnv pra ir pro proximo processo
        {
            try
            {
                if(this.CpuEmUso == true)
                {
                    Console.WriteLine("CPU está em uso. Aguarde a finalização do processo atual.");
                    return;
                }
                Processo processo = this.Processos.FirstOrDefault(p => p.Id == id);
                if (processo == null)
                {
                    Console.WriteLine($"Processo com ID {id} não encontrado.");
                    return;
                }
                processo.Estado = Enums.Estados.Executando;
                this.ProcessoEmExecucaoId = id;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao executar o processo: {ex.Message}");
            }
        }

        public void PausarProcesso(int id)
        {
            try
            {
                Processo processo = this.Processos.FirstOrDefault(p => p.Id == id);
                if (processo == null)
                {
                    Console.WriteLine($"Processo com ID {id} não encontrado.");
                    return;
                }
                processo.Estado = Enums.Estados.Bloqueado;
                this.CpuEmUso = false;
                this.ProcessoEmExecucaoId = 0;
                Console.WriteLine($"Processo com ID {id} pausado.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao pausar o processo: {ex.Message}");
            }
        }
    }
}
