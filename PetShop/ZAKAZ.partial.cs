using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop
{
    public partial class Zakazy
    {
        // Вычисляемая сумма заказа
        public decimal TotalAmount
        {
            get
            {
                return Purchases?.Sum(p => p.Quantity * (p.Products?.Price ?? 0)) ?? 0;
            }
        }

        // Полное имя пользователя
        public string UserFullName
        {
            get
            {
                return $"{Users?.LastName} {Users?.FirstName} {Users?.Patronymic}";
            }
        }

        // Альтернативный вариант, если нужно просто одно поле из Users
        public string UserName
        {
            get
            {
                return $"{Users?.LastName} {Users?.FirstName} {Users?.Patronymic}".Trim();
            }
        }
    }
}
