using Sistema_Operacional;

public class Program
{
    static void Main(string[] args)
    {
        SistemaOperacional sistema = new SistemaOperacional(10);
        Console.WriteLine("Sistema Operacional Iniciado com Escalonador FCFS (First Come First Served).");
        Console.WriteLine("=========================================================================\n");

        // Demonstração do escalonador FCFS
        Console.WriteLine("=== DEMONSTRAÇÃO DO ESCALONADOR FCFS ===\n");

        // Criando processos com pequenos intervalos para mostrar ordem de chegada
        Console.WriteLine("1. Criando processos...");
        sistema.CriarProcesso("Processo A");
        System.Threading.Thread.Sleep(10); // Pequeno delay para garantir ordem de chegada
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

        Console.WriteLine("5. Finalizando processo atual e executando próximo automaticamente...");
        sistema.FinalizarProcesso(1);
        sistema.MostrarStatusCPU();
        sistema.ListarFilaProcessos();

        Console.WriteLine("6. Pausando processo atual...");
        sistema.PausarProcesso(2);
        sistema.MostrarStatusCPU();
        sistema.ListarFilaProcessos();

        Console.WriteLine("7. Retomando processo pausado (vai para final da fila)...");
        sistema.RetomarProcesso(2);
        sistema.ListarFilaProcessos();

        Console.WriteLine("8. Lista completa de processos:");
        sistema.ListarProcessos();

        Console.WriteLine("\n=== COMANDOS DISPONÍVEIS ===");
        Console.WriteLine("- ExecutarProximoProcesso(): Executa o próximo processo da fila FCFS");
        Console.WriteLine("- FinalizarProcesso(id): Finaliza um processo e executa o próximo automaticamente");
        Console.WriteLine("- PausarProcesso(id): Pausa um processo e executa o próximo");
        Console.WriteLine("- RetomarProcesso(id): Retoma um processo pausado (vai para final da fila)");
        Console.WriteLine("- ListarFilaProcessos(): Mostra a fila FCFS atual");
        Console.WriteLine("- MostrarStatusCPU(): Mostra status da CPU e fila");
    }
}