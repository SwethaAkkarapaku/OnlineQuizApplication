namespace QuizMVC.DTO
{
    public class GetAccountData
    {
        public List<AccountInfoDTO> data { get; set; } = new List<AccountInfoDTO>();
    }
    public class AccountInfoDTO
    {
        public Guid accountId { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string role { get; set; }
    }
    public class LoginUser
    {
        public string email { get; set; }
        public string password { get; set; }
    }
}
