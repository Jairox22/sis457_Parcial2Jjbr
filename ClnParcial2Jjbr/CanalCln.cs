using CadParcial2Jjbr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClnParcial2Jjbr
{
    public class CanalCln
    {
        public static List<Canal> listar()
        {
            using (var context = new Parcial2JjbrEntities())
            {
                return context.Canal.Where(x => x.estadoRegistro == 1).OrderBy(x => x.nombre).ToList();
            }
        }
    }
}
