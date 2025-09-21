# 🖥️ Simulador de Sistema Operacional  

## 📄 Relatório da Primeira Entrega  
**Integrantes do Grupo:**  
Henrique Akio Kuroda RA: 112886  
Thiago da Silva RA: 113483  
Rafael de Camargo RA: 114119  

---

## ✅ 1. Funcionalidades Implementadas  

Nesta primeira fase do projeto, foi desenvolvida a **estrutura central do simulador de Sistema Operacional**, com foco no gerenciamento de **processos**, **threads** e na implementação de um escalonador **FCFS**.  

As seguintes funcionalidades foram concluídas:  

### 🔹 Gerenciamento de Processos  
- Criação de processos com **nome** e **ID únicos**.  
- Manipulação de estados básicos: `Criado`, `Pronto`, `Executando`, `Bloqueado`, `Finalizado`.  
- Pausa (bloqueio) de processos em execução ou na fila.  
- Retomada de processos, reinserindo-os na fila de prontos.  
- Finalização de processos com liberação de memória e CPU.  

### 🔹 Gerenciamento de Threads  
- Adição de **threads** a um processo existente, com **alocação de memória** específica.  
- Pausa e retomada de threads individuais.  
- Finalização de threads com liberação da memória utilizada.  

### 🔹 Escalonador de CPU e Gerenciamento de Memória  
- Implementação do algoritmo **First-Come, First-Served (FCFS)** para gerenciamento da fila de prontos.  
- Modelo simplificado de gerenciamento de memória:  
  - Controle da memória total do sistema.  
  - Bloqueio de criação de novas threads quando não há espaço disponível.  

### 🔹 Interface e Demonstração  
- Operação via **menu interativo em linha de comando**.  
- Rotina de **demonstração automática (Opção 16)** que executa um fluxo de operações para validar as funcionalidades principais.  

---

## 📊 2. Percentual de Conclusão do Projeto  

Considerando o escopo definido na especificação (múltiplos algoritmos de escalonamento, gerenciamento de memória com paginação/segmentação, simulação de E/S, sistema de arquivos e coleta de métricas), o projeto encontra-se em aproximadamente **40% de conclusão**.  

A **base estrutural** já foi criada, mas os módulos mais complexos (memória avançada, E/S, arquivos) ainda não foram iniciados.  

---

## ⚠️ 3. Problemas e Limitações das Partes Entregues  

As funcionalidades implementadas operam corretamente dentro de seu escopo, mas apresentam limitações que precisam ser resolvidas:  

- **Gerenciamento de Memória Simplista:**  
  O modelo atual é básico, apenas controla a memória total e não implementa paginação ou segmentação.  

- **Escalonador Fixo:**  
  Atualmente apenas o algoritmo **FCFS** está implementado. Será necessário adicionar suporte a **Round Robin** e **Prioridades**.  

- **Falta de Simulação de Troca de Contexto:**  
  A transição entre processos ocorre de forma instantânea, sem simulação de **overhead**.  

- **PCB/TCB Incompletos:**  
  As classes `Processo` e `Thread` ainda não possuem todos os campos de um PCB/TCB real (registradores, contador de programa, pilha lógica etc.).  

---

## 🚀 Próximos Passos  

- Implementar **algoritmos adicionais de escalonamento** (Round Robin, Prioridades).  
- Aprimorar o **gerenciamento de memória** com paginação e segmentação.  
- Criar a **simulação de E/S** e de **troca de contexto** com overhead.  
- Completar as estruturas de **PCB/TCB**.  

---

## 📝 Diagrama de Classes  

```mermaid
classDiagram
    class Program {
        +Main(args: string[])
        +MostrarMenu()
        +ExecutarDemo()
    }

    class SistemaOperacional {
        -totalMemoria: int
        -cpuEmUso: bool
        -processoEmExecucaoId: int
        -processos: List<Processo>
        -escalonador: Escalonador
        +CriarProcesso(nome: string): Processo
        +ExecutarProximoProcesso()
        +FinalizarProcesso(id: int)
        +PausarProcesso(id: int)
        +RetomarProcesso(id: int)
        +AdicionarThread(idProcesso: int, memoria: float): bool
        +ListarProcessos(): List<Processo>
    }

    class Escalonador {
        -filaProntos: Queue<Processo>
        +AdicionarProcesso(processo: Processo)
        +ObterProximoProcesso(): Processo
        +RemoverProcesso(id: int): bool
    }

    class Processo {
        +nome: string
        +id: int
        +prioridade: int
        +estado: Estados
        +threads: List<Thread>
        +memoriaUtilizada: float
        +tempoChegada: DateTime
        +AdicionarThread(memoria: float): bool
        +FinalizarThread(id: int)
    }

    class Thread {
        +id: int
        +estado: Estados
        +memoriaUtilizada: float
        +processoPaiId: int
        +Pausar()
        +Retomar()
    }

    class Estados {
        <<enumeration>>
        Criado
        Pronto
        Executando
        Bloqueado
        Finalizado
    }

    Program ..> SistemaOperacional : usa
    SistemaOperacional o-- Escalonador : possui
    SistemaOperacional o-- "*" Processo : gerencia
    Escalonador --> Processo : escalona
    Processo *-- "*" Thread : contém
    Processo ..> Estados
    Thread ..> Estados
