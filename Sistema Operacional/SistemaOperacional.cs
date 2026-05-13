using Sistema_Operacional.Escalonamento;
using Sistema_Operacional.Modelos;
using Sistema_Operacional.Memoria;
using Sistema_Operacional.Enums;
using Sistema_Operacional.Utilidades;
using Sistema_Operacional.SistemaArquivos;
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
        private GerenciadorArquivos GerenciadorArquivos { get; set; }
        private int TamanhoPagina { get; set; }

        private List<Processo> ProcessosFinalizados = new List<Processo>();
        private double TempoTotalCPUUtilMs { get; set; } = 0;
        private double TempoTotalOverheadMs { get; set; } = 0;

        public SistemaOperacional(int totalMemoria, IEscalonador escalonadorInicial, int tempoSobrecarga, int tamanhoPagina)
        {
            NumeroProcessos = 0;
            CpuEmUso = false;
            Escalonador = escalonadorInicial;
            TempoSobrecargaTrocaContexto = tempoSobrecarga;

            this.TamanhoPagina = tamanhoPagina;
            this.GerenciadorMemoria = new GerenciadorMemoria(totalMemoria, tamanhoPagina);
            this.GerenciadorArquivos = new GerenciadorArquivos(512);
        }

        public void CriarProcesso(string nome, int priority = 5, float memoriaInicial = 10f)
        {
            int paginasNecessarias = (int)Math.Ceiling(memoriaInicial / (float)this.TamanhoPagina);
            if (paginasNecessarias == 0) paginasNecessarias = 1;

            int novoId = (Processos.Any() ? Processos.Max(p => p.Id) : 0) + 1;
            List<int> framesAlocados = GerenciadorMemoria.AlocarPaginas(novoId, paginasNecessarias);

            if (framesAlocados == null)
            {
                Console.WriteLine($"ERRO: Memória insuficiente para criar processo '{nome}'. {paginasNecessarias} páginas solicitadas.");
                GerenciadorMemoria.MostrarStatusMemoria();
                return;
            }

            var novoProcesso = new Processo(nome, novoId, priority);
            List<int> paginasLogicas = novoProcesso.TabelaDePaginas.RegistrarAlocacao(framesAlocados);
            bool sucesso = novoProcesso.AdicionarThread(memoriaInicial, paginasLogicas);

            if (sucesso)
            {
                this.Processos.Add(novoProcesso);
                Escalonador.AdicionarProcesso(novoProcesso);

                Logger.Registrar($"PROCESSO CRIADO: '{nome}' (ID: {novoId}, Prioridade: {priority}, Memória: {memoriaInicial}MB, Páginas: {paginasNecessarias})");
                
                Console.WriteLine($"Processo '{nome}' (ID {novoId}) criado. {paginasNecessarias} páginas ({memoriaInicial}MB) alocadas.");
                GerenciadorMemoria.MostrarStatusMemoria();
            }
            else
            {
                Logger.Registrar($"ERRO: Falha ao criar processo '{nome}' - Thread inicial não criada");
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

        public void MostrarEstatisticasMemoria()
        {
            // Fragmentação interna: diferença entre memória alocada em páginas e memória real usada
            float totalFragmentacaoMB = 0;
            foreach (var p in Processos)
            {
                float memoriaReal = p.CalcularMemoriaTotal();
                float memoriaAlocada = p.TabelaDePaginas.TotalPaginas() * TamanhoPagina;
                if (memoriaAlocada > memoriaReal)
                    totalFragmentacaoMB += memoriaAlocada - memoriaReal;
            }
            float percentFragmentacao = GerenciadorMemoria.MemoriaTotal > 0
                ? (totalFragmentacaoMB / GerenciadorMemoria.MemoriaTotal) * 100
                : 0;

            GerenciadorMemoria.MostrarEstatisticasGerais(percentFragmentacao);

            Console.WriteLine("=== ESTATÍSTICAS POR PROCESSO ===");
            foreach (var processo in Processos.OrderBy(p => p.Id))
            {
                Console.WriteLine($"\nProcesso: {processo.Nome} (ID: {processo.Id})");
                processo.TabelaDePaginas.MostrarEstatisticas();
            }
        }

        public void SimularAcessoMemoria(int processoId, int paginaLogica)
        {
            var processo = Processos.FirstOrDefault(p => p.Id == processoId);
            if (processo == null)
            {
                Console.WriteLine($"Processo com ID {processoId} não encontrado.");
                return;
            }

            int frameFisico = processo.TabelaDePaginas.TraduzirEndereco(paginaLogica);
            
            if (frameFisico >= 0)
            {
                Console.WriteLine($"[Acesso] Processo {processoId} | Página Lógica {paginaLogica} → Frame Físico {frameFisico}");
            }
            else
            {
                Console.WriteLine($"[PAGE FAULT] Processo {processoId} | Página Lógica {paginaLogica} não encontrada!");
            }
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
                Logger.Registrar($"TROCA DE CONTEXTO: Processo {ProcessoEmExecucaoId} -> {processo.Id} (Overhead: {TempoSobrecargaTrocaContexto}ms)");
                
                Console.WriteLine($"\n--- TROCA DE CONTEXTO ---");
                Console.WriteLine($"Overhead do sistema: {TempoSobrecargaTrocaContexto}ms.");
                System.Threading.Thread.Sleep(TempoSobrecargaTrocaContexto);
                Console.WriteLine($"--------------------------\n");

                TempoTotalOverheadMs += TempoSobrecargaTrocaContexto;
            }

            if (processo.TempoPrimeiraExecucao == null)
            {
                processo.TempoPrimeiraExecucao = DateTime.Now;
                Logger.Registrar($"PRIMEIRA EXECUÇÃO: Processo '{processo.Nome}' (ID: {processo.Id})");
            }

            CpuEmUso = true;
            ProcessoEmExecucaoId = processo.Id;
            processo.Estado = Enums.Estados.Executando;

            int quantum = (Escalonador is EscalonadorRoundRobin rr) ? rr.Quantum : processo.TempoDeExecucaoTotal;
            int tempoParaExecutar = Math.Min(quantum, processo.TempoDeExecucaoTotal - processo.TempoExecutado);

            System.Threading.Thread.Sleep(tempoParaExecutar);
            processo.TempoExecutado += tempoParaExecutar;

            TempoTotalCPUUtilMs += tempoParaExecutar;

            Logger.Registrar($"EXECUÇÃO: Processo '{processo.Nome}' (ID: {processo.Id}) executou {tempoParaExecutar}ms ({processo.TempoExecutado}/{processo.TempoDeExecucaoTotal}ms)");
            
            Console.WriteLine($"Executando quantum do processo '{processo.Nome}' (ID: {processo.Id}) por {tempoParaExecutar}ms.");
            Console.WriteLine($"Quantum concluído. Processo '{processo.Nome}' executou por {processo.TempoExecutado}/{processo.TempoDeExecucaoTotal}ms no total.");

            if (processo.Terminou)
            {
                Logger.Registrar($"PROCESSO COMPLETO: '{processo.Nome}' (ID: {processo.Id}) finalizou sua execução");
                Console.WriteLine($"Processo '{processo.Nome}' (ID: {processo.Id}) terminou a execução.");
                FinalizarProcesso(processo.Id);
            }
            else
            {
                processo.Estado = Estados.Pronto;
                Console.WriteLine($"Processo '{processo.Nome}' (ID: {processo.Id}) voltando ao estado Pronto.");
                Escalonador.AdicionarProcesso(processo);
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
                
                Logger.Registrar($"PROCESSO FINALIZADO: '{processo.Nome}' (ID: {id}) - Memória liberada: {framesLiberar.Count} páginas ({framesLiberar.Count * TamanhoPagina}MB)");
                Logger.Registrar($"  Métricas - Turnaround: {turnaround.TotalMilliseconds:F0}ms, Espera: {processo.TempoDeEspera.TotalMilliseconds:F0}ms");
                
                Console.WriteLine($"Memória liberada: {framesLiberar.Count} páginas ({framesLiberar.Count * TamanhoPagina}MB).");

                processo.Estado = Enums.Estados.Finalizado;
                this.Processos.Remove(processo);
                this.ProcessosFinalizados.Add(processo);

                Escalonador.RemoverProcessoDaFila(id);

                if (ProcessoEmExecucaoId == id)
                {
                    this.CpuEmUso = false;
                    this.ProcessoEmExecucaoId = 0;

                    Logger.Registrar($"CPU LIBERADA após finalização do processo ID: {id}");
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

                if (processo.Estado == Enums.Estados.Bloqueado)
                {
                    Console.WriteLine($"Processo com ID {id} já está pausado.");
                    return;
                }

                if (ProcessoEmExecucaoId == id)
                {
                    processo.Estado = Enums.Estados.Bloqueado;
                    this.CpuEmUso = false;
                    this.ProcessoEmExecucaoId = 0;
                    
                    Logger.Registrar($"PROCESSO PAUSADO: '{processo.Nome}' (ID: {id}) - CPU liberada");
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
                    
                    Logger.Registrar($"PROCESSO PAUSADO: '{processo.Nome}' (ID: {id})");
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

                processo.Estado = Estados.Pronto;
                Escalonador.AdicionarProcesso(processo);
                
                Logger.Registrar($"PROCESSO RETOMADO: '{processo.Nome}' (ID: {id}) - Adicionado à fila de prontos");
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

                int paginasAdicionais = (int)Math.Ceiling(memoriaThread / (float)this.TamanhoPagina);
                if (paginasAdicionais == 0) paginasAdicionais = 1;

                if (paginasAdicionais > GerenciadorMemoria.GetMoldurasDisponiveis())
                {
                    Console.WriteLine($"ERRO: Memória insuficiente para alocar thread!");
                    Console.WriteLine($"Páginas solicitadas: {paginasAdicionais} ({memoriaThread}MB)");
                    GerenciadorMemoria.MostrarStatusMemoria();
                    return false;
                }

                List<int> framesAlocados = GerenciadorMemoria.AlocarPaginas(processo.Id, paginasAdicionais);
                if (framesAlocados == null)
                {
                    Console.WriteLine($"ERRO: Falha na alocação (fragmentação?). Memória insuficiente.");
                    return false;
                }

                List<int> paginasLogicas = processo.TabelaDePaginas.RegistrarAlocacao(framesAlocados);
                bool sucesso = processo.AdicionarThread(memoriaThread, paginasLogicas);

                if (sucesso)
                {
                    Logger.Registrar($"THREAD ADICIONADA: Processo '{processo.Nome}' (ID: {processoId}) - {paginasAdicionais} páginas ({memoriaThread}MB)");
                    Console.WriteLine($"Thread alocada com sucesso! {paginasAdicionais} páginas alocadas.");
                    GerenciadorMemoria.MostrarStatusMemoria();
                }
                else
                {
                    Logger.Registrar($"ERRO: Falha ao criar processo '{processoId}' - Adição de thread falhou");
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

                Modelos.Thread thread = processo.FinalizarThread(threadId);

                if (thread != null)
                {
                    List<int> framesLiberados = processo.TabelaDePaginas.LiberarPaginasEspecificas(thread.PaginasLogicasAlocadas);
                    GerenciadorMemoria.LiberarPaginas(framesLiberados);
                    
                    Logger.Registrar($"THREAD FINALIZADA: Processo '{processo.Nome}' (ID: {processoId}), Thread ID: {threadId} - {framesLiberados.Count} páginas liberadas");
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

                if (thread.Estado == Estados.Bloqueado)
                {
                    Console.WriteLine($"Thread com ID {threadId} já está pausada.");
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

                if (thread.Estado != Estados.Bloqueado)
                {
                    Console.WriteLine($"Thread com ID {threadId} não está pausada.");
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

        public double GetTempoTotalCPUUtilMs()
        {
            return TempoTotalCPUUtilMs;
        }

        public double GetTempoTotalOverheadMs()
        {
            return TempoTotalOverheadMs;
        }

        public DateTime GetDataInicio()
        {
            return DataInicio;
        }

        public int GetTempoSobrecarga()
        {
            return TempoSobrecargaTrocaContexto;
        }

        // ─── SISTEMA DE ARQUIVOS ─────────────────────────────────────────────────

        public bool CriarArquivo(string nome, string conteudo, int processoId = -1)
        {
            bool resultado = GerenciadorArquivos.CriarArquivo(nome, GerenciadorArquivos.DiretorioRaiz, conteudo);
            if (resultado && processoId > 0)
            {
                var processo = Processos.FirstOrDefault(p => p.Id == processoId);
                if (processo != null && !processo.TabelaArquivosAbertos.Contains(nome))
                {
                    processo.TabelaArquivosAbertos.Add(nome);
                    Logger.Registrar($"ARQUIVO ABERTO: '{nome}' pelo processo '{processo.Nome}' (ID: {processoId})");
                }
            }
            return resultado;
        }

        public bool CriarDiretorio(string nome)
        {
            return GerenciadorArquivos.CriarDiretorio(nome, GerenciadorArquivos.DiretorioRaiz);
        }

        public string LerArquivo(string nome, int processoId = -1)
        {
            string conteudo = GerenciadorArquivos.LerArquivo(nome, GerenciadorArquivos.DiretorioRaiz);
            if (conteudo != null && processoId > 0)
            {
                var processo = Processos.FirstOrDefault(p => p.Id == processoId);
                if (processo != null && !processo.TabelaArquivosAbertos.Contains(nome))
                {
                    processo.TabelaArquivosAbertos.Add(nome);
                    Logger.Registrar($"ARQUIVO ABERTO: '{nome}' pelo processo '{processo.Nome}' (ID: {processoId})");
                }
            }
            return conteudo;
        }

        public bool EscreverArquivo(string nome, string novoConteudo)
        {
            return GerenciadorArquivos.EscreverArquivo(nome, GerenciadorArquivos.DiretorioRaiz, novoConteudo);
        }

        public bool DeletarArquivo(string nome)
        {
            // Remove da tabela de arquivos abertos de todos os processos
            foreach (var p in Processos)
                p.TabelaArquivosAbertos.Remove(nome);

            return GerenciadorArquivos.DeletarArquivo(nome, GerenciadorArquivos.DiretorioRaiz);
        }

        public void ListarDiretorioRaiz()
        {
            GerenciadorArquivos.ListarDiretorio(GerenciadorArquivos.DiretorioRaiz);
        }

        public void MostrarStatusDisco()
        {
            GerenciadorArquivos.MostrarStatusDisco();
        }

        public Arquivo GetDiretorioRaiz() => GerenciadorArquivos.DiretorioRaiz;

        // ─── SUSPENSÃO DE PROCESSOS (Estados ProntoSuspenso / EsperaSuspensa) ───

        public void SuspenderProcesso(int id)
        {
            try
            {
                var processo = Processos.FirstOrDefault(p => p.Id == id);
                if (processo == null)
                {
                    Console.WriteLine($"Processo com ID {id} não encontrado.");
                    return;
                }

                if (processo.Estado == Estados.ProntoSuspenso || processo.Estado == Estados.EsperaSuspensa)
                {
                    Console.WriteLine($"Processo '{processo.Nome}' (ID: {id}) já está suspenso.");
                    return;
                }

                if (processo.Estado == Estados.Finalizado)
                {
                    Console.WriteLine($"Processo '{processo.Nome}' (ID: {id}) já foi finalizado.");
                    return;
                }

                // Determina o estado suspenso: se bloqueado → EsperaSuspensa, caso contrário → ProntoSuspenso
                Estados novoEstado = processo.Estado == Estados.Bloqueado
                    ? Estados.EsperaSuspensa
                    : Estados.ProntoSuspenso;

                // Salva quantas páginas estavam alocadas para poder restaurar depois
                processo.PaginasAntesDaSuspensao = processo.TabelaDePaginas.TotalPaginas();

                // Libera os frames físicos e limpa o mapeamento (processo vai para "disco")
                List<int> frames = processo.TabelaDePaginas.ObterTodosFrames();
                processo.TabelaDePaginas.LimparMapeamento();
                GerenciadorMemoria.LiberarPaginas(frames);

                // Remove da fila de prontos se estava pronto/executando
                if (novoEstado == Estados.ProntoSuspenso)
                    Escalonador.RemoverProcessoDaFila(id);

                if (ProcessoEmExecucaoId == id)
                {
                    CpuEmUso = false;
                    ProcessoEmExecucaoId = 0;
                }

                processo.Estado = novoEstado;
                Logger.Registrar($"PROCESSO SUSPENSO: '{processo.Nome}' (ID: {id}) → {novoEstado} | {frames.Count} páginas liberadas para memória");
                Console.WriteLine($"Processo '{processo.Nome}' (ID: {id}) suspenso ({novoEstado}).");
                Console.WriteLine($"{frames.Count} página(s) devolvida(s) à memória física (processo em disco virtual).");
                MostrarStatusMemoria();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao suspender processo: {ex.Message}");
            }
        }

        public void ReativarProcessoSuspenso(int id)
        {
            try
            {
                var processo = Processos.FirstOrDefault(p => p.Id == id);
                if (processo == null)
                {
                    Console.WriteLine($"Processo com ID {id} não encontrado.");
                    return;
                }

                if (processo.Estado != Estados.ProntoSuspenso && processo.Estado != Estados.EsperaSuspensa)
                {
                    Console.WriteLine($"Processo '{processo.Nome}' (ID: {id}) não está suspenso.");
                    return;
                }

                int paginasNecessarias = processo.PaginasAntesDaSuspensao;
                if (paginasNecessarias <= 0)
                    paginasNecessarias = Math.Max(1, (int)Math.Ceiling(processo.CalcularMemoriaTotal() / (float)TamanhoPagina));

                List<int> novosFrames = GerenciadorMemoria.AlocarPaginas(processo.Id, paginasNecessarias);
                if (novosFrames == null)
                {
                    Console.WriteLine($"ERRO: Memória insuficiente para reativar '{processo.Nome}'. Necessário: {paginasNecessarias} página(s).");
                    Console.WriteLine("Sugestão: suspenda outro processo para liberar memória.");
                    MostrarStatusMemoria();
                    return;
                }

                processo.TabelaDePaginas.RegistrarAlocacao(novosFrames);

                // Restaura estado: EsperaSuspensa → Bloqueado, ProntoSuspenso → Pronto
                bool eraEsperaSuspensa = processo.Estado == Estados.EsperaSuspensa;
                processo.Estado = eraEsperaSuspensa ? Estados.Bloqueado : Estados.Pronto;
                processo.PaginasAntesDaSuspensao = 0;

                if (processo.Estado == Estados.Pronto)
                    Escalonador.AdicionarProcesso(processo);

                Logger.Registrar($"PROCESSO REATIVADO: '{processo.Nome}' (ID: {id}) | {novosFrames.Count} páginas realocadas → {processo.Estado}");
                Console.WriteLine($"Processo '{processo.Nome}' (ID: {id}) reativado. {novosFrames.Count} página(s) realocada(s).");
                Console.WriteLine($"Estado: {processo.Estado}{(processo.Estado == Estados.Bloqueado ? " (ainda bloqueado — use Retomar para colocar na fila)" : "")}");
                MostrarStatusMemoria();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao reativar processo: {ex.Message}");
            }
        }
    }
}