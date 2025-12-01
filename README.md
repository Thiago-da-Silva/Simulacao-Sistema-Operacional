# 🖥️ Simulador de Sistema Operacional

## 📖 Visão Geral
O Simulador é uma ferramenta de simulação de alto nível desenvolvida em C# (.NET) que emula os componentes centrais de um sistema operacional moderno. O projeto foca na visualização e compreensão de algoritmos de escalonamento de CPU, gerenciamento de memória via paginação e o ciclo de vida de processos e threads.

O sistema opera via console interativo, permitindo configuração granular de parâmetros de hardware simulado (como tamanho de página e quantum) e oferece logs detalhados para análise de métricas.

---

## 🚀 Funcionalidades Principais

### 1. Gerenciamento de Processos e Threads
O simulador implementa uma arquitetura robusta de PCB (Process Control Block) e TCB (Thread Control Block):
- **Multithreading:** Suporte a múltiplas threads por processo, compartilhando o espaço de endereçamento do pai.
- **Contexto de Hardware:** Simulação de registradores de propósito geral (`AX`, `BX`, `CX`, `DX`) e `Program Counter` (PC) por processo.
- **Pilha Lógica:** Cada thread possui sua própria pilha de execução simulada.
- **Ciclo de Vida Completo:** Transições de estado geridas automaticamente (`Criado` → `Pronto` → `Executando` → `Bloqueado` → `Finalizado`).

### 2. Algoritmos de Escalonamento
O núcleo do sistema permite a injeção de dependência de diferentes estratégias de escalonamento (`IEscalonador`):
- **FCFS (First-Come, First-Served):** Execução não-preemptiva baseada na ordem de chegada.
- **Round Robin:** Execução preemptiva com **Quantum** configurável.
- **Prioridades:** Escalonamento não-preemptivo com reordenação dinâmica da fila de prontos.
- **Troca de Contexto:** Simulação realista de *overhead* (tempo de sobrecarga) ao alternar entre processos.

### 3. Gerenciamento de Memória (Paginação)
Implementação de um MMU (Memory Management Unit) simulado:
- **Paginação:** Divisão da memória física em *Frames* (Molduras) e memória lógica em *Páginas*.
- **TLB (Translation Lookaside Buffer):** Simulação de cache de endereçamento para otimização de tradução, com métricas de *Hits* e *Misses*.
- **Tabela de Páginas:** Mapeamento individual por processo.
- **Proteção de Memória:** Validação de limites e simulação de *Page Faults* ao acessar endereços inválidos.
- **Alocação Dinâmica:** Algoritmo *First-Fit* para busca de quadros livres.

### 4. Métricas e Logs
- **Estatísticas Finais:** Cálculo automático de *Throughput*, *Turnaround Time*, Tempo de Espera e Taxa de Utilização da CPU/Memória.

---

## 🛠️ Arquitetura do Projeto

A solução segue os princípios de Orientação a Objetos e SOLID:

* **Kernel (`SistemaOperacional`):** Atua como a fachada controladora, orquestrando chamadas entre memória, CPU e processos.
* **Camada de Modelo (`Modelos`):** Define as estruturas de dados (`Processo`, `Thread`, `Registradores`).
* **Camada de Memória (`Memoria`):** Encapsula a lógica de hardware físico (`GerenciadorMemoria`) e lógico (`TabelaPaginas`, `TLB`).
* **Camada de Escalonamento (`Escalonamento`):** Implementa a estratégia de seleção de processos através da interface `IEscalonador`.

---

## ⚙️ Como Executar

### Pré-requisitos
* .NET SDK 7.0 ou superior.

### Instalação e Execução
1. Clone o repositório:
   ```bash
   git clone [https://github.com/seu-usuario/simulacao-so.git](https://github.com/seu-usuario/simulacao-so.git)
## 📝 Diagrama de Classes  

```mermaid
classDiagram
    %% Classes Principais
    class Program {
        +Main()
        +MostrarMenu()
        +ExecutarDemo()
    }

    class SistemaOperacional {
        -GerenciadorMemoria memoria
        -IEscalonador escalonador
        -List~Processo~ processos
        -int tempoSobrecarga
        +CriarProcesso()
        +ExecutarProximoProcesso()
        +SimularAcessoMemoria()
        +AdicionarThreadAoProcesso()
    }

    %% Gerenciamento de Processos
    class Processo {
        +int Id
        +string Nome
        +Estados Estado
        +RegistradoresContexto ContextoCPU
        +TabelaPaginas TabelaDePaginas
        +List~Thread~ Threads
        +AdicionarThread()
        +CalcularMemoriaTotal()
    }

    class Thread {
        +int Id
        +float MemoriaUtilizada
        +Stack~string~ PilhaLogica
        +List~int~ PaginasLogicasAlocadas
        +Pausar()
        +Retomar()
    }

    class RegistradoresContexto {
        +int AX
        +int BX
        +int PC
    }

    class Estados {
        <<enumeration>>
        Criado
        Pronto
        Executando
        Bloqueado
        Finalizado
    }

    %% Escalonamento
    class IEscalonador {
        <<interface>>
        +AdicionarProcesso()
        +ObterProximoProcesso()
        +RemoverProcessoDaFila()
    }

    class EscalonadorFCFS {
        -Queue~Processo~ FilaFCFS
    }

    class EscalonadorRoundRobin {
        -Queue~Processo~ FilaDeProntos
        +int Quantum
    }

    class EscalonadorPrioridades {
        -List~Processo~ FilaDeProntos
    }

    %% Gerenciamento de Memória
    class GerenciadorMemoria {
        -FrameInfo[] MapaDeMolduras
        +int TamanhoPagina
        +AlocarPaginas()
        +LiberarPaginas()
    }

    class FrameInfo {
        +bool Ocupado
        +int ProcessoId
        +Alocar()
    }

    class TabelaPaginas {
        -Dictionary~int,int~ Mapeamento
        -TLB tlb
        +TraduzirEndereco()
        +RegistrarAlocacao()
    }

    class TLB {
        -Dictionary~int,int~ Cache
        +int TotalHits
        +int TotalMisses
        +TentarObter()
    }

    %% Utilitários
    class Logger {
        +Registrar()
        +Ler()
    }

    class AleatorioSistema {
        +Next()
        +Inicializar()
    }

    %% Relacionamentos
    Program ..> SistemaOperacional : Inicializa
    SistemaOperacional --> GerenciadorMemoria : Possui
    SistemaOperacional --> IEscalonador : Usa
    SistemaOperacional "1" *-- "*" Processo : Gerencia
    
    Processo *-- RegistradoresContexto : Possui PCB
    Processo *-- TabelaPaginas : Possui
    Processo "1" *-- "*" Thread : Contém
    
    Thread ..> Estados : Usa
    Processo ..> Estados : Usa
    
    IEscalonador <|.. EscalonadorFCFS : Implementa
    IEscalonador <|.. EscalonadorRoundRobin : Implementa
    IEscalonador <|.. EscalonadorPrioridades : Implementa
    
    GerenciadorMemoria *-- FrameInfo : Mapeia Frames
    TabelaPaginas *-- TLB : Usa Cache
    
    SistemaOperacional ..> Logger : Registra Logs
    SistemaOperacional ..> AleatorioSistema : Usa Random
