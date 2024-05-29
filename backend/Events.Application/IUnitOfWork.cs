namespace Events.Application;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}