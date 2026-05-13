using ScheduleApp.Application.Interfaces;

namespace ScheduleApp.Application.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;

        public SubjectService(ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        public async Task<bool> DeleteSubjectAsync(Guid id)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);

            if (subject == null)
                throw new Exception("Subject not found");

            if (!subject.IsActive)
                throw new Exception("The subject has already been deleted");

            subject.IsActive = false;

            await _subjectRepository.UpdateAsync(subject);

            return true;
        }

        public Task<bool> DeleteSubjectAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}