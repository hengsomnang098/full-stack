namespace FullStack.Application.Interfaces;

public interface ICurrentUserService
{
    /// <summary>Business employee id from the "employee_id" claim (null for users not linked to an employee).</summary>
    int? EmployeeId { get; }
}
