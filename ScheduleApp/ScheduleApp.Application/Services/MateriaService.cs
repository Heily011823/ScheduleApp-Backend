using ScheduleApp.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleApp.Application.Services
{
    public class MateriaService : IMateriaService
    {
        private readonly IMateriaRepository _materiaRepository;

        public MateriaService(IMateriaRepository materiaRepository)
        {
            _materiaRepository = materiaRepository;
        }

        public async Task<bool> EliminarMateriaAsync(int id)
        {
            var materia = await _materiaRepository.GetByIdAsync(id);

            if (materia == null)
                throw new Exception("Materia no encontrada");

            if (!materia.Activo)
                throw new Exception("La materia ya fue eliminada previamente");

            materia.Activo = false;

            await _materiaRepository.UpdateAsync(materia);

            return true;
        }
    }
}
