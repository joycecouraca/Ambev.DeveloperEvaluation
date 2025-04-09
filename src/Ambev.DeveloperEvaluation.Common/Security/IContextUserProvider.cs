namespace Ambev.DeveloperEvaluation.Common.Security;

public interface IContextUserProvider
{
    IUser GetCurrentUser();
}
