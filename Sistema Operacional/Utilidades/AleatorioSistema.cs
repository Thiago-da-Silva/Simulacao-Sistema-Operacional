using System;

namespace Sistema_Operacional.Utilidades
{
    // Gerador de números aleatórios centralizado para garantir determinismo
    public static class AleatorioSistema
    {
        private static Random _random;
        private static int _seedAtual;
        private static bool _inicializado = false;

        public static void Inicializar(int seed)
        {
            _seedAtual = seed;
            _random = new Random(seed);
            _inicializado = true;
        }

        public static int Next(int min, int max)
        {
            if (!_inicializado)
            {
                throw new InvalidOperationException(
                    "AleatorioSistema não foi inicializado. Chame Inicializar(seed) antes de usar.");
            }

            return _random.Next(min, max);
        }

        public static int Next(int maxValue)
        {
            if (!_inicializado)
            {
                throw new InvalidOperationException(
                    "AleatorioSistema não foi inicializado. Chame Inicializar(seed) antes de usar.");
            }

            return _random.Next(maxValue);
        }

        public static int GetSeedAtual()
        {
            return _seedAtual;
        }

        public static bool EstaInicializado()
        {
            return _inicializado;
        }
    }
}
