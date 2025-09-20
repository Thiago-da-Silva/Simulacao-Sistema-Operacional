using Sistema_Operacional;

public class Program
{
    static void Main(string[] args)
    {
        SistemaOperacional sistema = new SistemaOperacional(10);
        Console.WriteLine("Sistema Operacional Iniciado com Escalonador FCFS (First Come First Served).");
        Console.WriteLine("=========================================================================\n");

        bool continuar = true;
        
        while (continuar)
        {
            MostrarMenu();
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
                    ExecutarDemo(sistema);
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

    static void MostrarMenu()
    {
        Console.WriteLine("=== MENU DO SISTEMA OPERACIONAL ===");
        Console.WriteLine("1  - Criar Processo");
        Console.WriteLine("2  - Executar Próximo Processo (FCFS)");
        Console.WriteLine("3  - Finalizar Processo");
        Console.WriteLine("4  - Pausar Processo");
        Console.WriteLine("5  - Retomar Processo");
        Console.WriteLine();
        Console.WriteLine("6  - Adicionar Thread a Processo");
        Console.WriteLine("7  - Listar Threads de Processo");
        Console.WriteLine("8  - Finalizar Thread");
        Console.WriteLine("9  - Pausar Thread");
        Console.WriteLine("10 - Retomar Thread");
        Console.WriteLine();
        Console.WriteLine("11 - Listar Todos os Processos");
        Console.WriteLine("12 - Mostrar Fila FCFS");
        Console.WriteLine("13 - Mostrar Status da CPU");
        Console.WriteLine("14 - Executar Demonstração");
        Console.WriteLine();
        Console.WriteLine("0  - Sair");
        Console.WriteLine();
        Console.Write("Escolha uma opção: ");
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
        
        sistema.CriarProcesso(nome);
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
        Console.Write("Digite o ID do processo para adicionar thread: ");
        if (int.TryParse(Console.ReadLine(), out int processoId))
        {
            sistema.AdicionarThreadAoProcesso(processoId);
        }
        else
        {
            Console.WriteLine("ID inválido!");
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

    static void ExecutarDemo(SistemaOperacional sistema)
    {
        Console.WriteLine("=== DEMONSTRAÇÃO DO ESCALONADOR FCFS ===\n");

        // Criando processos com pequenos intervalos para mostrar ordem de chegada
        Console.WriteLine("1. Criando processos...");
        sistema.CriarProcesso("Processo A");
        System.Threading.Thread.Sleep(10);
        sistema.CriarProcesso("Processo B");
        System.Threading.Thread.Sleep(10);
        sistema.CriarProcesso("Processo C");
        System.Threading.Thread.Sleep(10);
        sistema.CriarProcesso("Processo D");

        Console.WriteLine("\n2. Visualizando fila de processos (ordem FCFS):");
        sistema.ListarFilaProcessos();

        Console.WriteLine("3. Status inicial da CPU:");
        sistema.MostrarStatusCPU();

        Console.WriteLine("4. Executando processos em ordem FCFS...\n");
        
        // Executa o primeiro processo
        sistema.ExecutarProximoProcesso();
        sistema.MostrarStatusCPU();
        sistema.ListarFilaProcessos();

        Console.WriteLine("5. Adicionando threads ao processo em execução...");
        var processoAtual = sistema.ObterProcessoPorId(sistema.GetProcessoEmExecucaoId());
        if (processoAtual != null)
        {
            sistema.AdicionarThreadAoProcesso(processoAtual.Id);
            sistema.AdicionarThreadAoProcesso(processoAtual.Id);
            sistema.ListarThreadsDoProcesso(processoAtual.Id);
        }

        Console.WriteLine("\n6. Finalizando processo atual e executando próximo automaticamente...");
        if (processoAtual != null)
        {
            sistema.FinalizarProcesso(processoAtual.Id);
        }
        sistema.MostrarStatusCPU();
        sistema.ListarFilaProcessos();

        Console.WriteLine("7. Lista completa de processos:");
        sistema.ListarProcessos();

        Console.WriteLine("\nDemonstração concluída!");
    }
}