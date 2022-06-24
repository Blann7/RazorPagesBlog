using System.ComponentModel.DataAnnotations;

namespace LogViewer.Models
{
    public static class LogTypes
    {
        public enum LogType
        {
            [Display(Name = "Не выбрано")] NONE, [Display(Name = "Покупка роли")] BUY_ROLE, [Display(Name = "Роль истекла")] EXPIRED_ROLE,
            [Display(Name = "Пополнение денег")] GET_MONEY, [Display(Name = "Трата денег")] SPENT_MONEY, 
            [Display(Name = "Вход")] LOGIN, [Display(Name = "Выход")] LOGOUT, 
            [Display(Name = "Реферал приглашён")] REFERRAL_INVITE, [Display(Name = "Создана реф. ссылка")] REFERRAL_GENERATE_NEW_LINK,
            [Display(Name = "Блокировка")] BAN, [Display(Name = "Разблокировка")] UNBAN, 
            [Display(Name = "about us ред.")] EDIT_ABOUT_US, [Display(Name = "Изменение пользователя")] EDIT_USER, 
            [Display(Name = "Изменение руководства")] EDIT_HEADS, [Display(Name = "Изменение рефералки")] EDIT_REFERRAL, 
            [Display(Name = "Удаление рефералки")] DELETE_REFERRAL, [Display(Name = "Добавление статьи")] ADD_ARTICLE,
            [Display(Name = "Изменение статьи")] CHANGE_ARTICLE, [Display(Name = "Удаление статьи")] DELETE_ARTICLE,
            [Display(Name = "Изменение IP")] CHANGE_IP, [Display(Name = "Регистрация")] REGISTER_USER
        }
        public static readonly Dictionary<LogType, string> logTypesKeyEnum = new()
        {
            [LogType.NONE] = "Не выбрано",
            [LogType.BUY_ROLE] = "Покупка роли",
            [LogType.EXPIRED_ROLE] = "Роль истекла",
            [LogType.GET_MONEY] = "Пополнение денег",
            [LogType.SPENT_MONEY] = "Трата денег",
            [LogType.LOGIN] = "Вход",
            [LogType.LOGOUT] = "Выход",
            [LogType.REFERRAL_INVITE] = "Реферал приглашён",
            [LogType.REFERRAL_GENERATE_NEW_LINK] = "Создана реф. ссылка",
            [LogType.BAN] = "Блокировка",
            [LogType.UNBAN] = "Разблокировка",
            [LogType.EDIT_ABOUT_US] = "about us ред.",
            [LogType.EDIT_USER] = "Изменение пользователя",
            [LogType.EDIT_HEADS] = "Изменение руководства",
            [LogType.EDIT_REFERRAL] = "Изменение рефералки",
            [LogType.DELETE_REFERRAL] = "Удаление рефералки",
            [LogType.ADD_ARTICLE] = "Добавление статьи",
            [LogType.CHANGE_ARTICLE] = "Изменение статьи",
            [LogType.DELETE_ARTICLE] = "Удаление статьи",
            [LogType.CHANGE_IP] = "Изменение IP",
            [LogType.REGISTER_USER] = "Регистрация"
        };
    }
}