# ExchangeRatesService

Прошу простить тех, кто будет скачивать этот репозиторий, в нем бинарники и портативный постгрес.

Здесь содержится микросервис, который на вход получает идентификатор валюты (например KZT) и дату (сегодня, если не передана) и возвращает курс этой валюты к рублю по данным ЦБ РФ. Если курс уже есть в базе, возвращается из базы, иначе дергаем API ЦБ РФ и сохраняем данные у себя в базе.
- ASP.NET Core > 2
- Вероятность успешной записи в базу ~60%
- Postgres

Композиция сервиса происходит с помощью SimpleInjector. Запись в бд происходит в отдельном потоке с использованием ConcurrentQueue, чтобы не увеличивать отклик приложения из-за возможных фейлов работы с бд.

Перед запуском сервиса желательно запустить постгрес из PostgreSQLPortable-10.1.1\PostgreSQLPortable.exe.
В субд должна быть бд Valutes с таблицей Valute

DROP TABLE public."Valute";
CREATE TABLE public."Valute"
(
  "Name" text NOT NULL,
  "Date" date NOT NULL,
  "Rate" money,
  CONSTRAINT "Valute_pkey" PRIMARY KEY ("Date", "Name")
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public."Valute"
  OWNER TO postgres;
