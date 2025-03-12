namespace ProjectManagementSystem.Common.Exceptions;

public class EntityNotFoundException : BaseException
{
    public EntityNotFoundException(string id)
        : base($"Entity with ID: {id} was not found.", 404)
    {
    }
}