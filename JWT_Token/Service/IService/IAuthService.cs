using JWT_Token.Model;

namespace JWT_Token
{
    //interface helps to achieve loose coupling and dependency injection between the controllers and the services
    //. It defines a contract that the service must adhere to, allowing for flexibility and easier testing.

    // it also helps to achieve abstration by hiding the implementation details of the service from the controllers.
    // The controllers only need to know about the interface and its methods, not how they are implemented.



    // here we can follow interface segregation principle
    public interface IAuthService
    {
        Task<string> Register(User user);

        Task<string> Login(LoginDto loginDto);


    }



}
