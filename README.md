
Wep Application that represents a point of sales,
There are many stores each store hold an inventory of a specific products.
One product can be existed in many stores.
***********************************************************************************

There are two types of transactions
    * Purchase Transaction, that is transaction that increases a product in the store, 
      those are related to purchasing products from vendors
    * Sale Transaction, that is transaction that decrease a product in the store, 
      those are related to selling products to customers
*****************************************************************************************

The web app is developed using 
    * ASP.NET MVC
    * Entity Framework
    * Boostrap
    * SQL Server Database
    
******************************************************************************************
The web application insures and validates products in stores, so that at any point of time 
the count of a product in a store can not go beyond zero after ****** ADDING or DELETING or UPDATING ******* a Transaction
