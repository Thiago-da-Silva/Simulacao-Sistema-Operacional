using Sistema_Operacional;
using Sistema_Operacional.Interface;

public class Program
{
    static void Main(string[] args)
    {
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

        Console.Clear();

        SistemaOperacional sistema = new SistemaOperacional(1024, escalonador); // 1024MB de memória
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
                    MostrarInformacoesSistema(sistema);
                    break;
                case "16":
                    ExecutarDemo(sistema);
                    break;
                case "99":
                    Console.Clear();
                    Console.WriteLine("Tela limpa!");
                    break;
                case "0":
                    continuar = false;
                    Console.WriteLine("Encerrando Sistema Operacional...");
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
        Console.WriteLine("║ 2  - Executar Próximo Processo               ║");
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

        sistema.CriarProcesso(nome, prioridade);

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
        Console.WriteLine($"Memória atual do processo: {processo.MemoriaUtilizada:F2}MB");
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

    static void MostrarInformacoesSistema(SistemaOperacional sistema)
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

        string statusCpu = sistema.IsCpuEmUso() ? "EM USO" : "LIVRE";
        Console.WriteLine($"║ Status da CPU: {statusCpu}".PadRight(55) + "║");

        if (sistema.IsCpuEmUso())
        {
            int processoAtualId = sistema.GetProcessoEmExecucaoId();
            var processoAtual = sistema.ObterProcessoPorId(processoAtualId);
            if (processoAtual != null)
            {
                Console.WriteLine($"║ Processo Executando: {processoAtual.Nome} (ID: {processoAtual.Id})".PadRight(55) + "║");
                Console.WriteLine($"║ Memória do Processo: {processoAtual.MemoriaUtilizada:F2}MB".PadRight(55) + "║");
                Console.WriteLine($"║ Threads: {processoAtual.Threads.Count}".PadRight(47) + "║");
            }
        }

        Console.WriteLine($"║ Sistema iniciado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}".PadRight(47) + "║");
        Console.WriteLine("║                                              ║");
        Console.WriteLine("║ Algoritmo de Escalonamento: FCFS            ║");
        Console.WriteLine("║    (First Come First Served)                ║");
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

        Console.WriteLine("\nDemonstração concluída!");
        Console.WriteLine("O sistema demonstrou:");
        Console.WriteLine("- Escalonamento FCFS");
        Console.WriteLine("- Gerenciamento de memória");
        Console.WriteLine("- Validação de limites de memória");
        Console.WriteLine("- Liberação automática de memória");
    }
}