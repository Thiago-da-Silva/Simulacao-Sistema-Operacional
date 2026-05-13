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

        // Este � o "mapa de molduras" f�sico
        private FrameInfo[] MapaDeMolduras;

        public int TotalAlocacoes { get; private set; } = 0;
        public int TotalLiberacoes { get; private set; } = 0;
        public int TotalFalhasAlocacao { get; private set; } = 0;

        public GerenciadorMemoria(int memoriaTotalMB, int tamanhoPaginaMB)
        {
            if (tamanhoPaginaMB <= 0) tamanhoPaginaMB = 4; // Padr�o
            if (memoriaTotalMB <= 0) memoriaTotalMB = 1024; // Padr�o

            MemoriaTotal = memoriaTotalMB;
            TamanhoPagina = tamanhoPaginaMB;
            TotalMolduras = memoriaTotalMB / tamanhoPaginaMB;

            MapaDeMolduras = new FrameInfo[TotalMolduras];
            for (int i = 0; i < TotalMolduras; i++)
            {
                MapaDeMolduras[i] = new FrameInfo();
            }

            Console.WriteLine($"Gerenciador de Mem�ria iniciado: {MemoriaTotal}MB Total, {TotalMolduras} molduras de {TamanhoPagina}MB cada.");
        }

        // Aloca 'N' p�ginas usando a pol�tica First-Fit. Lista de �ndices de frames alocados, ou null se falhar
        public List<int> AlocarPaginas(int processoId, int paginasNecessarias)
        {
            if (paginasNecessarias > GetMoldurasDisponiveis())
            {
                TotalFalhasAlocacao++;
                return null; // Mem�ria insuficiente
            }

            var framesAlocados = new List<int>();
            int paginaLogicaId = 0; // Isso ser� gerenciado pela Tabela de P�ginas

            // Pol�tica First-Fit: Encontra os N primeiros frames livres
            for (int i = 0; i < TotalMolduras; i++)
            {
                if (!MapaDeMolduras[i].Ocupado)
                {
                    framesAlocados.Add(i);
                    if (framesAlocados.Count == paginasNecessarias)
                        break; // Encontrou todos
                }
            }

            // Se n�o encontrou o suficiente (improv�vel se verificamos antes, mas bom para concorr�ncia)
            if (framesAlocados.Count < paginasNecessarias)
            {
                TotalFalhasAlocacao++;
                return null;
            }

            // Marca os frames como ocupados
            foreach (var frameIndex in framesAlocados)
            {
                MapaDeMolduras[frameIndex].Alocar(processoId, paginaLogicaId++); // O PaginaLogicaId aqui � s� ilustrativo
            }

            TotalAlocacoes++;
            return framesAlocados;
        }

        // Libera uma lista espec�fica de molduras de p�gina
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

            Console.WriteLine("=== STATUS DA MEM�RIA (Pagina��o) ===");
            Console.WriteLine($"Mem�ria Total: {MemoriaTotal}MB");
            Console.WriteLine($"Tamanho da P�gina: {TamanhoPagina}MB");
            Console.WriteLine($"Molduras: {GetMoldurasUsadas()} / {TotalMolduras} (Usadas/Total)");
            Console.WriteLine($"Mem�ria Usada: {memoriaUsada:F2}MB ({percentualUso:F1}%)");
            Console.WriteLine($"Mem�ria Dispon�vel: {memoriaDisponivel:F2}MB");
            Console.WriteLine();
        }

        // fragmentacaoInternaPercent é calculado externamente pelo SistemaOperacional,
        // que tem acesso à memória real de cada processo vs. páginas alocadas.
        public void MostrarEstatisticasGerais(float fragmentacaoInternaPercent = 0)
        {
            Console.WriteLine("=== ESTAT�STICAS GERAIS DE MEM�RIA ===");
            Console.WriteLine($"Total de aloca��es bem-sucedidas: {TotalAlocacoes}");
            Console.WriteLine($"Total de libera��es: {TotalLiberacoes}");
            Console.WriteLine($"Total de falhas de aloca��o: {TotalFalhasAlocacao}");
            Console.WriteLine($"Fragmenta��o interna estimada: {fragmentacaoInternaPercent:F2}%");
            Console.WriteLine($"Utiliza��o de mem�ria: {(CalcularMemoriaUsada() / MemoriaTotal * 100):F2}%");
            Console.WriteLine();
        }
    }
}
