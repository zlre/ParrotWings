# ParrotWings

<p>Небольшой тестовый пример на WebApi ASP.NET Core 3 и Angular 9 - "Parrot Wings".</p>
<p>Бэк стартует по дефолту на https://localhost:5001/. Если меняется адрес бека, то на фронте в enviroments тоже надо изменить адрес хоста.</p>
<p>В качестве orm используется EFCore 3, для отслеживания изменений в базе - SqlTableDependency.</p>
<p>Обмен данными между фронтом и беком посредством REST и SignalR.</p>

# StudentManagerFront

<p>Фронт на Angular 9. Предварительно необходимо выполнить npm i для установки пакетов. Запускается командой `ng serve` на http://localhost:4200/</p>
<p>Для верстки используется bootstrap 4, для пользовательских нотификаций toastr.</p>

# DataBase

<p>База на ms sql server. Используется code first подход к созданию таблиц.</p>
<p>Приложение использует service broker для отслеживания транзакций и баланса пользователя, поэтому его необходимо включить для базы:</p>
<pre>ALTER DATABASE test SET ENABLE_BROKER</pre>
