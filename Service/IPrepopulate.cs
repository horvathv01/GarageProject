namespace GarageProject.Service;

public interface IPrepopulate
{
    Task PrepopulateDB();
    Task PrepopulateInMemory();

    Task ClearDb();
}