# MiniBank
## Author - Zvezdin Roman

Демо проект, моделирующий работу приложения банка. 

Основной функционал:
- Конвертация валют с использование курса со стороннего ресурса
- Механизм аккаунтов пользователей
- Механизм банковских счетов пользователей
- Механизм перевода средств между счетами, с учетом курса конвертации валют

Детали реализации:
- Стэк: C#, ASP.NET Core, WebAPI, PostgreSQL, EntityFramework Core.
- Приложение реализовано по REST-архитектуре
- Реализована контейнеризация приложения
- Реализован паттерн репозиторий
- Приложение разделено на слои (Core, Data, Web)
- Реализовано unit-тестирование бизнес-логики приложения (Xunit)
- Реализован механизм аутентификации, при помощи Jwt-токена и Duende Identity server
- Для работы с БД использован EF Core, подход CodeFirst и механизм миграций
- Реализовано асинхронное выполнение методов API
- Реализован маппинг объектов сущностей между слоями приложения
