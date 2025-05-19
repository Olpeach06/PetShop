using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop
{
    public partial class ZAKAZ
    {
        // Вычисляемая сумма заказа
        public decimal TotalAmount
        {
            get
            {
                return PURCHASE?.Sum(p => p.quantity * (p.PRODUCTS?.price ?? 0)) ?? 0;
            }
        }

        // Полное имя пользователя
        public string UserFullName
        {
            get
            {
                return $"{USERS?.last_name} {USERS?.first_name} {USERS?.patronymic}";
            }
        }

        // Альтернативный вариант, если нужно просто одно поле из Users
        public string UserName
        {
            get
            {
                return $"{USERS?.last_name} {USERS?.first_name} {USERS?.patronymic}".Trim();
            }
        }
    }
}
