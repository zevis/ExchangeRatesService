# ExchangeRatesService

Прошу простить тех, кто будет скачивать этот репозиторий, в нем бинарники, не настроен .gitignore.
Можно было бы еще вынести некоторые классы в другие проекты, но уже появится чувство оверинжиниринга.

Здесь содержится микросервис, который на вход получает идентификатор валюты (например KZT) и дату (сегодня, если не передана) и возвращает курс этой валюты к рублю по данным ЦБ РФ. Если курс уже есть в базе, возвращается из базы, иначе дергаем API ЦБ РФ и сохраняем данные у себя в базе.
- ASP.NET Core > 2
- Моделируется неуспешная запись в базу с вероятностью ~40%
- Postgres

Композиция сервиса происходит с помощью SimpleInjector, для логгирования Nlog. 
Запись в бд происходит в отдельном потоке с использованием ConcurrentQueue, чтобы не увеличивать отклик сервиса из-за возможных фейлов кеширования в бд.

Перед запуском сервиса желательно запустить постгрес из https://sourceforge.net/projects/pgadminportable/.
В субд с помощью консоли или https://sourceforge.net/projects/postgresqlportable/ создать бд Currencies с таблицей Currency

DROP TABLE public."Currency";
CREATE TABLE public."Currency"
(
  "Name" text NOT NULL,
  "Date" date NOT NULL,
  "Rate" numeric,
  CONSTRAINT "Currency_pkey" PRIMARY KEY ("Date", "Name")
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public."Currency"
  OWNER TO postgres;
