namespace ISTEGY.Sales.Models
{
    public enum TransactionType
    {
        // Transactions which increase items in a store -- BUYING FROM VENDORS---   
        Purchase = 0,
        // Transactions which decrease items in a store -- SELLING TO CUSTOMERS---   
        Sales = 1
    }
}