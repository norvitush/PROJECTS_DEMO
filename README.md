# PROJECTS_DEMO
В репозитории представлено 2 рабочих проекта  на Unity. Они живые и существуют в Google Play (JUMP SURVIVAL 3D, SANTA JAM: XMAS BASKETBALL 3D).

В каждом проекте заложена архитектура для дальнейшего развития:
- данные загружаются из JSON;
- имеются загрузчик сцены, менеджер пула объектов, тач-менеджер, Дата-кипер для хранения игровой информации в зашифрованном виде,
  DB менеджер, который занимается загрузкой данных (пока локальных но можно и расширить) и доступом к данным,
  евент-мнеджер с возможностью подписки-отписки и несложным созданием новых ивентов собственного производства.

Также в репозитории отдельно представлен минимальный функционал для работы с сущностями Эффектов через Фабрику VFXFabriс,
с расширениями для GameObject позволяющие запускать удобно запускать карутины разными способами: 
активировать на время, активировать с задержкой, c Задержкой и на определенный период и т.п.

В проектах также существуют VFX собственного изготовления с набором чб сэмплов для различных частиц, 3d-модели (домики, окружение и т.д.).

Также в репозитории находится небольшой пэт-проект для WPF.NET для ознакомления с реализацией паттерна MVVM с биндом на XAML.
