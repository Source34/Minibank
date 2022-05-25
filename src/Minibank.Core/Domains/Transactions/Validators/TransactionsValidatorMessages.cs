namespace Minibank.Core.Domains.Transactions.Validators
{
    public class TransactionsValidatorMessages
    {
        public const string InvalidAmount = "Некорректная сумма!";
        public const string FromAccountError = "Неверный аккаунт отправителя!";
        public const string ToAccountError = "Неверный аккаунт получателя!";
        public const string FromAccountNotActive = "Аккаунт отправителя закрыт!";
        public const string ToAccountNotActive = "Аккаунт получателя закрыт!";
        public const string EqualsBankAccountIds = "Одиннаковые Id аккаунтов!";
        public const string NotEnoughMoney = "Недостаточно средств!";
    }
}
