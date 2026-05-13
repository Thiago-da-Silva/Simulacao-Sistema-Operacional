using Sistema_Operacional;
using Sistema_Operacional.Escalonamento;
using Sistema_Operacional.Modelos;
using Sistema_Operacional.Utilidades;
using System.Runtime.CompilerServices;

public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== CONFIGURAÇÃO INICIAL DO SISTEMA ===");
        Console.Write("Digite uma seed para o gerador aleatório (deixe vazio para usar seed baseada no tempo): ");
        string seedInput = Console.ReadLine();
        
        int seed;
        if (string.IsNullOrWhiteSpace(seedInput))
        {
            seed = Environment.TickCount;
            Console.WriteLine($"Usando seed baseada no tempo: {seed}");
        }
        else if (int.TryParse(seedInput, out seed))
        {
            Console.WriteLine($"Usando seed fornecida: {seed}");
        }
        else
        {
            seed = Environment.TickCount;
            Console.WriteLine($"Entrada inválida. Usando seed baseada no tempo: {seed}");
        }
        
        AleatorioSistema.Inicializar(seed);
        Logger.LimparLog();
        Logger.Registrar("Sistema Operacional Iniciado");
        
        Console.WriteLine("Nota: Use a mesma seed para reproduzir resultados idênticos.");
        Console.WriteLine();

        Console.WriteLine("Escolha o algoritmo de escalonamento:");
        Console.WriteLine("1 - FCFS (First Come, First Served)");
        Console.WriteLine("2 - Prioridades (Não Preemptivo)");
        Console.WriteLine("3 - Round Robin");
        Console.Write("Opção: ");
        string escolha = Console.ReadLine();

        IEscalonador escalonador;
        string nomeEscalonador;

        switch (escolha)
        {
            case "2":
                escalonador = new EscalonadorPrioridades();
                nomeEscalonador = "Prioridades (Não Preemptivo)";
                break;
            case "3":
                Console.Write("Digite o valor do Quantum (em ms): ");
                if (!int.TryParse(Console.ReadLine(), out int quantum) || quantum <= 0)
                {
                    Console.WriteLine("Valor inválido. Usando quantum padrão de 100ms.");
                    quantum = 100;
                }
                escalonador = new EscalonadorRoundRobin(quantum);
                nomeEscalonador = $"Round Robin (Quantum: {quantum}ms)";
                break;
            default:
                escalonador = new EscalonadorFCFS();
                nomeEscalonador = "FCFS (First Come, First Served)";
                break;
        }

        Console.Write("Digite o tempo de sobrecarga para troca de contexto (em ms): ");
        if (!int.TryParse(Console.ReadLine(), out int sobrecarga) || sobrecarga < 0)
        {
            Console.WriteLine("Valor inválido. Usando sobrecarga padrão de 10ms.");
            sobrecarga = 10;
        }

        Console.Write("Digite o Tamanho da Página/Moldura (em MB, ex: 4): ");
        if (!int.TryParse(Console.ReadLine(), out int tamanhoPagina) || tamanhoPagina <= 0)
        {
            Console.WriteLine("Valor inválido. Usando padrão de 4MB.");
            tamanhoPagina = 4;
        }

        Console.Clear();

        SistemaOperacional sistema = new SistemaOperacional(1024, escalonador, sobrecarga, tamanhoPagina);
        Logger.Registrar($"Escalonador: {nomeEscalonador}, Memória: 1024MB, Sobrecarga: {sobrecarga}ms, Página: {tamanhoPagina}MB");
        
        Console.WriteLine($"Sistema Operacional Iniciado com Escalonador: {nomeEscalonador}.");
        Console.WriteLine("=========================================================================");
        Console.WriteLine($"Memória Total: {sistema.GetTotalMemoria()}MB");
        Console.WriteLine($"Sistema iniciado em: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
        Console.WriteLine();

        bool continuar = true;

        while (continuar)
        {
            MostrarMenu(sistema);
            string opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    CriarProcesso(sistema);
                    break;
                case "2":
                    ExecutarProximoProcesso(sistema);
                    break;
                case "3":
                    FinalizarProcesso(sistema);
                    break;
                case "4":
                    PausarProcesso(sistema);
                    break;
                case "5":
                    RetomarProcesso(sistema);
                    break;
                case "6":
                    AdicionarThread(sistema);
                    break;
                case "7":
                    ListarThreadsProcesso(sistema);
                    break;
                case "8":
                    FinalizarThread(sistema);
                    break;
                case "9":
                    PausarThread(sistema);
                    break;
                case "10":
                    RetomarThread(sistema);
                    break;
                case "11":
                    sistema.ListarProcessos();
                    break;
                case "12":
                    sistema.ListarFilaProcessos();
                    break;
                case "13":
                    sistema.MostrarStatusCPU();
                    break;
                case "14":
                    sistema.MostrarStatusMemoria();
                    break;
                case "15":
                    MostrarInformacoesSistema(sistema, nomeEscalonador);
                    break;
                case "16":
                    ExecutarDemo(sistema);
                    break;
                case "17":
                    sistema.MostrarEstatisticasMemoria();
                    break;
                case "18":
                    SimularAcessoMemoria(sistema);
                    break;
                case "19":
                    VisualizarLog();
                    break;
                case "20":
                    CriarArquivo(sistema);
                    break;
                case "21":
                    LerArquivo(sistema);
                    break;
                case "22":
                    EscreverArquivo(sistema);
                    break;
                case "23":
                    DeletarArquivo(sistema);
                    break;
                case "24":
                    CriarDiretorio(sistema);
                    break;
                case "25":
                    sistema.ListarDiretorioRaiz();
                    break;
                case "26":
                    sistema.MostrarStatusDisco();
                    break;
                case "27":
                    SuspenderProcesso(sistema);
                    break;
                case "28":
                    ReativarProcessoSuspenso(sistema);
                    break;
                case "99":
                    Console.Clear();
                    Console.WriteLine("Tela limpa!");
                    break;
                case "0":
                    continuar = false;
                    Logger.Registrar("Sistema Operacional Encerrado");
                    Console.WriteLine("Encerrando Sistema Operacional...");
                    MostrarMetricasFinais(sistema);
                    break;
                default:
                    Console.WriteLine("Opção inválida! Tente novamente.");
                    break;
            }

            if (continuar)
            {
                Console.WriteLine("\nPressione qualquer tecla para continuar...");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }

    static void MostrarMenu(SistemaOperacional sistema)
    {
        Console.WriteLine("╔══════════════════════════════════════════════╗");
        Console.WriteLine("║         MENU DO SISTEMA OPERACIONAL          ║");
        Console.WriteLine("╠══════════════════════════════════════════════╣");
        Console.WriteLine("║ GERENCIAMENTO DE PROCESSOS                   ║");
        Console.WriteLine("║ 1  - Criar Processo                          ║");
        Console.WriteLine("║ 2 - Executar Próximo Quantum/Processo        ║");
        Console.WriteLine("║ 3  - Finalizar Processo                      ║");
        Console.WriteLine("║ 4  - Pausar Processo                         ║");
        Console.WriteLine("║ 5  - Retomar Processo                        ║");
        Console.WriteLine("║                                              ║");
        Console.WriteLine("║ GERENCIAMENTO DE THREADS                     ║");
        Console.WriteLine("║ 6  - Adicionar Thread a Processo             ║");
        Console.WriteLine("║ 7  - Listar Threads de Processo              ║");
        Console.WriteLine("║ 8  - Finalizar Thread                        ║");
        Console.WriteLine("║ 9  - Pausar Thread                           ║");
        Console.WriteLine("║ 10 - Retomar Thread                          ║");
        Console.WriteLine("║                                              ║");
        Console.WriteLine("║ INFORMAÇÕES DO SISTEMA                       ║");
        Console.WriteLine("║ 11 - Listar Todos os Processos               ║");
        Console.WriteLine("║ 12 - Mostrar Fila de Prontos                 ║");
        Console.WriteLine("║ 13 - Mostrar Status da CPU                   ║");
        Console.WriteLine("║ 14 - Mostrar Status da Memória               ║");
        Console.WriteLine("║ 15 - Informações do Sistema                  ║");
        Console.WriteLine("║ 16 - Executar Demonstração                   ║");
        Console.WriteLine("║ 17 - Estatísticas de Memória (TLB)           ║");
        Console.WriteLine("║ 18 - Simular Acesso à Memória                ║");
        Console.WriteLine("║ 19 - Visualizar Log da Simulação             ║");
        Console.WriteLine("║                                              ║");
        Console.WriteLine("║ SISTEMA DE ARQUIVOS                          ║");
        Console.WriteLine("║ 20 - Criar Arquivo                           ║");
        Console.WriteLine("║ 21 - Ler Arquivo                             ║");
        Console.WriteLine("║ 22 - Escrever no Arquivo                     ║");
        Console.WriteLine("║ 23 - Deletar Arquivo/Diretório               ║");
        Console.WriteLine("║ 24 - Criar Diretório                         ║");
        Console.WriteLine("║ 25 - Listar Diretório Raiz                   ║");
        Console.WriteLine("║ 26 - Status do Disco Virtual                 ║");
        Console.WriteLine("║                                              ║");
        Console.WriteLine("║ SUSPENSÃO DE PROCESSOS                       ║");
        Console.WriteLine("║ 27 - Suspender Processo (→ disco)            ║");
        Console.WriteLine("║ 28 - Reativar Processo Suspenso              ║");
        Console.WriteLine("║                                              ║");
        Console.WriteLine("║ 99 - Limpar Tela                             ║");
        Console.WriteLine("║ 0  - Sair                                    ║");
        Console.WriteLine("╚══════════════════════════════════════════════╝");

        // Mostra status resumido
        string statusCpu = sistema.IsCpuEmUso() ? "EM USO" : "LIVRE";
        float memoriaUsada = sistema.CalcularMemoriaUsada();
        float memoriaTotal = sistema.GetTotalMemoria();
        float percentualMemoria = (memoriaUsada / memoriaTotal) * 100;

        Console.WriteLine($"Status: CPU {statusCpu} | Processos: {sistema.GetNumeroProcessos()} | Memória: {memoriaUsada:F1}/{memoriaTotal}MB ({percentualMemoria:F1}%)");
        Console.WriteLine();
    }

    static void CriarProcesso(SistemaOperacional sistema)
    {
        Console.Write("Digite o nome do processo: ");
        string nome = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(nome))
        {
            Console.WriteLine("Nome inválido!");
            return;
        }

        Console.Write("Digite a prioridade do processo (ex: 1 = alta, 5 = baixa): ");
        if (!int.TryParse(Console.ReadLine(), out int prioridade) || prioridade <= 0)
        {
            Console.WriteLine("Prioridade inválida! Usando prioridade padrão (5).");
            prioridade = 5;
        }

        Console.Write("Digite a memória inicial (MB) para o processo (ex: 50): ");
        if (!float.TryParse(Console.ReadLine(), out float memoriaInicial) || memoriaInicial <= 0)
        {
            Console.WriteLine("Memória inválida! Usando padrão de 10MB.");
            memoriaInicial = 10;
        }

        sistema.CriarProcesso(nome, prioridade, memoriaInicial);

    }

    static void ExecutarProximoProcesso(SistemaOperacional sistema)
    {
        sistema.ExecutarProximoProcesso();
    }

    static void FinalizarProcesso(SistemaOperacional sistema)
    {
        Console.Write("Digite o ID do processo a finalizar: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            sistema.FinalizarProcesso(id);
        }
        else
        {
            Console.WriteLine("ID inválido!");
        }
    }

    static void PausarProcesso(SistemaOperacional sistema)
    {
        Console.Write("Digite o ID do processo a pausar: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            sistema.PausarProcesso(id);
        }
        else
        {
            Console.WriteLine("ID inválido!");
        }
    }

    static void RetomarProcesso(SistemaOperacional sistema)
    {
        Console.Write("Digite o ID do processo a retomar: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            sistema.RetomarProcesso(id);
        }
        else
        {
            Console.WriteLine("ID inválido!");
        }
    }

    static void AdicionarThread(SistemaOperacional sistema)
    {
        Console.WriteLine("ADICIONAR THREAD");
        Console.WriteLine("==================");

        // Mostra status da memória
        sistema.MostrarStatusMemoria();

        // Lista processos disponíveis
        sistema.ListarProcessos();

        Console.Write("Digite o ID do processo para adicionar thread: ");
        if (!int.TryParse(Console.ReadLine(), out int processoId))
        {
            Console.WriteLine("ID inválido!");
            return;
        }

        // Verifica se o processo existe
        var processo = sistema.ObterProcessoPorId(processoId);
        if (processo == null)
        {
            Console.WriteLine($"Processo com ID {processoId} não encontrado!");
            return;
        }

        Console.WriteLine($"Processo selecionado: {processo.Nome} (ID: {processo.Id})");
        Console.WriteLine($"Memória Lógica (soma das threads): {processo.CalcularMemoriaTotal():F2}MB");
        Console.WriteLine($"Memória Física (páginas alocadas): {processo.TabelaDePaginas.TotalPaginas()} páginas");
        Console.WriteLine($"Memória disponível no sistema: {sistema.CalcularMemoriaDisponivel():F2}MB");
        Console.WriteLine();

        Console.Write("Digite a quantidade de memória para a thread (MB): ");
        if (!float.TryParse(Console.ReadLine(), out float memoriaThread) || memoriaThread <= 0)
        {
            Console.WriteLine("Valor de memória inválido! Deve ser um número positivo.");
            return;
        }

        // Tenta adicionar a thread
        bool sucesso = sistema.AdicionarThreadAoProcesso(processoId, memoriaThread);

        if (sucesso)
        {
            Console.WriteLine("Thread adicionada com sucesso!");
        }
        else
        {
            Console.WriteLine("Falha ao adicionar thread!");
        }
    }

    static void ListarThreadsProcesso(SistemaOperacional sistema)
    {
        Console.Write("Digite o ID do processo para listar threads: ");
        if (int.TryParse(Console.ReadLine(), out int processoId))
        {
            sistema.ListarThreadsDoProcesso(processoId);
        }
        else
        {
            Console.WriteLine("ID inválido!");
        }
    }

    static void FinalizarThread(SistemaOperacional sistema)
    {
        Console.Write("Digite o ID do processo: ");
        if (int.TryParse(Console.ReadLine(), out int processoId))
        {
            Console.Write("Digite o ID da thread a finalizar: ");
            if (int.TryParse(Console.ReadLine(), out int threadId))
            {
                sistema.FinalizarThreadDoProcesso(processoId, threadId);
            }
            else
            {
                Console.WriteLine("ID da thread inválido!");
            }
        }
        else
        {
            Console.WriteLine("ID do processo inválido!");
        }
    }

    static void PausarThread(SistemaOperacional sistema)
    {
        Console.Write("Digite o ID do processo: ");
        if (int.TryParse(Console.ReadLine(), out int processoId))
        {
            Console.Write("Digite o ID da thread a pausar: ");
            if (int.TryParse(Console.ReadLine(), out int threadId))
            {
                sistema.PausarThreadDoProcesso(processoId, threadId);
            }
            else
            {
                Console.WriteLine("ID da thread inválido!");
            }
        }
        else
        {
            Console.WriteLine("ID do processo inválido!");
        }
    }

    static void RetomarThread(SistemaOperacional sistema)
    {
        Console.Write("Digite o ID do processo: ");
        if (int.TryParse(Console.ReadLine(), out int processoId))
        {
            Console.Write("Digite o ID da thread a retomar: ");
            if (int.TryParse(Console.ReadLine(), out int threadId))
            {
                sistema.RetomarThreadDoProcesso(processoId, threadId);
            }
            else
            {
                Console.WriteLine("ID da thread inválido!");
            }
        }
        else
        {
            Console.WriteLine("ID do processo inválido!");
        }
    }

    static void MostrarInformacoesSistema(SistemaOperacional sistema, string nomeEscalonador)
    {
        Console.WriteLine("╔══════════════════════════════════════════════╗");
        Console.WriteLine("║           INFORMAÇÕES DO SISTEMA             ║");
        Console.WriteLine("╠══════════════════════════════════════════════╣");

        float memoriaUsada = sistema.CalcularMemoriaUsada();
        float memoriaTotal = sistema.GetTotalMemoria();
        float memoriaDisponivel = sistema.CalcularMemoriaDisponivel();
        float percentualUso = (memoriaUsada / memoriaTotal) * 100;

        Console.WriteLine($"║ Memória Total: {memoriaTotal}MB".PadRight(47) + "║");
        Console.WriteLine($"║ Memória Usada: {memoriaUsada:F2}MB ({percentualUso:F1}%)".PadRight(55) + "║");
        Console.WriteLine($"║ Memória Disponível: {memoriaDisponivel:F2}MB".PadRight(55) + "║");
        Console.WriteLine($"║ Processos Ativos: {sistema.GetNumeroProcessos()}".PadRight(47) + "║");
        Console.WriteLine($"║ Trocas de Contexto: {sistema.NumeroTrocasContexto}".PadRight(47) + "║");

        string statusCpu = sistema.IsCpuEmUso() ? "EM USO" : "LIVRE";
        Console.WriteLine($"║ Status da CPU: {statusCpu}".PadRight(55) + "║");

        if (sistema.IsCpuEmUso())
        {
            int processoAtualId = sistema.GetProcessoEmExecucaoId();
            var processoAtual = sistema.ObterProcessoPorId(processoAtualId);
            if (processoAtual != null)
            {
                Console.WriteLine($"║ Processo Executando: {processoAtual.Nome} (ID: {processoAtual.Id})".PadRight(55) + "║");
                Console.WriteLine($"║ Memória Lógica: {processoAtual.CalcularMemoriaTotal():F2}MB".PadRight(55) + "║");
                Console.WriteLine($"║ Páginas Alocadas: {processoAtual.TabelaDePaginas.TotalPaginas()}".PadRight(47) + "║");
                Console.WriteLine($"║ Threads: {processoAtual.Threads.Count}".PadRight(47) + "║");
            }
        }
        Console.WriteLine($"║ Sistema iniciado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}".PadRight(47) + "║");
        Console.WriteLine($"║ Seed Aleatória: {AleatorioSistema.GetSeedAtual()}".PadRight(47) + "║");
        Console.WriteLine("║                                              ║");
        Console.WriteLine($"║ Algoritmo: {nomeEscalonador}".PadRight(55) + "║");
        Console.WriteLine("║ Gerenciamento de Memória: Ativo             ║");
        Console.WriteLine("║    (Validação automática de limites)        ║");
        Console.WriteLine("╚══════════════════════════════════════════════╝");
    }

    static void ExecutarDemo(SistemaOperacional sistema)
    {
        Console.WriteLine("DEMONSTRAÇÃO DO ESCALONADOR FCFS");
        Console.WriteLine("===================================\n");

        Console.WriteLine("1. Criando processos de exemplo...");
        sistema.CriarProcesso("Editor de Texto");
        System.Threading.Thread.Sleep(50);
        sistema.CriarProcesso("Navegador Web");
        System.Threading.Thread.Sleep(50);
        sistema.CriarProcesso("Player de Música");
        System.Threading.Thread.Sleep(50);
        sistema.CriarProcesso("Calculadora");

        Console.WriteLine("\n2. Visualizando fila FCFS:");
        sistema.ListarFilaProcessos();

        Console.WriteLine("3. Status da CPU:");
        sistema.MostrarStatusCPU();

        Console.WriteLine("4. Status inicial da memória:");
        sistema.MostrarStatusMemoria();

        Console.WriteLine("5. Executando primeiro processo...");
        sistema.ExecutarProximoProcesso();

        Console.WriteLine("\n6. Adicionando threads ao processo em execução...");
        int processoAtualId = sistema.GetProcessoEmExecucaoId();
        if (processoAtualId > 0)
        {
            Console.WriteLine("Adicionando thread com 50MB de memória...");
            sistema.AdicionarThreadAoProcesso(processoAtualId, 50.0f);

            Console.WriteLine("Adicionando thread com 75MB de memória...");
            sistema.AdicionarThreadAoProcesso(processoAtualId, 75.0f);

            Console.WriteLine("Tentando adicionar thread com memória excessiva (2000MB)...");
            sistema.AdicionarThreadAoProcesso(processoAtualId, 2000.0f);

            sistema.ListarThreadsDoProcesso(processoAtualId);
        }

        Console.WriteLine("\n7. Finalizando processo e executando próximo...");
        if (processoAtualId > 0)
        {
            sistema.FinalizarProcesso(processoAtualId);
        }

        Console.WriteLine("\n8. Estado final do sistema:");
        sistema.ListarProcessos();
        sistema.MostrarStatusCPU();

        Console.WriteLine("\n9. Estatísticas de Memória e TLB:");
        sistema.MostrarEstatisticasMemoria();

        Console.WriteLine("\nDemonstração concluída!");
        Console.WriteLine("O sistema demonstrou:");
        Console.WriteLine("- Escalonamento FCFS");
        Console.WriteLine("- Gerenciamento de memória com paginação");
        Console.WriteLine("- TLB (Translation Lookaside Buffer)");
        Console.WriteLine("- Estatísticas de acesso à memória");
        Console.WriteLine("- Validação de limites de memória");
        Console.WriteLine("- Liberação automática de memória");
    }

    static void SimularAcessoMemoria(SistemaOperacional sistema)
    {
        Console.WriteLine("=== SIMULAÇÃO DE ACESSO À MEMÓRIA ===");
        
        sistema.ListarProcessos();
        
        Console.Write("Digite o ID do processo: ");
        if (!int.TryParse(Console.ReadLine(), out int processoId))
        {
            Console.WriteLine("ID inválido!");
            return;
        }

        var processo = sistema.ObterProcessoPorId(processoId);
        if (processo == null)
        {
            Console.WriteLine($"Processo com ID {processoId} não encontrado!");
            return;
        }

        Console.WriteLine($"\nProcesso selecionado: {processo.Nome} (ID: {processo.Id})");
        Console.WriteLine($"Páginas alocadas: {processo.TabelaDePaginas.TotalPaginas()}");
        Console.WriteLine();

        Console.Write("Digite o número de acessos aleatórios a simular: ");
        if (!int.TryParse(Console.ReadLine(), out int numAcessos) || numAcessos <= 0)
        {
            Console.WriteLine("Número inválido! Usando padrão de 10 acessos.");
            numAcessos = 10;
        }

        int totalPaginas = processo.TabelaDePaginas.TotalPaginas();

        Console.WriteLine($"\nSimulando {numAcessos} acessos aleatórios...\n");

        for (int i = 0; i < numAcessos; i++)
        {
            int paginaLogica = AleatorioSistema.Next(0, totalPaginas + 5);
            sistema.SimularAcessoMemoria(processoId, paginaLogica);
        }

        Console.WriteLine();
        processo.TabelaDePaginas.MostrarEstatisticas();
    }

    static void VisualizarLog()
    {
        Console.WriteLine("=== VISUALIZAR LOG DA SIMULAÇÃO ===");
        Logger.Ler();
        Console.WriteLine();
    }

    static void CriarArquivo(SistemaOperacional sistema)
    {
        Console.WriteLine("=== CRIAR ARQUIVO ===");
        sistema.ListarDiretorioRaiz();

        Console.Write("Nome do arquivo (ex: dados.txt): ");
        string nome = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(nome))
        {
            Console.WriteLine("Nome inválido!");
            return;
        }

        Console.Write("Conteúdo do arquivo: ");
        string conteudo = Console.ReadLine() ?? string.Empty;

        Console.Write("ID do processo proprietário (0 = nenhum): ");
        int.TryParse(Console.ReadLine(), out int processoId);

        sistema.CriarArquivo(nome, conteudo, processoId);
    }

    static void LerArquivo(SistemaOperacional sistema)
    {
        Console.WriteLine("=== LER ARQUIVO ===");
        sistema.ListarDiretorioRaiz();

        Console.Write("Nome do arquivo a ler: ");
        string nome = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(nome))
        {
            Console.WriteLine("Nome inválido!");
            return;
        }

        Console.Write("ID do processo leitor (0 = nenhum): ");
        int.TryParse(Console.ReadLine(), out int processoId);

        string conteudo = sistema.LerArquivo(nome, processoId);
        if (conteudo != null)
        {
            Console.WriteLine($"\n--- Conteúdo de '{nome}' ---");
            Console.WriteLine(conteudo.Length == 0 ? "(arquivo vazio)" : conteudo);
            Console.WriteLine("----------------------------");
        }
    }

    static void EscreverArquivo(SistemaOperacional sistema)
    {
        Console.WriteLine("=== ESCREVER NO ARQUIVO ===");
        sistema.ListarDiretorioRaiz();

        Console.Write("Nome do arquivo a escrever: ");
        string nome = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(nome))
        {
            Console.WriteLine("Nome inválido!");
            return;
        }

        Console.Write("Novo conteúdo (sobrescreve o anterior): ");
        string conteudo = Console.ReadLine() ?? string.Empty;

        sistema.EscreverArquivo(nome, conteudo);
    }

    static void DeletarArquivo(SistemaOperacional sistema)
    {
        Console.WriteLine("=== DELETAR ARQUIVO / DIRETÓRIO ===");
        sistema.ListarDiretorioRaiz();

        Console.Write("Nome do arquivo ou diretório a deletar: ");
        string nome = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(nome))
        {
            Console.WriteLine("Nome inválido!");
            return;
        }

        sistema.DeletarArquivo(nome);
    }

    static void CriarDiretorio(SistemaOperacional sistema)
    {
        Console.WriteLine("=== CRIAR DIRETÓRIO ===");

        Console.Write("Nome do diretório: ");
        string nome = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(nome))
        {
            Console.WriteLine("Nome inválido!");
            return;
        }

        sistema.CriarDiretorio(nome);
    }

    static void SuspenderProcesso(SistemaOperacional sistema)
    {
        Console.WriteLine("=== SUSPENDER PROCESSO ===");
        Console.WriteLine("O processo é removido da memória física e vai para o disco virtual.");
        Console.WriteLine("Estados: Pronto/Executando → ProntoSuspenso | Bloqueado → EsperaSuspensa");
        Console.WriteLine();
        sistema.ListarProcessos();

        Console.Write("ID do processo a suspender: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            sistema.SuspenderProcesso(id);
        }
        else
        {
            Console.WriteLine("ID inválido!");
        }
    }

    static void ReativarProcessoSuspenso(SistemaOperacional sistema)
    {
        Console.WriteLine("=== REATIVAR PROCESSO SUSPENSO ===");
        Console.WriteLine("O processo volta do disco para a memória física e entra na fila de prontos.");
        Console.WriteLine();
        sistema.ListarProcessos();

        Console.Write("ID do processo a reativar: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            sistema.ReativarProcessoSuspenso(id);
        }
        else
        {
            Console.WriteLine("ID inválido!");
        }
    }

    static void MostrarMetricasFinais(SistemaOperacional sistema)
    {
        Console.Clear();
        Console.WriteLine("╔══════════════════════════════════════════════╗");
        Console.WriteLine("║        ESTATÍSTICAS FINAIS DA SIMULAÇÃO      ║");
        Console.WriteLine("╚══════════════════════════════════════════════╝");

        Console.WriteLine($"\nSeed utilizada: {AleatorioSistema.GetSeedAtual()}");
        Console.WriteLine("(Use a mesma seed para reproduzir esta simulação)\n");

        DateTime inicioSimulacao = sistema.GetDataInicio();
        DateTime fimSimulacao = DateTime.Now;
        TimeSpan tempoTotalSimulacao = fimSimulacao - inicioSimulacao;

        var processosFinalizados = sistema.GetProcessosFinalizados();

        Logger.Registrar("=== ESTATÍSTICAS FINAIS ===");
        Logger.Registrar($"Tempo Total de Simulação: {tempoTotalSimulacao.TotalSeconds:F2}s");
        Logger.Registrar($"Processos Finalizados: {processosFinalizados.Count}");

        if (processosFinalizados.Count == 0)
        {
            Console.WriteLine("\nNenhum processo foi finalizado. Não há estatísticas para mostrar.");
            return;
        }

        Console.WriteLine("\n=== MÉTRICAS GERAIS DO SISTEMA ===");
        Console.WriteLine($"Tempo Total de Simulação: {tempoTotalSimulacao.TotalSeconds:F2} segundos");

        double throughput = processosFinalizados.Count / tempoTotalSimulacao.TotalSeconds;
        Logger.Registrar($"Throughput: {throughput:F4} processos/segundo");
        Console.WriteLine($"Throughput do Sistema: {throughput:F4} processos/segundo ({processosFinalizados.Count} processos em {tempoTotalSimulacao.TotalSeconds:F2}s)");

        double tempoTotalCPUUtilMs = sistema.GetTempoTotalCPUUtilMs();
        double tempoTotalOverheadMs = sistema.GetTempoTotalOverheadMs();
        double tempoTotalCPUOcupadaMs = tempoTotalCPUUtilMs + tempoTotalOverheadMs;
        
        double utilizacaoCPU = (tempoTotalCPUOcupadaMs / tempoTotalSimulacao.TotalMilliseconds) * 100.0;
        double utilizacaoCPUUtil = (tempoTotalCPUUtilMs / tempoTotalSimulacao.TotalMilliseconds) * 100.0;
        
        Logger.Registrar($"Utilização CPU: {utilizacaoCPU:F2}% (Útil: {utilizacaoCPUUtil:F2}%)");
        Logger.Registrar($"Trocas de Contexto: {sistema.NumeroTrocasContexto}");
        Logger.Registrar($"Overhead Total: {tempoTotalOverheadMs:F0}ms");
        
        Console.WriteLine($"Utilização total da CPU: {utilizacaoCPU:F2}% (Útil: {utilizacaoCPUUtil:F2}%)");
        Console.WriteLine($"Tempo de CPU Útil: {tempoTotalCPUUtilMs:F0}ms");

        int trocas = sistema.NumeroTrocasContexto;
        Console.WriteLine($"Número de Trocas de Contexto: {trocas}");

        int sobrecargaUnitaria = sistema.GetTempoSobrecarga();
        double percentualSobrecarga = (tempoTotalCPUOcupadaMs > 0) ? (tempoTotalOverheadMs / tempoTotalCPUOcupadaMs) * 100.0 : 0;
        Console.WriteLine($"Sobrecarga (Overhead): {tempoTotalOverheadMs:F0}ms ({percentualSobrecarga:F2}% do tempo total de CPU)");

        Console.WriteLine("\n=== MÉTRICAS INDIVIDUAIS (Médias) ===");

        double medioTurnaround = processosFinalizados.Average(p => ((TimeSpan)(p.TempoFinalizacao - p.TempoChegada)).TotalMilliseconds);
        Logger.Registrar($"Tempo Médio de Retorno: {medioTurnaround:F2}ms");
        Console.WriteLine($"Tempo Médio de Retorno: {medioTurnaround:F2} ms");

        double medioEspera = processosFinalizados.Average(p => p.TempoDeEspera.TotalMilliseconds);
        Logger.Registrar($"Tempo Médio de Espera: {medioEspera:F2}ms");
        Console.WriteLine($"Tempo Médio de Espera (Pronto): {medioEspera:F2} ms");

        double medioResposta = processosFinalizados
            .Where(p => p.TempoPrimeiraExecucao != null)
            .Average(p => ((TimeSpan)(p.TempoPrimeiraExecucao - p.TempoChegada)).TotalMilliseconds);
        Logger.Registrar($"Tempo Médio de Resposta: {medioResposta:F2}ms");
        Console.WriteLine($"Tempo Médio de Resposta: {medioResposta:F2} ms");

        Console.WriteLine("\n--- Detalhes por Processo ---");
        Console.WriteLine($"{"ID",-4} | {"Nome",-15} | {"Turnaround (ms)",-17} | {"Espera (ms)",-14} | {"Resposta (ms)",-15}");
        Console.WriteLine(new string('-', 70));

        foreach (var p in processosFinalizados.OrderBy(p => p.Id))
        {
            double turnaround = ((TimeSpan)(p.TempoFinalizacao - p.TempoChegada)).TotalMilliseconds;
            double espera = p.TempoDeEspera.TotalMilliseconds;
            double resposta = p.TempoPrimeiraExecucao.HasValue ? ((TimeSpan)(p.TempoPrimeiraExecucao - p.TempoChegada)).TotalMilliseconds : -1;

            Console.WriteLine($"{p.Id,-4} | {p.Nome,-15} | {turnaround,-17:F0} | {espera,-14:F0} | {resposta,-15:F0}");
        }
    }

}