namespace JWT_Token.Model
{
    public class User
    {

        //int 4 bytes
        //long 8 bytes


        public long Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public string Role { get; set; }    
    }
}
