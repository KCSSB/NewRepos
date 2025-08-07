Шаги для правильного запуска контейнеров бекенд приложения:
1. Скачать и установить docker desktop
2. Запустить docker desktop
3. Открыть решение DataBaseTrello.sln внутри Visual Studio
4. Открыть Powershall для разработчиков внутри VS(консоль)
5. docker compose down -v
6. Ввести docker compose build --no-cache (Дождаться конца сборки)
7. Ввести docker compose up -d