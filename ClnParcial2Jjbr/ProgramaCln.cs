using CadParcial2Jjbr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClnParcial2Jjbr
{
    public class ProgramaCln
    {
        /// <summary>
        /// Inserta un nuevo programa usando el procedimiento almacenado
        /// </summary>
        public static int insertar(Programa programa)
        {
            try
            {
                using (var context = new Parcial2JjbrEntities())
                {
                    var resultado = context.sp_CrearPrograma(
                        programa.idCanal,
                        programa.titulo,
                        programa.descripcion,
                        programa.duracion,
                        programa.productor,
                        programa.fechaEstreno
                    ).FirstOrDefault();

                    return resultado?.id ?? 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al insertar programa: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Actualiza un programa existente usando el procedimiento almacenado
        /// </summary>
        public static int actualizar(Programa programa)
        {
            try
            {
                using (var context = new Parcial2JjbrEntities())
                {
                    context.sp_ActualizarPrograma(
                        programa.id,
                        programa.idCanal,
                        programa.titulo,
                        programa.descripcion,
                        programa.duracion,
                        programa.productor,
                        programa.fechaEstreno
                    );

                    return programa.id;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar programa: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Elimina un programa (eliminación lógica) usando el procedimiento almacenado
        /// </summary>
        public static int eliminar(int id)
        {
            try
            {
                using (var context = new Parcial2JjbrEntities())
                {
                    context.sp_EliminarPrograma(id);
                    return 1; // Éxito
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar programa: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lista todos los programas activos usando LINQ
        /// </summary>
        public static List<Programa> listar()
        {
            try
            {
                using (var context = new Parcial2JjbrEntities())
                {
                    return context.Programa
                        .Where(p => p.estado == 1)
                        .OrderByDescending(p => p.fechaEstreno)
                        .ThenBy(p => p.titulo)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al listar programas: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lista todos los programas usando el procedimiento almacenado (con JOIN a Canal)
        /// </summary>
        public static List<sp_ListarProgramas_Result> listarConSP()
        {
            try
            {
                using (var context = new Parcial2JjbrEntities())
                {
                    return context.sp_ListarProgramas().ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al listar programas con SP: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lista programas filtrados por canal
        /// </summary>
        public static List<sp_ListarProgramasPorCanal_Result> listarPorCanal(int idCanal)
        {
            try
            {
                using (var context = new Parcial2JjbrEntities())
                {
                    return context.sp_ListarProgramasPorCanal(idCanal).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al listar programas por canal: {ex.Message}", ex);
            }
        }


        /// <summary>
        /// Busca programas por título (búsqueda flexible)
        /// </summary>
        public static List<Programa> buscarPorTitulo(string titulo)
        {
            try
            {
                using (var context = new Parcial2JjbrEntities())
                {
                    return context.Programa
                        .Where(p => p.estado == 1 && p.titulo.Contains(titulo))
                        .OrderBy(p => p.titulo)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al buscar programas: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Valida que un programa tenga datos correctos antes de insertar/actualizar
        /// </summary>
        public static bool validar(Programa programa, out string mensaje)
        {
            mensaje = "";

            if (programa.idCanal <= 0)
            {
                mensaje = "Debe seleccionar un canal válido";
                return false;
            }

            if (string.IsNullOrWhiteSpace(programa.titulo))
            {
                mensaje = "El título es obligatorio";
                return false;
            }

            if (programa.titulo.Length > 100)
            {
                mensaje = "El título no puede exceder 100 caracteres";
                return false;
            }

            if (programa.descripcion != null && programa.descripcion.Length > 250)
            {
                mensaje = "La descripción no puede exceder 250 caracteres";
                return false;
            }

            if (programa.duracion <= 0)
            {
                mensaje = "La duración debe ser mayor a 0 minutos";
                return false;
            }

            if (programa.productor != null && programa.productor.Length > 100)
            {
                mensaje = "El nombre del productor no puede exceder 100 caracteres";
                return false;
            }

            if (programa.fechaEstreno == default(DateTime))
            {
                mensaje = "La fecha de estreno es obligatoria";
                return false;
            }

            return true;
        }
    }
}