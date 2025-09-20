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
        private Escalonador EscalonadorFCFS { get; set; } = new Escalonador();

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
            var novoProcesso = new Processo(nome, this.NumeroProcessos, 1);
            this.Processos.Add(novoProcesso);
            
            // Adiciona ao escalonador FCFS
            EscalonadorFCFS.AdicionarProcesso(novoProcesso);
            
            Console.WriteLine($"Processo '{nome}' criado com ID {this.NumeroProcessos} em {novoProcesso.TempoChegada:HH:mm:ss.fff}");
        }

        public void ListarProcessos()
        {
            Console.WriteLine("=== LISTA DE PROCESSOS ===");
            foreach (var processo in Processos.OrderBy(p => p.TempoChegada))
            {
                Console.WriteLine($"Processo ID: {processo.Id}\nNome: {processo.Nome}\nPrioridade: {processo.Prioridade}\nEstado: {processo.Estado}\nThreads: {processo.Threads.Count}\nTempo de Chegada: {processo.TempoChegada:HH:mm:ss.fff}\n");
            }
        }

        public void ListarFilaProcessos()
        {
            EscalonadorFCFS.ExibirInformacoesFila();
        }

        public void ExecutarProximoProcesso()
        {
            try
            {
                if (CpuEmUso)
                {
                    Console.WriteLine("CPU está em uso. Finalize o processo atual primeiro.");
                    return;
                }

                var processo = EscalonadorFCFS.ObterProximoProcesso();
                if (processo == null)
                {
                    Console.WriteLine("Não há processos na fila para executar.");
                    return;
                }

                processo.Estado = Enums.Estados.Executando;
                CpuEmUso = true;
                ProcessoEmExecucaoId = processo.Id;
                
                Console.WriteLine($"Executando processo '{processo.Nome}' (ID: {processo.Id}) - Chegada: {processo.TempoChegada:HH:mm:ss.fff}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao executar o próximo processo: {ex.Message}");
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
                
                // Remove da fila se ainda estiver lá
                EscalonadorFCFS.RemoverProcessoDaFila(id);
                
                // Se este processo estava executando, libera a CPU
                if (ProcessoEmExecucaoId == id)
                {
                    this.CpuEmUso = false;
                    this.ProcessoEmExecucaoId = 0;
                    Console.WriteLine($"Processo com ID {id} finalizado. CPU liberada.");
                    
                    // Automaticamente executa o próximo processo na fila
                    if (EscalonadorFCFS.QuantidadeProcessosNaFila > 0)
                    {
                        Console.WriteLine("Executando próximo processo da fila...");
                        ExecutarProximoProcesso();
                    }
                }
                else
                {
                    Console.WriteLine($"Processo com ID {id} finalizado.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao finalizar o processo: {ex.Message}");
            }
        }

        //public void ExecutarProcesso(int id)
        //// Método mantido para compatibilidade, mas recomenda-se usar ExecutarProximoProcesso()
        //{
        //    try
        //    {
        //        if(this.CpuEmUso == true)
        //        {
        //            Console.WriteLine("CPU está em uso. Aguarde a finalização do processo atual.");
        //            return;
        //        }
        //        Processo processo = this.Processos.FirstOrDefault(p => p.Id == id);
        //        if (processo == null)
        //        {
        //            Console.WriteLine($"Processo com ID {id} não encontrado.");
        //            return;
        //        }
        //        processo.Estado = Enums.Estados.Executando;
        //        this.ProcessoEmExecucaoId = id;
        //        this.CpuEmUso = true;
        //        Console.WriteLine($"Processo com ID {id} executando.");

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Erro ao executar o processo: {ex.Message}");
        //    }
        //}

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
                
                if (ProcessoEmExecucaoId == id)
                {
                    processo.Estado = Enums.Estados.Bloqueado;
                    this.CpuEmUso = false;
                    this.ProcessoEmExecucaoId = 0;
                    Console.WriteLine($"Processo com ID {id} pausado. CPU liberada.");
                    
                    // Automaticamente executa o próximo processo na fila
                    if (EscalonadorFCFS.QuantidadeProcessosNaFila > 0)
                    {
                        Console.WriteLine("Executando próximo processo da fila...");
                        ExecutarProximoProcesso();
                    }
                }
                else
                {
                    processo.Estado = Enums.Estados.Bloqueado;
                    // Remove da fila se estiver lá
                    EscalonadorFCFS.RemoverProcessoDaFila(id);
                    Console.WriteLine($"Processo com ID {id} pausado.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao pausar o processo: {ex.Message}");
            }
        }

        public void RetomarProcesso(int id)
        {
            try
            {
                Processo processo = this.Processos.FirstOrDefault(p => p.Id == id);
                if (processo == null)
                {
                    Console.WriteLine($"Processo com ID {id} não encontrado.");
                    return;
                }

                if (processo.Estado != Enums.Estados.Bloqueado)
                {
                    Console.WriteLine($"Processo com ID {id} não está pausado.");
                    return;
                }

                // Adiciona novamente ao escalonador
                EscalonadorFCFS.AdicionarProcesso(processo);
                Console.WriteLine($"Processo com ID {id} adicionado novamente à fila de prontos (final da fila FCFS).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao retomar o processo: {ex.Message}");
            }
        }

        public void MostrarStatusCPU()
        {
            Console.WriteLine("=== STATUS DA CPU ===");
            if (CpuEmUso)
            {
                var processoAtual = Processos.FirstOrDefault(p => p.Id == ProcessoEmExecucaoId);
                if (processoAtual != null)
                {
                    Console.WriteLine($"CPU EM USO: Processo '{processoAtual.Nome}' (ID: {processoAtual.Id})");
                }
            }
            else
            {
                Console.WriteLine("CPU LIVRE");
            }
            Console.WriteLine($"Processos na fila FCFS: {EscalonadorFCFS.QuantidadeProcessosNaFila}");
            Console.WriteLine();
        }
    }
}
