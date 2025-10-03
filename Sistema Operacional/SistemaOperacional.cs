using Sistema_Operacional.Interface;
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
        private int NumeroProcessos { get; set; } = 0;
        private bool CpuEmUso { get; set; } = false;
        private DateTime DataInicio { get; set; } = DateTime.Now;
        private DateTime? DataFinal { get; set; } = null;
        private int ProcessoEmExecucaoId { get; set; } = 0;
        private IEscalonador Escalonador { get; set; }

        private List<Processo> Processos = new List<Processo>();

        public SistemaOperacional(int totalMemoria, IEscalonador escalonadorInicial)
        {
            TotalMemoria = totalMemoria;
            NumeroProcessos = 0;
            CpuEmUso = false;
            Escalonador = escalonadorInicial;
        }

        public void CriarProcesso(string nome, int prioridade = 5)
        {
            int novoId = (Processos.Any() ? Processos.Max(p => p.Id) : 0) + 1;
            var novoProcesso = new Processo(nome, novoId, prioridade);
            this.Processos.Add(novoProcesso);

            Escalonador.AdicionarProcesso(novoProcesso);

            Console.WriteLine($"Processo '{nome}' (Prioridade: {prioridade}) criado com ID {novoId} em {novoProcesso.TempoChegada:HH:mm:ss.fff}");
        }

        public float CalcularMemoriaUsada()
        {
            return Processos.Sum(p => p.MemoriaUtilizada);
        }

        public float CalcularMemoriaDisponivel()
        {
            return TotalMemoria - CalcularMemoriaUsada();
        }

        public bool VerificarMemoriaDisponivel(float memoriaRequerida)
        {
            return CalcularMemoriaDisponivel() >= memoriaRequerida;
        }

        public void MostrarStatusMemoria()
        {
            float memoriaUsada = CalcularMemoriaUsada();
            float memoriaDisponivel = CalcularMemoriaDisponivel();
            float percentualUso = (memoriaUsada / TotalMemoria) * 100;

            Console.WriteLine("=== STATUS DA MEMÓRIA ===");
            Console.WriteLine($"Memória Total: {TotalMemoria}MB");
            Console.WriteLine($"Memória Usada: {memoriaUsada:F2}MB ({percentualUso:F1}%)");
            Console.WriteLine($"Memória Disponível: {memoriaDisponivel:F2}MB");
            Console.WriteLine();
        }

        public void ListarProcessos()
        {
            Console.WriteLine("=== LISTA DE PROCESSOS ===");
            if (Processos.Count == 0)
            {
                Console.WriteLine("Nenhum processo encontrado.");
                return;
            }

            foreach (var processo in Processos.OrderBy(p => p.TempoChegada))
            {
                Console.WriteLine($"Processo ID: {processo.Id}");
                Console.WriteLine($"Nome: {processo.Nome}");
                Console.WriteLine($"Prioridade: {processo.Prioridade}");
                Console.WriteLine($"Estado: {processo.Estado}");
                Console.WriteLine($"Threads: {processo.Threads.Count}");
                Console.WriteLine($"Memória Utilizada: {processo.MemoriaUtilizada:F2}MB");
                Console.WriteLine($"Tempo de Chegada: {processo.TempoChegada:HH:mm:ss.fff}");
                Console.WriteLine();
            }
            
            MostrarStatusMemoria();
        }

        public void ListarFilaProcessos()
        {
            Escalonador.ExibirInformacoesFila();
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

                var processo = Escalonador.ObterProximoProcesso();
                if (processo == null)
                {
                    Console.WriteLine("Não há processos na fila para executar.");
                    return;
                }

                processo.Estado = Enums.Estados.Executando;
                CpuEmUso = true;
                ProcessoEmExecucaoId = processo.Id;

                Console.WriteLine($"Executando processo '{processo.Nome}' (ID: {processo.Id})");
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

                float memoriaLiberada = processo.MemoriaUtilizada;

                processo.Estado = Enums.Estados.Finalizado;
                this.Processos.Remove(processo);

                Escalonador.RemoverProcessoDaFila(id);

                if (ProcessoEmExecucaoId == id)
                {
                    this.CpuEmUso = false;
                    this.ProcessoEmExecucaoId = 0;
                    Console.WriteLine($"Processo com ID {id} finalizado. CPU liberada. Memória liberada: {memoriaLiberada:F2}MB");

                    if (Escalonador.QuantidadeProcessosNaFila > 0)
                    {
                        Console.WriteLine("Executando próximo processo da fila...");
                        ExecutarProximoProcesso();
                    }
                }
                else
                {
                    Console.WriteLine($"Processo com ID {id} finalizado. Memória liberada: {memoriaLiberada:F2}MB");
                }

                MostrarStatusMemoria();
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

                    if (Escalonador.QuantidadeProcessosNaFila > 0)
                    {
                        Console.WriteLine("Executando próximo processo da fila...");
                        ExecutarProximoProcesso();
                    }
                }
                else
                {
                    processo.Estado = Enums.Estados.Bloqueado;
                    Escalonador.RemoverProcessoDaFila(id);
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

                Escalonador.AdicionarProcesso(processo);
                Console.WriteLine($"Processo com ID {id} adicionado novamente à fila de prontos.");
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
            Console.WriteLine($"Processos na fila: {Escalonador.QuantidadeProcessosNaFila}");
            Console.WriteLine();
        }

        public bool AdicionarThreadAoProcesso(int processoId, float memoriaThread)
        {
            try
            {
                Processo processo = this.Processos.FirstOrDefault(p => p.Id == processoId);
                if (processo == null)
                {
                    Console.WriteLine($"Processo com ID {processoId} não encontrado.");
                    return false;
                }

                // Verifica se há memória suficiente
                if (!VerificarMemoriaDisponivel(memoriaThread))
                {
                    float memoriaDisponivel = CalcularMemoriaDisponivel();
                    Console.WriteLine($"ERRO: Memória insuficiente!");
                    Console.WriteLine($"Memória solicitada: {memoriaThread}MB");
                    Console.WriteLine($"Memória disponível: {memoriaDisponivel:F2}MB");
                    Console.WriteLine($"Memória total do sistema: {TotalMemoria}MB");
                    MostrarStatusMemoria();
                    return false;
                }

                bool sucesso = processo.AdicionarThread(memoriaThread);
                if (sucesso)
                {
                    Console.WriteLine($"Thread criada com sucesso!");
                    MostrarStatusMemoria();
                }
                return sucesso;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao adicionar thread ao processo: {ex.Message}");
                return false;
            }
        }

        public void AdicionarThreadAoProcesso(int processoId)
        {
            // Método mantido para compatibilidade - usa valor padrão
            AdicionarThreadAoProcesso(processoId, 1.0f);
        }

        public void ListarThreadsDoProcesso(int processoId)
        {
            try
            {
                Processo processo = this.Processos.FirstOrDefault(p => p.Id == processoId);
                if (processo == null)
                {
                    Console.WriteLine($"Processo com ID {processoId} não encontrado.");
                    return;
                }
                
                if (processo.Threads.Count == 0)
                {
                    Console.WriteLine($"Processo '{processo.Nome}' (ID: {processo.Id}) não possui threads.");
                    return;
                }
                
                Console.WriteLine($"=== THREADS DO PROCESSO '{processo.Nome}' (ID: {processo.Id}) ===");
                processo.ListarThreads();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar threads do processo: {ex.Message}");
            }
        }

        public void FinalizarThreadDoProcesso(int processoId, int threadId)
        {
            try
            {
                Processo processo = this.Processos.FirstOrDefault(p => p.Id == processoId);
                if (processo == null)
                {
                    Console.WriteLine($"Processo com ID {processoId} não encontrado.");
                    return;
                }
                processo.FinalizarThread(threadId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao finalizar thread do processo: {ex.Message}");
            }
        }

        public void PausarThreadDoProcesso(int processoId, int threadId)
        {
            try
            {
                Processo processo = this.Processos.FirstOrDefault(p => p.Id == processoId);
                if (processo == null)
                {
                    Console.WriteLine($"Processo com ID {processoId} não encontrado.");
                    return;
                }
                
                var thread = processo.Threads.FirstOrDefault(t => t.Id == threadId);
                if (thread == null)
                {
                    Console.WriteLine($"Thread com ID {threadId} não encontrada no processo {processo.Nome} (ID: {processo.Id}).");
                    return;
                }
                
                thread.PausarThread();
                Console.WriteLine($"Thread com ID {threadId} pausada no processo '{processo.Nome}' (ID: {processo.Id}).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao pausar thread do processo: {ex.Message}");
            }
        }

        public void RetomarThreadDoProcesso(int processoId, int threadId)
        {
            try
            {
                Processo processo = this.Processos.FirstOrDefault(p => p.Id == processoId);
                if (processo == null)
                {
                    Console.WriteLine($"Processo com ID {processoId} não encontrado.");
                    return;
                }
                
                var thread = processo.Threads.FirstOrDefault(t => t.Id == threadId);
                if (thread == null)
                {
                    Console.WriteLine($"Thread com ID {threadId} não encontrada no processo {processo.Nome} (ID: {processo.Id}).");
                    return;
                }
                
                thread.RetomarThread();
                Console.WriteLine($"Thread com ID {threadId} retomada no processo '{processo.Nome}' (ID: {processo.Id}).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao retomar thread do processo: {ex.Message}");
            }
        }

        public Processo ObterProcessoPorId(int id)
        {
            return this.Processos.FirstOrDefault(p => p.Id == id);
        }

        public int GetProcessoEmExecucaoId()
        {
            return ProcessoEmExecucaoId;
        }

        public bool IsCpuEmUso()
        {
            return CpuEmUso;
        }

        public int GetTotalMemoria()
        {
            return TotalMemoria;
        }

        public int GetNumeroProcessos()
        {
            return NumeroProcessos;
        }
    }
}
