using Common.DbModels;
using Common.Models.BusinessModels;

namespace Common.Abstractions.Repositories
{
    public interface ITimetableRepository
    {
        DbTimetable CreateTimetable(Timetable timetable);
    }
}