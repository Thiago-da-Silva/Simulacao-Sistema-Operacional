using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Operacional.SistemaArquivos
{
    public class Arquivo
    {
        public string Nome { get; set; }
        public string Extensao { get; set; }
        public int TamanhoBytes { get; set; }
        public int BlocoInicial { get; set; } // Referência onde o conteúdo começa
        public Arquivo DiretorioPai { get; set; } // Para hierarquia
        public bool EhDiretorio { get; set; }
        public List<Arquivo> Filhos { get; set; } = new List<Arquivo>();

        public Arquivo(string nome, bool ehDiretorio = false)
        {
            Nome = nome;
            EhDiretorio = ehDiretorio;
        }
    }
}