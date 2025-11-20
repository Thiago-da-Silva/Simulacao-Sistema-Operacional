using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Operacional.Memoria
{
    public class GerenciadorMemoria
    {
        public int TamanhoPagina { get; private set; } // Em MB
        public int TotalMolduras { get; private set; }
        public int MemoriaTotal { get; private set; } // Em MB

        // Este é o "mapa de molduras" físico
        private FrameInfo[] MapaDeMolduras;

        public int TotalAlocacoes { get; private set; } = 0;
        public int TotalLiberacoes { get; private set; } = 0;
        public int TotalFalhasAlocacao { get; private set; } = 0;

        public GerenciadorMemoria(int memoriaTotalMB, int tamanhoPaginaMB)
        {
            if (tamanhoPaginaMB <= 0) tamanhoPaginaMB = 4; // Padrão
            if (memoriaTotalMB <= 0) memoriaTotalMB = 1024; // Padrão

            MemoriaTotal = memoriaTotalMB;
            TamanhoPagina = tamanhoPaginaMB;
            TotalMolduras = memoriaTotalMB / tamanhoPaginaMB;

            MapaDeMolduras = new FrameInfo[TotalMolduras];
            for (int i = 0; i < TotalMolduras; i++)
            {
                MapaDeMolduras[i] = new FrameInfo();
            }

            Console.WriteLine($"Gerenciador de Memória iniciado: {MemoriaTotal}MB Total, {TotalMolduras} molduras de {TamanhoPagina}MB cada.");
        }

        // Aloca 'N' páginas usando a política First-Fit. Lista de índices de frames alocados, ou null se falhar
        public List<int> AlocarPaginas(int processoId, int paginasNecessarias)
        {
            if (paginasNecessarias > GetMoldurasDisponiveis())
            {
                TotalFalhasAlocacao++;
                return null; // Memória insuficiente
            }

            var framesAlocados = new List<int>();
            int paginaLogicaId = 0; // Isso será gerenciado pela Tabela de Páginas

            // Política First-Fit: Encontra os N primeiros frames livres
            for (int i = 0; i < TotalMolduras; i++)
            {
                if (!MapaDeMolduras[i].Ocupado)
                {
                    framesAlocados.Add(i);
                    if (framesAlocados.Count == paginasNecessarias)
                        break; // Encontrou todos
                }
            }

            // Se não encontrou o suficiente (improvável se verificamos antes, mas bom para concorrência)
            if (framesAlocados.Count < paginasNecessarias)
            {
                TotalFalhasAlocacao++;
                return null;
            }

            // Marca os frames como ocupados
            foreach (var frameIndex in framesAlocados)
            {
                MapaDeMolduras[frameIndex].Alocar(processoId, paginaLogicaId++); // O PaginaLogicaId aqui é só ilustrativo
            }

            TotalAlocacoes++;
            return framesAlocados;
        }

        // Libera uma lista específica de molduras de página
        public void LiberarPaginas(List<int> indicesFrames)
        {
            foreach (var frameIndex in indicesFrames)
            {
                if (frameIndex >= 0 && frameIndex < TotalMolduras)
                {
                    MapaDeMolduras[frameIndex].Liberar();
                }
            }

            if (indicesFrames.Count > 0)
            {
                TotalLiberacoes++;
            }
        }

        public int GetMoldurasUsadas() => MapaDeMolduras.Count(f => f.Ocupado);
        public int GetMoldurasDisponiveis() => TotalMolduras - GetMoldurasUsadas();

        public float CalcularMemoriaUsada() => GetMoldurasUsadas() * TamanhoPagina;
        public float CalcularMemoriaDisponivel() => GetMoldurasDisponiveis() * TamanhoPagina;

        public void MostrarStatusMemoria()
        {
            float memoriaUsada = CalcularMemoriaUsada();
            float memoriaDisponivel = CalcularMemoriaDisponivel();
            float percentualUso = (memoriaUsada / MemoriaTotal) * 100;

            Console.WriteLine("=== STATUS DA MEMÓRIA (Paginação) ===");
            Console.WriteLine($"Memória Total: {MemoriaTotal}MB");
            Console.WriteLine($"Tamanho da Página: {TamanhoPagina}MB");
            Console.WriteLine($"Molduras: {GetMoldurasUsadas()} / {TotalMolduras} (Usadas/Total)");
            Console.WriteLine($"Memória Usada: {memoriaUsada:F2}MB ({percentualUso:F1}%)");
            Console.WriteLine($"Memória Disponível: {memoriaDisponivel:F2}MB");
            Console.WriteLine();
        }

        public void MostrarEstatisticasGerais()
        {
            Console.WriteLine("=== ESTATÍSTICAS GERAIS DE MEMÓRIA ===");
            Console.WriteLine($"Total de alocações bem-sucedidas: {TotalAlocacoes}");
            Console.WriteLine($"Total de liberações: {TotalLiberacoes}");
            Console.WriteLine($"Total de falhas de alocação: {TotalFalhasAlocacao}");
            
            float fragmentacaoInterna = 0;
            int moldurasUsadas = GetMoldurasUsadas();
            if (moldurasUsadas > 0)
            {
                fragmentacaoInterna = ((float)moldurasUsadas * TamanhoPagina - CalcularMemoriaUsada()) / MemoriaTotal * 100;
            }
            
            Console.WriteLine($"Fragmentação interna estimada: {fragmentacaoInterna:F2}%");
            Console.WriteLine($"Utilização de memória: {(CalcularMemoriaUsada() / MemoriaTotal * 100):F2}%");
            Console.WriteLine();
        }
    }
}
