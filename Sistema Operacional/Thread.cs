using Sistema_Operacional.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Operacional
{
    public class Thread
    {
        public float MemoriaUtilizada {  get; set; }
        public Estados Estado { get; set; } = Estados.Criado;
        public int Id {  get; set; }

        public Thread(float memoriaUtilizada, int id)
        {
            MemoriaUtilizada = memoriaUtilizada;
            Id = id;
        }

        public void PausarThread()
        {
            if (this.Estado == Estados.Executando || this.Estado == Estados.Pronto)
            {
                this.Estado = Estados.Bloqueado;
                Console.WriteLine($"Thread com ID {this.Id} pausada.");
            }
            else
            {
                Console.WriteLine($"Thread com ID {this.Id} não está em um estado que permite pausa.");
            }
        }

        public void RetomarThread()
        {
            if (this.Estado == Estados.Bloqueado)
            {
                this.Estado = Estados.Pronto;
                Console.WriteLine($"Thread com ID {this.Id} retomada.");
            }
            else
            {
                Console.WriteLine($"Thread com ID {this.Id} não está em um estado que permite retomada.");
            }
        }
    }
}
