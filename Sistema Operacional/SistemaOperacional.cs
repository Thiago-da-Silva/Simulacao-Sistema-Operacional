using Sistema_Operacional.Escalonamento;
using Sistema_Operacional.Modelos;
using Sistema_Operacional.Memoria;
using Sistema_Operacional.Enums;
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
        private int NumeroProcessos { get; set; } = 0;
        private bool CpuEmUso { get; set; } = false;
        private DateTime DataInicio { get; set; } = DateTime.Now;
        private DateTime? DataFinal { get; set; } = null;
        private int ProcessoEmExecucaoId { get; set; } = 0;
        private IEscalonador Escalonador { get; set; }
        private int TempoSobrecargaTrocaContexto { get; set; }
        public int NumeroTrocasContexto { get; private set; } = 0;

        private List<Processo> Processos = new List<Processo>();
        private GerenciadorMemoria GerenciadorMemoria { get; set; }
        private int TamanhoPagina { get; set; }

        private List<Processo> ProcessosFinalizados = new List<Processo>();
        private double TempoTotalCPUOcupadaMs { get; set; } = 0;

        public SistemaOperacional(int totalMemoria, IEscalonador escalonadorInicial, int tempoSobrecarga, int tamanhoPagina)
        {
            NumeroProcessos = 0;
            CpuEmUso = false;
            Escalonador = escalonadorInicial;
            TempoSobrecargaTrocaContexto = tempoSobrecarga;

            this.TamanhoPagina = tamanhoPagina;
            this.GerenciadorMemoria = new GerenciadorMemoria(totalMemoria, tamanhoPagina);
        }


        public void CriarProcesso(string nome, int priority = 5, float memoriaInicial = 10f)
        {
            // Calcular páginas necessárias
            int paginasNecessarias = (int)Math.Ceiling(memoriaInicial / (float)this.TamanhoPagina);
            if (paginasNecessarias == 0) paginasNecessarias = 1; // Aloca pelo menos 1 página

            // Tentar alocar memória
            int novoId = (Processos.Any() ? Processos.Max(p => p.Id) : 0) + 1;
            List<int> framesAlocados = GerenciadorMemoria.AlocarPaginas(novoId, paginasNecessarias);

            if (framesAlocados == null)
            {
                Console.WriteLine($"ERRO: Memória insuficiente para criar processo '{nome}'. {paginasNecessarias} páginas solicitadas.");
                GerenciadorMemoria.MostrarStatusMemoria();
                return;
            }

            // Criar processo e registrar alocação
            var novoProcesso = new Processo(nome, novoId, priority);

            List<int> paginasLogicas = novoProcesso.TabelaDePaginas.RegistrarAlocacao(framesAlocados);

            // Passamos as páginas lógicas para a thread
            bool sucesso = novoProcesso.AdicionarThread(memoriaInicial, paginasLogicas);

            if (sucesso)
            {
                this.Processos.Add(novoProcesso);
                Escalonador.AdicionarProcesso(novoProcesso);

                Console.WriteLine($"Processo '{nome}' (ID {novoId}) criado. {paginasNecessarias} páginas ({memoriaInicial}MB) alocadas.");
                GerenciadorMemoria.MostrarStatusMemoria();
            }
            else
            {
                Console.WriteLine($"ERRO CRÍTICO: Falha ao criar thread inicial do processo '{nome}'. Revertendo...");
                novoProcesso.TabelaDePaginas.LiberarPaginasEspecificas(paginasLogicas);
                GerenciadorMemoria.LiberarPaginas(framesAlocados);
            }
        }

        public float CalcularMemoriaUsada()
        {
            return GerenciadorMemoria.CalcularMemoriaUsada();
        }

        public float CalcularMemoriaDisponivel()
        {
            return GerenciadorMemoria.CalcularMemoriaDisponivel();
        }
        public bool VerificarMemoriaDisponivel(float memoriaRequerida)
        {
            return GerenciadorMemoria.CalcularMemoriaDisponivel() >= memoriaRequerida;
        }

        public void MostrarStatusMemoria()
        {
            GerenciadorMemoria.MostrarStatusMemoria();
        }

        public void ListarProcessos()
        {
            Console.WriteLine("=== LISTA DE PROCESSOS ===");
            if (Processos.Count == 0)
            {
                Console.WriteLine("Nenhum processo encontrado.");
                return;
            }

            foreach (var processo in Processos.OrderBy(p => p.Id))
            {
                Console.WriteLine($"ID: {processo.Id} | Nome: {processo.Nome} | Estado: {processo.Estado}");
                Console.WriteLine($"  Memória: {processo.TabelaDePaginas.TotalPaginas() * TamanhoPagina:F2}MB ({processo.TabelaDePaginas.TotalPaginas()} páginas)");
                Console.WriteLine($"  Tempo Executado: {processo.TempoExecutado} / {processo.TempoDeExecucaoTotal}ms");

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
            if (CpuEmUso)
            {
                Console.WriteLine("CPU já está executando um slice. Aguarde a finalização.");
                return;
            }

            var processo = Escalonador.ObterProximoProcesso();
            if (processo == null)
            {
                Console.WriteLine("Não há processos na fila para executar.");
                return;
            }

            if (ProcessoEmExecucaoId != processo.Id && ProcessoEmExecucaoId != 0)
            {
                NumeroTrocasContexto++;
                Console.WriteLine($"\n--- TROCA DE CONTEXTO ---");
                Console.WriteLine($"Overhead do sistema: {TempoSobrecargaTrocaContexto}ms.");
                System.Threading.Thread.Sleep(TempoSobrecargaTrocaContexto);
                Console.WriteLine($"--------------------------\n");

                TempoTotalCPUOcupadaMs += TempoSobrecargaTrocaContexto;

                Console.WriteLine($"--------------------------\n");
            }

            if (processo.TempoPrimeiraExecucao == null)
            {
                processo.TempoPrimeiraExecucao = DateTime.Now;
            }

            CpuEmUso = true;
            ProcessoEmExecucaoId = processo.Id;
            processo.Estado = Enums.Estados.Executando;

            int quantum = (Escalonador is EscalonadorRoundRobin rr) ? rr.Quantum : processo.TempoDeExecucaoTotal;
            int tempoParaExecutar = Math.Min(quantum, processo.TempoDeExecucaoTotal - processo.TempoExecutado);

            // Simula a passagem do tempo
            System.Threading.Thread.Sleep(tempoParaExecutar);
            processo.TempoExecutado += tempoParaExecutar;

            TempoTotalCPUOcupadaMs += tempoParaExecutar;

            Console.WriteLine($"Executando quantumn do processo '{processo.Nome}' (ID: {processo.Id}) por {tempoParaExecutar}ms.");

            Console.WriteLine($"Quantumn concluído. Processo '{processo.Nome}' executou por {processo.TempoExecutado}/{processo.TempoDeExecucaoTotal}ms no total.");

            if (processo.Terminou)
            {
                Console.WriteLine($"Processo '{processo.Nome}' (ID: {processo.Id}) terminou a execução.");
                FinalizarProcesso(processo.Id); // Finaliza o processo
            }
            else
            {
                Console.WriteLine($"Processo '{processo.Nome}' (ID: {processo.Id}) não terminou. Voltando para a fila.");
                Escalonador.AdicionarProcesso(processo); // Devolve para a fila
            }

            CpuEmUso = false;
            ProcessoEmExecucaoId = 0;
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

                processo.TempoFinalizacao = DateTime.Now;
                TimeSpan turnaround = (TimeSpan)(processo.TempoFinalizacao - processo.TempoChegada);
                processo.TempoDeEspera = turnaround - TimeSpan.FromMilliseconds(processo.TempoDeExecucaoTotal);

                List<int> framesLiberar = processo.TabelaDePaginas.ObterTodosFrames();
                GerenciadorMemoria.LiberarPaginas(framesLiberar);
                Console.WriteLine($"Memória liberada: {framesLiberar.Count} páginas ({framesLiberar.Count * TamanhoPagina}MB).");


                processo.Estado = Enums.Estados.Finalizado;
                this.Processos.Remove(processo);
                this.ProcessosFinalizados.Add(processo);

                Escalonador.RemoverProcessoDaFila(id);

                if (ProcessoEmExecucaoId == id)
                {
                    this.CpuEmUso = false;
                    this.ProcessoEmExecucaoId = 0;

                    Console.WriteLine($"Processo com ID {id} finalizado. CPU liberada.");

                    if (Escalonador.QuantidadeProcessosNaFila > 0)
                    {
                        Console.WriteLine("Executando próximo processo da fila...");
                        ExecutarProximoProcesso();
                    }
                }
                else
                {
                    Console.WriteLine($"Processo com ID {id} finalizado.");
                }

                MostrarStatusMemoria();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao finalizar o processo: {ex.Message}");
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
                    Console.WriteLine($"Processo com ID {processoId} não encontrado!");
                    return false;
                }

                // Calcular páginas e verificar memória
                int paginasAdicionais = (int)Math.Ceiling(memoriaThread / (float)this.TamanhoPagina);
                if (paginasAdicionais == 0) paginasAdicionais = 1;

                if (paginasAdicionais > GerenciadorMemoria.GetMoldurasDisponiveis())
                {
                    Console.WriteLine($"ERRO: Memória insuficiente para alocar thread!");
                    Console.WriteLine($"Páginas solicitadas: {paginasAdicionais} ({memoriaThread}MB)");
                    GerenciadorMemoria.MostrarStatusMemoria();
                    return false;
                }

                // Tentar alocar
                List<int> framesAlocados = GerenciadorMemoria.AlocarPaginas(processo.Id, paginasAdicionais);
                if (framesAlocados == null)
                {
                    Console.WriteLine($"ERRO: Falha na alocação (fragmentação?). Memória insuficiente.");
                    return false;
                }

                // Registrar alocação e adicionar thread
                List<int> paginasLogicas = processo.TabelaDePaginas.RegistrarAlocacao(framesAlocados);

                bool sucesso = processo.AdicionarThread(memoriaThread, paginasLogicas);

                if (sucesso)
                {
                    Console.WriteLine($"Thread alocada com sucesso! {paginasAdicionais} páginas alocadas.");
                    GerenciadorMemoria.MostrarStatusMemoria();
                }
                else
                {
                    Console.WriteLine("Falha ao criar thread. Revertendo alocação de memória...");

                    processo.TabelaDePaginas.LiberarPaginasEspecificas(paginasLogicas);

                    GerenciadorMemoria.LiberarPaginas(framesAlocados);
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
                // (verificação de processo nulo)

                // Finaliza a thread e pega o objeto
                Modelos.Thread thread = processo.FinalizarThread(threadId);

                if (thread != null)
                {
                    // Em vez de LiberarPaginasRecentes
                    List<int> framesLiberados = processo.TabelaDePaginas.LiberarPaginasEspecificas(thread.PaginasLogicasAlocadas);

                    // Devolver ao gerenciador de memória
                    GerenciadorMemoria.LiberarPaginas(framesLiberados);
                    Console.WriteLine($"Memória da thread liberada: {framesLiberados.Count} páginas.");
                    GerenciadorMemoria.MostrarStatusMemoria();
                }
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
            return GerenciadorMemoria.MemoriaTotal;
        }
        public int GetNumeroProcessos()
        {
            return this.Processos.Count;
        }
        public List<Processo> GetProcessosFinalizados()
        {
            return ProcessosFinalizados;
        }
        public double GetTempoTotalCPUOcupadaMs()
        {
            return TempoTotalCPUOcupadaMs;
        }

        public DateTime GetDataInicio()
        {
            return DataInicio;
        }
        public int GetTempoSobrecarga()
        {
            return TempoSobrecargaTrocaContexto;
        }

    }
}